using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RockLib.Logging.SafeLogging
{
    /// <summary>
    /// Extension for safely logging using the <see cref="SafeToLogAttribute"/> and <see cref="NotSafeToLogAttribute"/>.
    /// </summary>
    public static class SafeLoggingExtensions
    {
        /// <summary>
        /// Safely logs at the debug level.
        /// </summary>
        /// <param name="logger">The logger that performs the debug logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="correlationId">The ID used to corralate a transaction across many service calls for this log entry.</param>
        /// <param name="businessProcessId">The business process ID.</param>
        /// <param name="businessActivityId">The business activity ID.</param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Debug(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            string correlationId = null,
            string businessProcessId = null,
            string businessActivityId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) =>
            logger.LogSafe(LogLevel.Debug, message, null, extendedProperties, correlationId, businessProcessId,
                businessActivityId, callerMemberName, callerFilePath, callerLineNumber);

        /// <summary>
        /// Safely logs at the debug level.
        /// </summary>
        /// <param name="logger">The logger that performs the debug logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the current logging operation.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="correlationId">The ID used to corralate a transaction across many service calls for this log entry.</param>
        /// <param name="businessProcessId">The business process ID.</param>
        /// <param name="businessActivityId">The business activity ID.</param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Debug(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            string correlationId = null,
            string businessProcessId = null,
            string businessActivityId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) =>
            logger.LogSafe(LogLevel.Debug, message, exception, extendedProperties, correlationId, businessProcessId,
                businessActivityId, callerMemberName, callerFilePath, callerLineNumber);

        /// <summary>
        /// Safely logs at the info level.
        /// </summary>
        /// <param name="logger">The logger that performs the info logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="correlationId">The ID used to corralate a transaction across many service calls for this log entry.</param>
        /// <param name="businessProcessId">The business process ID.</param>
        /// <param name="businessActivityId">The business activity ID.</param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Info(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            string correlationId = null,
            string businessProcessId = null,
            string businessActivityId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) =>
            logger.LogSafe(LogLevel.Info, message, null, extendedProperties, correlationId, businessProcessId,
                businessActivityId, callerMemberName, callerFilePath, callerLineNumber);

        /// <summary>
        /// Safely logs at the info level.
        /// </summary>
        /// <param name="logger">The logger that performs the info logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the current logging operation.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="correlationId">The ID used to corralate a transaction across many service calls for this log entry.</param>
        /// <param name="businessProcessId">The business process ID.</param>
        /// <param name="businessActivityId">The business activity ID.</param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Info(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            string correlationId = null,
            string businessProcessId = null,
            string businessActivityId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) =>
            logger.LogSafe(LogLevel.Info, message, exception, extendedProperties, correlationId, businessProcessId,
                businessActivityId, callerMemberName, callerFilePath, callerLineNumber);

        /// <summary>
        /// Safely logs at the warn level.
        /// </summary>
        /// <param name="logger">The logger that performs the warn logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="correlationId">The ID used to corralate a transaction across many service calls for this log entry.</param>
        /// <param name="businessProcessId">The business process ID.</param>
        /// <param name="businessActivityId">The business activity ID.</param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Warn(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            string correlationId = null,
            string businessProcessId = null,
            string businessActivityId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) =>
            logger.LogSafe(LogLevel.Warn, message, null, extendedProperties, correlationId, businessProcessId,
                businessActivityId, callerMemberName, callerFilePath, callerLineNumber);

        /// <summary>
        /// Safely logs at the warn level.
        /// </summary>
        /// <param name="logger">The logger that performs the warn logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the current logging operation.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="correlationId">The ID used to corralate a transaction across many service calls for this log entry.</param>
        /// <param name="businessProcessId">The business process ID.</param>
        /// <param name="businessActivityId">The business activity ID.</param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Warn(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            string correlationId = null,
            string businessProcessId = null,
            string businessActivityId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) =>
            logger.LogSafe(LogLevel.Warn, message, exception, extendedProperties, correlationId, businessProcessId,
                businessActivityId, callerMemberName, callerFilePath, callerLineNumber);

        /// <summary>
        /// Safely logs at the error level.
        /// </summary>
        /// <param name="logger">The logger that performs the error logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="correlationId">The ID used to corralate a transaction across many service calls for this log entry.</param>
        /// <param name="businessProcessId">The business process ID.</param>
        /// <param name="businessActivityId">The business activity ID.</param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Error(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            string correlationId = null,
            string businessProcessId = null,
            string businessActivityId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) =>
            logger.LogSafe(LogLevel.Error, message, null, extendedProperties, correlationId, businessProcessId,
                businessActivityId, callerMemberName, callerFilePath, callerLineNumber);

        /// <summary>
        /// Safely logs at the error level.
        /// </summary>
        /// <param name="logger">The logger that performs the error logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the current logging operation.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="correlationId">The ID used to corralate a transaction across many service calls for this log entry.</param>
        /// <param name="businessProcessId">The business process ID.</param>
        /// <param name="businessActivityId">The business activity ID.</param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Error(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            string correlationId = null,
            string businessProcessId = null,
            string businessActivityId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) =>
            logger.LogSafe(LogLevel.Error, message, exception, extendedProperties, correlationId, businessProcessId,
                businessActivityId, callerMemberName, callerFilePath, callerLineNumber);

        /// <summary>
        /// Safely logs at the fatal level.
        /// </summary>
        /// <param name="logger">The logger that performs the fatal logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="correlationId">The ID used to corralate a transaction across many service calls for this log entry.</param>
        /// <param name="businessProcessId">The business process ID.</param>
        /// <param name="businessActivityId">The business activity ID.</param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Fatal(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            string correlationId = null,
            string businessProcessId = null,
            string businessActivityId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) =>
            logger.LogSafe(LogLevel.Fatal, message, null, extendedProperties, correlationId, businessProcessId,
                businessActivityId, callerMemberName, callerFilePath, callerLineNumber);

        /// <summary>
        /// Safely logs at the fatal level.
        /// </summary>
        /// <param name="logger">The logger that performs the fatal logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the current logging operation.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="correlationId">The ID used to corralate a transaction across many service calls for this log entry.</param>
        /// <param name="businessProcessId">The business process ID.</param>
        /// <param name="businessActivityId">The business activity ID.</param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Fatal(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            string correlationId = null,
            string businessProcessId = null,
            string businessActivityId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) =>
            logger.LogSafe(LogLevel.Fatal, message, exception, extendedProperties, correlationId, businessProcessId,
                businessActivityId, callerMemberName, callerFilePath, callerLineNumber);

        /// <summary>
        /// Safely logs at the audit level.
        /// </summary>
        /// <param name="logger">The logger that performs the audit logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="correlationId">The ID used to corralate a transaction across many service calls for this log entry.</param>
        /// <param name="businessProcessId">The business process ID.</param>
        /// <param name="businessActivityId">The business activity ID.</param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Audit(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            string correlationId = null,
            string businessProcessId = null,
            string businessActivityId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) =>
            logger.LogSafe(LogLevel.Audit, message, null, extendedProperties, correlationId, businessProcessId,
                businessActivityId, callerMemberName, callerFilePath, callerLineNumber);

        /// <summary>
        /// Safely logs at the audit level.
        /// </summary>
        /// <param name="logger">The logger that performs the audit logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the current logging operation.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="correlationId">The ID used to corralate a transaction across many service calls for this log entry.</param>
        /// <param name="businessProcessId">The business process ID.</param>
        /// <param name="businessActivityId">The business activity ID.</param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Audit(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            string correlationId = null,
            string businessProcessId = null,
            string businessActivityId = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) =>
            logger.LogSafe(LogLevel.Audit, message, exception, extendedProperties, correlationId, businessProcessId,
                businessActivityId, callerMemberName, callerFilePath, callerLineNumber);

        private static void LogSafe(this ILogger logger,
            LogLevel level,
            string message,
            Exception exception,
            object extendedProperties,
            string correlationId,
            string businessProcessId,
            string businessActivityId,
            string callerMemberName,
            string callerFilePath,
            int callerLineNumber)
        {
            if (logger.IsDisabled || level < logger.Level)
                return;

            var logEntry = new LogEntry(message, exception, level)
            {
                CorrelationId = correlationId,
                BusinessProcessId = businessProcessId,
                BusinessActivityId = businessActivityId
            };
            logEntry.SetSafeExtendedProperties(extendedProperties);

            logger.Log(logEntry, callerMemberName, callerFilePath, callerLineNumber);
        }
    }
}
