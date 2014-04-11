using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rock.DependencyInjection;

namespace Rock.Logging
{
    public class Logger : ILogger
    {
        private bool _isInitialized;

        private ILoggerConfiguration _configuration;
        private IThrottlingRuleEvaluator _throttlingRuleEvaluator;
        private ILogProvider _auditLogProvider;
        private IEnumerable<ILogProvider> _logProviders;
        private IEnumerable<IContextProvider> _contextProviders;

        public virtual void Init(IResolver container)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;

                container.SetValueFor(out _configuration);
                container.SetValueFor(out _configuration);
                container.SetValueFor(out _throttlingRuleEvaluator);
                container.SetValueFor(out _auditLogProvider, null);
                container.SetValueFor(out _logProviders);
                container.SetValueFor(out _contextProviders, Enumerable.Empty<IContextProvider>);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            CheckInitialization();

            return
                _configuration.IsLoggingEnabled
                && logLevel != LogLevel.None
                && logLevel >= _configuration.LoggingLevel;
        }

        public async Task Log(LogEntry logEntry)
        {
            CheckInitialization();

            if (!IsEnabled(logEntry.LogLevel)
                || (_throttlingRuleEvaluator != null && !_throttlingRuleEvaluator.ShouldLog(logEntry)))
            {
                return;
            }

            AddContextData(logEntry);
            foreach (var contextProvider in _contextProviders)
            {
                contextProvider.AddContextData(logEntry);
            }

            if (logEntry.LogLevel == LogLevel.Audit && _auditLogProvider != null)
            {
                await _auditLogProvider.Write(logEntry);
            }
            else
            {
                await Task.WhenAll(_logProviders.Select(logProvider => logProvider.Write(logEntry)));
            }
        }

        private void CheckInitialization()
        {
            if (!_isInitialized)
            {
                throw new Exception("Attempting to use Logger before Init() was called."); // TODO: better message, exception type
            }
        }

        protected virtual void AddContextData(LogEntry entry)
        {
        }
    }

    public static class SyncExtension
    {
        /// <summary>
        /// A Task that has already been completed successfully.
        /// </summary>
        private static readonly Task _completedTask = Task.FromResult(0);

        public static ILogger Sync(this ILogger logger, bool synchronous = true)
        {
            return new SyncLogger(logger, synchronous);
        }

        private class SyncLogger : ILogger
        {
            private readonly ILogger _logger;
            private readonly bool _synchronous;

            public SyncLogger(ILogger logger, bool synchronous)
            {
                _logger = logger;
                _synchronous = synchronous;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return _logger.IsEnabled(logLevel);
            }

            public Task Log(LogEntry logEntry)
            {
                if (!_synchronous)
                {
                    // If we're asynchronous, just do what we would have done otherwise.
                    return _logger.Log(logEntry);
                }

                _logger.Log(logEntry).Wait();
                return _completedTask;
            }
        }
    }

    /// <summary>
    /// Evaluates wheter a log entry should be logged or not
    /// </summary>
    public interface IThrottlingRuleEvaluator
    {
        /// <summary>
        /// Determines whether the specified log entry should be logged.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <returns>
        /// <c>true</c> if the specified log entry should be logged; otherwise, <c>false</c>.
        /// </returns>
        bool ShouldLog(LogEntry logEntry);
    }

    public static class ToThrottlingRuleEvaluatorExtension
    {
        public static IThrottlingRuleEvaluator ToThrottlingRuleEvaluator(this IThrottlingRuleConfiguration throttlingRule)
        {
            return new ThrottlingRuleEvaluator(throttlingRule);
        }
    }

    public class ThrottlingRuleEvaluator : IThrottlingRuleEvaluator
    {
        private readonly IThrottlingRuleConfiguration _throttlingRule;

        private static readonly Dictionary<int, RuleTracker> _lastTimeLogged = new Dictionary<int, RuleTracker>();
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottlingRuleEvaluator"/> class.
        /// </summary>
        /// <param name="throttlingRule">The throttling rule.</param>
        public ThrottlingRuleEvaluator(IThrottlingRuleConfiguration throttlingRule)
        {
            _throttlingRule = throttlingRule;
        }

        public bool ShouldLog(LogEntry logEntry)
        {
            int key = logEntry.GetHashCode();

            if (_throttlingRule.MinEventThreshold <= 1 && _throttlingRule.MinInterval == TimeSpan.Zero)
            {
                return true;
            }

            bool toReturn = true;

            lock (_lockObject)
            {
                if (_lastTimeLogged.ContainsKey(key))
                {
                    if (_throttlingRule.MinEventThreshold <= 1 && _throttlingRule.MinInterval != TimeSpan.Zero)
                    {
                        if (_lastTimeLogged[key].LastTimeLogged.Add(_throttlingRule.MinInterval) > DateTime.Now)
                        {
                            RuleTracker tracker = _lastTimeLogged[key];
                            tracker.MessagesSkippedSinceLastLog += 1;
                            _lastTimeLogged[key] = tracker;
                            toReturn = false;
                        }
                        else
                        {
                            logEntry.ExtendedProperties.Add("ThrottledMessagesSkippedSinceLastLog", _lastTimeLogged[key].MessagesSkippedSinceLastLog.ToString());
                            _lastTimeLogged[key] = new RuleTracker(DateTime.Now, 0);
                        }
                    }
                    else if (_throttlingRule.MinEventThreshold > 1 && _throttlingRule.MinInterval == TimeSpan.Zero)
                    {
                        if (_lastTimeLogged[key].MessagesSkippedSinceLastLog < _throttlingRule.MinEventThreshold - 1)
                        {
                            RuleTracker tracker = _lastTimeLogged[key];
                            tracker.MessagesSkippedSinceLastLog += 1;
                            _lastTimeLogged[key] = tracker;
                            toReturn = false;
                        }
                        else
                        {
                            logEntry.ExtendedProperties.Add("ThrottledMessagesSkippedSinceLastLog", _lastTimeLogged[key].MessagesSkippedSinceLastLog.ToString());
                            _lastTimeLogged[key] = new RuleTracker(DateTime.Now, 0);
                        }
                    }
                    else if (_throttlingRule.MinEventThreshold > 1 && _throttlingRule.MinInterval != TimeSpan.Zero)
                    {
                        if (_lastTimeLogged[key].LastTimeLogged.Add(_throttlingRule.MinInterval) > DateTime.Now && _lastTimeLogged[key].MessagesSkippedSinceLastLog < _throttlingRule.MinEventThreshold - 1)
                        {
                            RuleTracker tracker = _lastTimeLogged[key];
                            tracker.MessagesSkippedSinceLastLog += 1;
                            _lastTimeLogged[key] = tracker;
                            toReturn = false;
                        }
                        else
                        {
                            logEntry.ExtendedProperties.Add("ThrottledMessagesSkippedSinceLastLog", _lastTimeLogged[key].MessagesSkippedSinceLastLog.ToString());
                            _lastTimeLogged[key] = new RuleTracker(DateTime.Now, 0);
                        }
                    }

                }
                else if (_throttlingRule.MinEventThreshold == -1 || _throttlingRule.MinEventThreshold > 1)
                {
                    _lastTimeLogged.Add(key, new RuleTracker(DateTime.Now, 1));
                    toReturn = false;
                }
                else
                {
                    _lastTimeLogged.Add(key, new RuleTracker(DateTime.Now, 0));
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Tracks throttling rule information.
        /// </summary>
        private struct RuleTracker
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RuleTracker"/> structure.
            /// </summary>
            /// <param name="lastTimeLogged">The last time logged.</param>
            /// <param name="messagesSkippedSinceLastLog">The messages skipped since last log.</param>
            public RuleTracker(DateTime lastTimeLogged, int messagesSkippedSinceLastLog)
            {
                LastTimeLogged = lastTimeLogged;
                MessagesSkippedSinceLastLog = messagesSkippedSinceLastLog;
            }

            /// <summary>
            /// Gets or sets the last time logged.
            /// </summary>
            public DateTime LastTimeLogged;

            /// <summary>
            /// Gets or sets the messages skipped since last log.
            /// </summary>
            public int MessagesSkippedSinceLastLog;
        }
    }
}