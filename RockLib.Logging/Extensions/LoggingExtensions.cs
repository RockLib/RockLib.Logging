using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RockLib.Logging
{
    /// <summary>
    /// Defines extension method that send logs at a given logging level.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Logs at the debug level.
        /// </summary>
        /// <param name="logger">The logger that performs the debug logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Debug(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Debug, message, null, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs at the debug level.
        /// </summary>
        /// <param name="logger">The logger that performs the debug logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the current logging operation.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Debug(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Debug, message, exception, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs at the info level.
        /// </summary>
        /// <param name="logger">The logger that performs the info logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Info(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Info, message, null, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs at the info level.
        /// </summary>
        /// <param name="logger">The logger that performs the info logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the current logging operation.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Info(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Info, message, exception, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs at the warn level.
        /// </summary>
        /// <param name="logger">The logger that performs the warn logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Warn(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Warn, message, null, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs at the warn level.
        /// </summary>
        /// <param name="logger">The logger that performs the warn logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the current logging operation.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Warn(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Warn, message, exception, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs at the error level.
        /// </summary>
        /// <param name="logger">The logger that performs the error logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Error(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Error, message, null, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs at the error level.
        /// </summary>
        /// <param name="logger">The logger that performs the error logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the current logging operation.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Error(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Error, message, exception, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs at the fatal level.
        /// </summary>
        /// <param name="logger">The logger that performs the fatal logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Fatal(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Fatal, message, null, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs at the fatal level.
        /// </summary>
        /// <param name="logger">The logger that performs the fatal logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the current logging operation.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Fatal(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Fatal, message, exception, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs at the audit level.
        /// </summary>
        /// <param name="logger">The logger that performs the audit logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Audit(
            this ILogger logger,
            string message,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Log(LogLevel.Audit, message, null, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Logs at the audit level.
        /// </summary>
        /// <param name="logger">The logger that performs the audit logging operation.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the current logging operation.</param>
        /// <param name="extendedProperties">
        /// An object whose properties are added to a <see cref="LogEntry.ExtendedProperties"/> dictionary.
        /// If this object is an <see cref="IDictionary{TKey, TValue}"/> with a string key, then each of
        /// its items are added to the <see cref="LogEntry.ExtendedProperties"/>.
        /// </param>
        /// <param name="callerMemberName">The method or property name of the caller.</param>
        /// <param name="callerFilePath">The path of the source file that contains the caller.</param>
        /// <param name="callerLineNumber">The line number in the source file at which this method is called.</param>
        public static void Audit(this ILogger logger,
            string message,
            Exception exception,
            object extendedProperties = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) =>
            logger.Log(LogLevel.Audit, message, exception, extendedProperties, callerMemberName, callerFilePath, callerLineNumber);

        private static void Log(this ILogger logger,
            LogLevel level,
            string message,
            Exception exception,
            object extendedProperties,
            string callerMemberName,
            string callerFilePath,
            int callerLineNumber)
        {
            if (logger.IsDisabled || level < logger.Level)
                return;

            var logEntry = new LogEntry(message, exception, level, extendedProperties);
            logger.Log(logEntry, callerMemberName, callerFilePath, callerLineNumber);
        }
    }
}
