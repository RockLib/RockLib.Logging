using System;
using System.Collections.Generic;

namespace Rock.Logging
{
    public class ThrottlingRuleEvaluator : IThrottlingRuleEvaluator
    {
        private readonly int _minEventThreshold;
        private readonly TimeSpan _minInterval;

        private static readonly Dictionary<int, RuleTracker> _lastTimeLogged = new Dictionary<int, RuleTracker>();
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottlingRuleEvaluator"/> class.
        /// </summary>
        public ThrottlingRuleEvaluator(int minEventThreshold, TimeSpan minInterval)
        {
            _minEventThreshold = minEventThreshold;
            _minInterval = minInterval;
        }

        public bool ShouldLog(ILogEntry logEntry)
        {
            int key = logEntry.GetThrottlingHashCode();

            if (_minEventThreshold <= 1 && _minInterval == TimeSpan.Zero)
            {
                return true;
            }

            bool toReturn = true;

            lock (_lockObject)
            {
                if (_lastTimeLogged.ContainsKey(key))
                {
                    if (_minEventThreshold <= 1 && _minInterval != TimeSpan.Zero)
                    {
                        if (_lastTimeLogged[key].LastTimeLogged.Add(_minInterval) > DateTime.Now)
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
                    else if (_minEventThreshold > 1 && _minInterval == TimeSpan.Zero)
                    {
                        if (_lastTimeLogged[key].MessagesSkippedSinceLastLog < _minEventThreshold - 1)
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
                    else if (_minEventThreshold > 1 && _minInterval != TimeSpan.Zero)
                    {
                        if (_lastTimeLogged[key].LastTimeLogged.Add(_minInterval) > DateTime.Now && _lastTimeLogged[key].MessagesSkippedSinceLastLog < _minEventThreshold - 1)
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
                else if (_minEventThreshold == -1 || _minEventThreshold > 1)
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