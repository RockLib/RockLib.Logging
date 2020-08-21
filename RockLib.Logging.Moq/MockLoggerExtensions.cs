using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Moq;

namespace RockLib.Logging.Moq
{
    /// <summary>
    /// Provides extension methods for setting up and verifying instances of <see cref="Mock{T}"/> of type
    /// <see cref="ILogger"/>.
    /// </summary>
    public static class MockLoggerExtensions
    {
        /// <summary>
        /// Specifies setups on the mock logger for the <see cref="ILogger.Level"/> and <see cref="ILogger.Name"/>
        /// properties.
        /// </summary>
        /// <param name="mockLogger">The mock logger to perform setups on.</param>
        /// <param name="level">The level of the mock logger.</param>
        /// <param name="name">The name of the mock logger.</param>
        /// <returns>The same mock logger.</returns>
        public static Mock<ILogger> SetupLogger(this Mock<ILogger> mockLogger, LogLevel level = LogLevel.Debug, string name = Logger.DefaultName)
        {
            mockLogger.Setup(m => m.Level).Returns(level);
            mockLogger.Setup(m => m.Name).Returns(name ?? Logger.DefaultName);

            return mockLogger;
        }

        /// <summary>
        /// Verifies that the mock logger logged at <see cref="LogLevel.Debug"/> the number of times specified
        /// by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log at <see cref="LogLevel.Debug"/> the number of times specified by
        /// <paramref name="times"/>.
        /// </exception>
        public static void VerifyDebug(this Mock<ILogger> mockLogger, Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(LogLevel.Debug, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged at <see cref="LogLevel.Info"/> the number of times specified
        /// by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log at <see cref="LogLevel.Info"/> the number of times specified by
        /// <paramref name="times"/>.
        /// </exception>
        public static void VerifyInfo(this Mock<ILogger> mockLogger, Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(LogLevel.Info, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged at <see cref="LogLevel.Warn"/> the number of times specified
        /// by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log at <see cref="LogLevel.Warn"/> the number of times specified by
        /// <paramref name="times"/>.
        /// </exception>
        public static void VerifyWarn(this Mock<ILogger> mockLogger, Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(LogLevel.Warn, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged at <see cref="LogLevel.Error"/> the number of times specified
        /// by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log at <see cref="LogLevel.Error"/> the number of times specified by
        /// <paramref name="times"/>.
        /// </exception>
        public static void VerifyError(this Mock<ILogger> mockLogger, Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(LogLevel.Error, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged at <see cref="LogLevel.Fatal"/> the number of times specified
        /// by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log at <see cref="LogLevel.Fatal"/> the number of times specified by
        /// <paramref name="times"/>.
        /// </exception>
        public static void VerifyFatal(this Mock<ILogger> mockLogger, Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(LogLevel.Fatal, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged at <see cref="LogLevel.Audit"/> the number of times specified
        /// by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log at <see cref="LogLevel.Audit"/> the number of times specified by
        /// <paramref name="times"/>.
        /// </exception>
        public static void VerifyAudit(this Mock<ILogger> mockLogger, Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(LogLevel.Audit, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged at any <see cref="LogLevel"/> the number of times specified
        /// by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log at any <see cref="LogLevel"/> the number of times specified by
        /// <paramref name="times"/>.
        /// </exception>
        public static void VerifyLog(this Mock<ILogger> mockLogger, Times? times, string failMessage) =>
            mockLogger.VerifyLog((LogLevel?)null, times, failMessage);

        private static void VerifyLog(this Mock<ILogger> mockLogger, LogLevel? logLevel, Times? times, string failMessage)
        {
            if (mockLogger == null)
                throw new ArgumentNullException(nameof(mockLogger));

            Expression<Func<LogEntry, bool>> matchingLogEntry = logEntry => !logLevel.HasValue || logEntry.Level == logLevel.Value;

            mockLogger.Verify(m => m.Log(It.Is(matchingLogEntry), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                times ?? Times.Once(), failMessage);
        }

        /// <summary>
        /// Verifies that the mock logger logged with the message specified by <paramref name="messagePattern"/>
        /// at <see cref="LogLevel.Debug"/> the number of times specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="messagePattern">
        /// A regular expression pattern that the message of a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the message specified by <paramref name="messagePattern"/>
        /// at <see cref="LogLevel.Debug"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyDebug(this Mock<ILogger> mockLogger, string messagePattern,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, LogLevel.Debug, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the message specified by <paramref name="messagePattern"/>
        /// at <see cref="LogLevel.Info"/> the number of times specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="messagePattern">
        /// A regular expression pattern that the message of a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the message specified by <paramref name="messagePattern"/>
        /// at <see cref="LogLevel.Info"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyInfo(this Mock<ILogger> mockLogger, string messagePattern,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, LogLevel.Info, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the message specified by <paramref name="messagePattern"/>
        /// at <see cref="LogLevel.Warn"/> the number of times specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="messagePattern">
        /// A regular expression pattern that the message of a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the message specified by <paramref name="messagePattern"/>
        /// at <see cref="LogLevel.Warn"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyWarn(this Mock<ILogger> mockLogger, string messagePattern,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, LogLevel.Warn, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the message specified by <paramref name="messagePattern"/>
        /// at <see cref="LogLevel.Error"/> the number of times specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="messagePattern">
        /// A regular expression pattern that the message of a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the message specified by <paramref name="messagePattern"/>
        /// at <see cref="LogLevel.Error"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyError(this Mock<ILogger> mockLogger, string messagePattern,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, LogLevel.Error, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the message specified by <paramref name="messagePattern"/>
        /// at <see cref="LogLevel.Fatal"/> the number of times specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="messagePattern">
        /// A regular expression pattern that the message of a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the message specified by <paramref name="messagePattern"/>
        /// at <see cref="LogLevel.Fatal"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyFatal(this Mock<ILogger> mockLogger, string messagePattern,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, LogLevel.Fatal, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the message specified by <paramref name="messagePattern"/>
        /// at <see cref="LogLevel.Audit"/> the number of times specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="messagePattern">
        /// A regular expression pattern that the message of a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the message specified by <paramref name="messagePattern"/>
        /// at <see cref="LogLevel.Audit"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyAudit(this Mock<ILogger> mockLogger, string messagePattern,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, LogLevel.Audit, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the message specified by <paramref name="messagePattern"/>
        /// at any <see cref="LogLevel"/> the number of times specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="messagePattern">
        /// A regular expression pattern that the message of a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the message specified by <paramref name="messagePattern"/>
        /// at any <see cref="LogLevel"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyLog(this Mock<ILogger> mockLogger, string messagePattern,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, null, times, failMessage);

        private static void VerifyLog(this Mock<ILogger> mockLogger, string messagePattern,
            LogLevel? logLevel, Times? times, string failMessage)
        {
            if (mockLogger == null)
                throw new ArgumentNullException(nameof(mockLogger));
            if (messagePattern == null)
                throw new ArgumentNullException(nameof(messagePattern));

            var messageRegex = new Regex(messagePattern);

            Expression<Func<LogEntry, bool>> matchingLogEntry = logEntry =>
                messageRegex.IsMatch(logEntry.Message)
                && (!logLevel.HasValue || logEntry.Level == logLevel.Value);

            mockLogger.Verify(m => m.Log(It.Is(matchingLogEntry), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                times ?? Times.Once(), failMessage);
        }

        /// <summary>
        /// Verifies that the mock logger logged with the extended properties specified by
        /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Debug"/> the number of times
        /// specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="extendedProperties">
        /// An object representing the extended properties that a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the extended properties specified by <paramref name="extendedProperties"/>
        /// at <see cref="LogLevel.Debug"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyDebug(this Mock<ILogger> mockLogger, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(extendedProperties, LogLevel.Debug, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the extended properties specified by
        /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Info"/> the number of times
        /// specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="extendedProperties">
        /// An object representing the extended properties that a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the extended properties specified by <paramref name="extendedProperties"/>
        /// at <see cref="LogLevel.Info"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyInfo(this Mock<ILogger> mockLogger, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(extendedProperties, LogLevel.Info, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the extended properties specified by
        /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Warn"/> the number of times
        /// specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="extendedProperties">
        /// An object representing the extended properties that a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the extended properties specified by <paramref name="extendedProperties"/>
        /// at <see cref="LogLevel.Warn"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyWarn(this Mock<ILogger> mockLogger, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(extendedProperties, LogLevel.Warn, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the extended properties specified by
        /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Error"/> the number of times
        /// specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="extendedProperties">
        /// An object representing the extended properties that a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the extended properties specified by <paramref name="extendedProperties"/>
        /// at <see cref="LogLevel.Error"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyError(this Mock<ILogger> mockLogger, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(extendedProperties, LogLevel.Error, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the extended properties specified by
        /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Fatal"/> the number of times
        /// specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="extendedProperties">
        /// An object representing the extended properties that a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the extended properties specified by <paramref name="extendedProperties"/>
        /// at <see cref="LogLevel.Fatal"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyFatal(this Mock<ILogger> mockLogger, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(extendedProperties, LogLevel.Fatal, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the extended properties specified by
        /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Audit"/> the number of times
        /// specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="extendedProperties">
        /// An object representing the extended properties that a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the extended properties specified by <paramref name="extendedProperties"/>
        /// at <see cref="LogLevel.Audit"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyAudit(this Mock<ILogger> mockLogger, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(extendedProperties, LogLevel.Audit, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the extended properties specified by
        /// <paramref name="extendedProperties"/> at any <see cref="LogLevel"/> the number of times
        /// specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="extendedProperties">
        /// An object representing the extended properties that a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the extended properties specified by <paramref name="extendedProperties"/>
        /// at any <see cref="LogLevel"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyLog(this Mock<ILogger> mockLogger, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(extendedProperties, null, times, failMessage);

        private static void VerifyLog(this Mock<ILogger> mockLogger, object extendedProperties,
            LogLevel? logLevel, Times? times, string failMessage)
        {
            if (mockLogger == null)
                throw new ArgumentNullException(nameof(mockLogger));
            if (extendedProperties == null)
                throw new ArgumentNullException(nameof(extendedProperties));

            var properties = GetExtendedPropertiesDictionary(extendedProperties);

            Func<LogEntry, bool> hasMatchingExtendedProperties = logEntry =>
            {
                foreach (var property in properties)
                {
                    if (!logEntry.ExtendedProperties.TryGetValue(property.Key, out var value)
                        || !value.GetType().IsAssignableFrom(property.Value.GetType()))
                        return false;

                    if (!AreEquivalent(property.Value, value))
                        return false;
                }
                return true;
            };

            Expression<Func<LogEntry, bool>> matchingLogEntry = logEntry =>
                (!logLevel.HasValue || logEntry.Level == logLevel.Value)
                && hasMatchingExtendedProperties(logEntry);

            mockLogger.Verify(m => m.Log(It.Is(matchingLogEntry), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                times ?? Times.Once(), failMessage);
        }

        /// <summary>
        /// Verifies that the mock logger logged with the message specified by <paramref name="messagePattern"/>
        /// and the extended properties specified by <paramref name="extendedProperties"/> at
        /// <see cref="LogLevel.Debug"/> the number of times specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="messagePattern">
        /// A regular expression pattern that the message of a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="extendedProperties">
        /// An object representing the extended properties that a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the message specified by <paramref name="messagePattern"/>
        /// and the extended properties specified by <paramref name="extendedProperties"/> at
        /// <see cref="LogLevel.Debug"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyDebug(this Mock<ILogger> mockLogger, string messagePattern, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, extendedProperties, LogLevel.Debug, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the message specified by <paramref name="messagePattern"/>
        /// and the extended properties specified by <paramref name="extendedProperties"/> at
        /// <see cref="LogLevel.Info"/> the number of times specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="messagePattern">
        /// A regular expression pattern that the message of a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="extendedProperties">
        /// An object representing the extended properties that a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the message specified by <paramref name="messagePattern"/>
        /// and the extended properties specified by <paramref name="extendedProperties"/> at
        /// <see cref="LogLevel.Info"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyInfo(this Mock<ILogger> mockLogger, string messagePattern, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, extendedProperties, LogLevel.Info, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the message specified by <paramref name="messagePattern"/>
        /// and the extended properties specified by <paramref name="extendedProperties"/> at
        /// <see cref="LogLevel.Warn"/> the number of times specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="messagePattern">
        /// A regular expression pattern that the message of a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="extendedProperties">
        /// An object representing the extended properties that a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the message specified by <paramref name="messagePattern"/>
        /// and the extended properties specified by <paramref name="extendedProperties"/> at
        /// <see cref="LogLevel.Warn"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyWarn(this Mock<ILogger> mockLogger, string messagePattern, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, extendedProperties, LogLevel.Warn, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the message specified by <paramref name="messagePattern"/>
        /// and the extended properties specified by <paramref name="extendedProperties"/> at
        /// <see cref="LogLevel.Error"/> the number of times specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="messagePattern">
        /// A regular expression pattern that the message of a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="extendedProperties">
        /// An object representing the extended properties that a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the message specified by <paramref name="messagePattern"/>
        /// and the extended properties specified by <paramref name="extendedProperties"/> at
        /// <see cref="LogLevel.Error"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyError(this Mock<ILogger> mockLogger, string messagePattern, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, extendedProperties, LogLevel.Error, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the message specified by <paramref name="messagePattern"/>
        /// and the extended properties specified by <paramref name="extendedProperties"/> at
        /// <see cref="LogLevel.Fatal"/> the number of times specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="messagePattern">
        /// A regular expression pattern that the message of a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="extendedProperties">
        /// An object representing the extended properties that a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the message specified by <paramref name="messagePattern"/>
        /// and the extended properties specified by <paramref name="extendedProperties"/> at
        /// <see cref="LogLevel.Fatal"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyFatal(this Mock<ILogger> mockLogger, string messagePattern, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, extendedProperties, LogLevel.Fatal, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the message specified by <paramref name="messagePattern"/>
        /// and the extended properties specified by <paramref name="extendedProperties"/> at
        /// <see cref="LogLevel.Audit"/> the number of times specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="messagePattern">
        /// A regular expression pattern that the message of a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="extendedProperties">
        /// An object representing the extended properties that a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the message specified by <paramref name="messagePattern"/>
        /// and the extended properties specified by <paramref name="extendedProperties"/> at
        /// <see cref="LogLevel.Audit"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyAudit(this Mock<ILogger> mockLogger, string messagePattern, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, extendedProperties, LogLevel.Audit, times, failMessage);

        /// <summary>
        /// Verifies that the mock logger logged with the message specified by <paramref name="messagePattern"/>
        /// and the extended properties specified by <paramref name="extendedProperties"/> at any
        /// <see cref="LogLevel"/> the number of times specified by <paramref name="times"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="messagePattern">
        /// A regular expression pattern that the message of a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="extendedProperties">
        /// An object representing the extended properties that a log must match in order for successful
        /// verification to occur.
        /// </param>
        /// <param name="times">
        /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
        /// <see cref="Times.Once()"/> is used.
        /// </param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        /// <exception cref="MockException">
        /// The logger did not log with the message specified by <paramref name="messagePattern"/>
        /// and the extended properties specified by <paramref name="extendedProperties"/> at any
        /// <see cref="LogLevel"/> the number of times specified by <paramref name="times"/>.
        /// </exception>
        public static void VerifyLog(this Mock<ILogger> mockLogger, string messagePattern, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, extendedProperties, null, times, failMessage);

        private static void VerifyLog(this Mock<ILogger> mockLogger, string messagePattern, object extendedProperties,
            LogLevel? logLevel, Times? times, string failMessage)
        {
            if (mockLogger == null)
                throw new ArgumentNullException(nameof(mockLogger));
            if (messagePattern == null)
                throw new ArgumentNullException(nameof(messagePattern));
            if (extendedProperties == null)
                throw new ArgumentNullException(nameof(extendedProperties));

            var messageRegex = new Regex(messagePattern);

            var properties = GetExtendedPropertiesDictionary(extendedProperties);

            Func<LogEntry, bool> hasMatchingExtendedProperties = logEntry =>
            {
                foreach (var property in properties)
                {
                    if (!logEntry.ExtendedProperties.TryGetValue(property.Key, out var value)
                        || !value.GetType().IsAssignableFrom(property.Value.GetType()))
                        return false;

                    if (!AreEquivalent(property.Value, value))
                        return false;
                    
                }
                return true;
            };

            Expression<Func<LogEntry, bool>> matchingLogEntry = logEntry =>
                messageRegex.IsMatch(logEntry.Message)
                && (!logLevel.HasValue || logEntry.Level == logLevel.Value)
                && hasMatchingExtendedProperties(logEntry);

            mockLogger.Verify(m => m.Log(It.Is(matchingLogEntry), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                times ?? Times.Once(), failMessage);
        }

        private static bool AreEquivalent(object lhs, object rhs)
        {
            switch (lhs)
            {
                case string pattern:
                    return Regex.IsMatch((string)rhs, pattern);
                default:
                    if (lhs is object[] lhsArray && rhs is object[] rhsArray && lhsArray.Length == rhsArray.Length)
                    {
                        for (int i = 0; i < lhsArray.Length; i++)
                            if (!AreEquivalent(lhsArray[i], rhsArray[i]))
                                return false;
                        return true;
                    }
                    return Equals(lhs, rhs);
            }
        }

        private static Dictionary<string, object> GetExtendedPropertiesDictionary(object extendedProperties)
        {
            var logEntry = new LogEntry();
            logEntry.SetExtendedProperties(extendedProperties);
            return logEntry.ExtendedProperties;
        }
    }
}
