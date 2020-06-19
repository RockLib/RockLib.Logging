using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Moq;

namespace RockLib.Logging.Moq
{
    public static class MockLoggerExtensions
    {
        public static Mock<ILogger> Setup(this Mock<ILogger> mockLogger, LogLevel level = LogLevel.Debug, string name = Logger.DefaultName)
        {
            mockLogger.Setup(m => m.Level).Returns(level);
            mockLogger.Setup(m => m.Name).Returns(name ?? Logger.DefaultName);

            return mockLogger;
        }

        public static void VerifyDebug(this Mock<ILogger> mockLogger, Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(LogLevel.Debug, times, failMessage);

        public static void VerifyInfo(this Mock<ILogger> mockLogger, Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(LogLevel.Info, times, failMessage);

        public static void VerifyWarn(this Mock<ILogger> mockLogger, Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(LogLevel.Warn, times, failMessage);

        public static void VerifyError(this Mock<ILogger> mockLogger, Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(LogLevel.Error, times, failMessage);

        public static void VerifyFatal(this Mock<ILogger> mockLogger, Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(LogLevel.Fatal, times, failMessage);

        public static void VerifyAudit(this Mock<ILogger> mockLogger, Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(LogLevel.Audit, times, failMessage);

        private static void VerifyLog(this Mock<ILogger> mockLogger, LogLevel logLevel, Times? times, string failMessage)
        {
            if (mockLogger == null)
                throw new ArgumentNullException(nameof(mockLogger));

            Expression<Func<LogEntry, bool>> matchingLogEntry = logEntry => logEntry.Level == logLevel;

            mockLogger.Verify(m => m.Log(It.Is(matchingLogEntry), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                times ?? Times.Once(), failMessage);
        }

        public static void VerifyDebug(this Mock<ILogger> mockLogger, string messagePattern,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, LogLevel.Debug, times, failMessage);

        public static void VerifyInfo(this Mock<ILogger> mockLogger, string messagePattern,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, LogLevel.Info, times, failMessage);

        public static void VerifyWarn(this Mock<ILogger> mockLogger, string messagePattern,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, LogLevel.Warn, times, failMessage);

        public static void VerifyError(this Mock<ILogger> mockLogger, string messagePattern,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, LogLevel.Error, times, failMessage);

        public static void VerifyFatal(this Mock<ILogger> mockLogger, string messagePattern,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, LogLevel.Fatal, times, failMessage);

        public static void VerifyAudit(this Mock<ILogger> mockLogger, string messagePattern,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, LogLevel.Audit, times, failMessage);

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

        public static void VerifyDebug(this Mock<ILogger> mockLogger, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(extendedProperties, LogLevel.Debug, times, failMessage);

        public static void VerifyInfo(this Mock<ILogger> mockLogger, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(extendedProperties, LogLevel.Info, times, failMessage);

        public static void VerifyWarn(this Mock<ILogger> mockLogger, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(extendedProperties, LogLevel.Warn, times, failMessage);

        public static void VerifyError(this Mock<ILogger> mockLogger, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(extendedProperties, LogLevel.Error, times, failMessage);

        public static void VerifyFatal(this Mock<ILogger> mockLogger, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(extendedProperties, LogLevel.Fatal, times, failMessage);

        public static void VerifyAudit(this Mock<ILogger> mockLogger, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(extendedProperties, LogLevel.Audit, times, failMessage);

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

                    switch (property.Value)
                    {
                        case string pattern:
                            if (!Regex.IsMatch((string)value, pattern))
                                return false;
                            break;
                        default:
                            if (!Equals(property.Value, value))
                                return false;
                            break;
                    }
                }
                return true;
            };

            Expression<Func<LogEntry, bool>> matchingLogEntry = logEntry =>
                (!logLevel.HasValue || logEntry.Level == logLevel.Value)
                && hasMatchingExtendedProperties(logEntry);

            mockLogger.Verify(m => m.Log(It.Is(matchingLogEntry), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                times ?? Times.Once(), failMessage);
        }

        public static void VerifyDebug(this Mock<ILogger> mockLogger, string messagePattern, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, extendedProperties, LogLevel.Debug, times, failMessage);

        public static void VerifyInfo(this Mock<ILogger> mockLogger, string messagePattern, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, extendedProperties, LogLevel.Info, times, failMessage);

        public static void VerifyWarn(this Mock<ILogger> mockLogger, string messagePattern, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, extendedProperties, LogLevel.Warn, times, failMessage);

        public static void VerifyError(this Mock<ILogger> mockLogger, string messagePattern, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, extendedProperties, LogLevel.Error, times, failMessage);

        public static void VerifyFatal(this Mock<ILogger> mockLogger, string messagePattern, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, extendedProperties, LogLevel.Fatal, times, failMessage);

        public static void VerifyAudit(this Mock<ILogger> mockLogger, string messagePattern, object extendedProperties,
            Times? times = null, string failMessage = null) =>
            mockLogger.VerifyLog(messagePattern, extendedProperties, LogLevel.Audit, times, failMessage);

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

                    switch (property.Value)
                    {
                        case string pattern:
                            if (!Regex.IsMatch((string)value, pattern))
                                return false;
                            break;
                        default:
                            if (!Equals(property.Value, value))
                                return false;
                            break;
                    }
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

        private static Dictionary<string, object> GetExtendedPropertiesDictionary(object extendedProperties)
        {
            var logEntry = new LogEntry();
            logEntry.SetExtendedProperties(extendedProperties);
            return logEntry.ExtendedProperties;
        }
    }
}
