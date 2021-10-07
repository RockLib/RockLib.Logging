using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace RockLib.Logging.Moq
{
    partial class MockLoggerExtensions
    {
        private static void VerifyLog(this Mock<ILogger> mockLogger, string message,
            Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
            LogLevel? logLevel, Times? times, string failMessage)
        {
            if (mockLogger == null)
                throw new ArgumentNullException(nameof(mockLogger));

            Expression<Func<LogEntry, bool>> matchingMessage = null, matchingExtendedProperties = null, matchingLogLevel = null;

            if (message != null)
                matchingMessage = GetMatchingMessageExpression(message);

            if (extendedProperties != null)
                matchingExtendedProperties = GetMatchingExtendedPropertiesExpression(extendedProperties);

            if (logLevel != null)
                matchingLogLevel = GetMatchingLogLevelExpression(logLevel.Value);

            var matchingLogEntry = GetMatchingLogEntryExpression(matchingMessage, matchingLogLevel, hasMatchingException, matchingExtendedProperties);

            mockLogger.Verify(mock => mock.Log(It.Is(matchingLogEntry), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                times ?? Times.Once(), failMessage);
        }

        private static Expression<Func<LogEntry, bool>> GetMatchingMessageExpression(string message)
        {
            var logEntryParameter = Expression.Parameter(typeof(LogEntry), "logEntry");
            Expression body;

            if (message.StartsWith("/") && message.EndsWith("/"))
            {
                var isMatchMethod = typeof(Regex).GetMethod(nameof(Regex.IsMatch), new Type[] { typeof(string), typeof(string) });
                body = Expression.Call(isMatchMethod,
                    Expression.Property(logEntryParameter, nameof(LogEntry.Message)),
                    Expression.Constant(message.Substring(1, message.Length - 2)));
            }            
            else
                body = Expression.Equal(Expression.Property(logEntryParameter, nameof(LogEntry.Message)), Expression.Constant(message));

            return Expression.Lambda<Func<LogEntry, bool>>(body, logEntryParameter);
        }

        private static Expression<Func<LogEntry, bool>> GetMatchingExtendedPropertiesExpression(object extendedProperties)
        {
            Expression<Func<LogEntry, bool>> hasMatchingExtendedProperties = null;

            var properties = GetExtendedPropertiesDictionary(extendedProperties);
            foreach (var property in properties.Reverse())
            {
                var logEntryParameter = Expression.Parameter(typeof(LogEntry), "logEntry");

                var matcher = new ExtendedPropertyMatcher();
                var body = Expression.Invoke(
                    Expression.Field(Expression.Constant(matcher), nameof(matcher.HasMatchingExtendedProperty)),
                    logEntryParameter, Expression.Constant(property.Key), Expression.Constant(property.Value, typeof(object)));

                var hasMatchingProperty = Expression.Lambda<Func<LogEntry, bool>>(body, logEntryParameter);

                if (hasMatchingExtendedProperties is null)
                    hasMatchingExtendedProperties = hasMatchingProperty;
                else
                    hasMatchingExtendedProperties = hasMatchingProperty.And(hasMatchingExtendedProperties);
            }

            return hasMatchingExtendedProperties;
        }

        private static Expression<Func<LogEntry, bool>> GetMatchingLogLevelExpression(LogLevel logLevel)
        {
            var logEntryParameter = Expression.Parameter(typeof(LogEntry), "logEntry");
            var body = Expression.Equal(Expression.Property(logEntryParameter, nameof(LogEntry.Level)), Expression.Constant(logLevel));
            return Expression.Lambda<Func<LogEntry, bool>>(body, logEntryParameter);
        }

        private static Expression<Func<LogEntry, bool>> GetMatchingLogEntryExpression(Expression<Func<LogEntry, bool>> hasMatchingMessage,
            Expression<Func<LogEntry, bool>> hasMatchingLogLevel, Expression<Func<Exception, bool>> hasMatchingException,
            Expression<Func<LogEntry, bool>> hasMatchingExtendedProperties)
        {
            Expression<Func<LogEntry, bool>> matchingLogEntry = null;

            if (hasMatchingExtendedProperties != null)
                matchingLogEntry = hasMatchingExtendedProperties;
            if (hasMatchingException != null)
                matchingLogEntry = hasMatchingException.And(matchingLogEntry);
            if (hasMatchingLogLevel != null)
                matchingLogEntry = hasMatchingLogLevel.And(matchingLogEntry);
            if (hasMatchingMessage != null)
                matchingLogEntry = hasMatchingMessage.And(matchingLogEntry);

            if (matchingLogEntry is null)
                matchingLogEntry = logEntry => true;

            return matchingLogEntry;
        }

        private static Expression<Func<LogEntry, bool>> And(this Expression<Func<LogEntry, bool>> lhs, Expression<Func<LogEntry, bool>> rhs)
        {
            if (rhs is null)
                return lhs;

            var logEntryParameter = Expression.Parameter(typeof(LogEntry), "logEntry");

            var body = Expression.And(LambdaHelper.RebindBody(lhs, logEntryParameter), LambdaHelper.RebindBody(rhs, logEntryParameter));

            return Expression.Lambda<Func<LogEntry, bool>>(body, logEntryParameter);
        }

        private static Expression<Func<LogEntry, bool>> And(this Expression<Func<Exception, bool>> lhs, Expression<Func<LogEntry, bool>> rhs)
        {
            var logEntryParameter = Expression.Parameter(typeof(LogEntry), "logEntry");
            var logEntryException = Expression.Property(logEntryParameter, nameof(LogEntry.Exception));

            Expression body;
            if (rhs is null)
                body = LambdaHelper.RebindBody(lhs, logEntryException);
            else
                body = Expression.And(LambdaHelper.RebindBody(lhs, logEntryException), LambdaHelper.RebindBody(rhs, logEntryParameter));

            return Expression.Lambda<Func<LogEntry, bool>>(body, logEntryParameter);
        }

        private static bool AreEquivalent(object lhs, object rhs)
        {
            switch (lhs)
            {
                case string pattern:
                    if (pattern.StartsWith("/") && pattern.EndsWith("/"))
                        return Regex.IsMatch((string)rhs, pattern.Substring(1, pattern.Length - 2));
                    return (string)rhs == pattern;
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

        // Pretend to be compiler generated so that Moq thinks it's dealing with closure
        // access when formatting an expression that has failed verification so that it
        // omits the "closure" class. TL;DR: This attribute makes things format nicely.
        [CompilerGenerated]
        private class ExtendedPropertyMatcher
        {
            public readonly Func<LogEntry, string, object, bool> HasMatchingExtendedProperty = (logEntry, extendedPropertyName, expectedValue) =>
            {
                if (!logEntry.ExtendedProperties.TryGetValue(extendedPropertyName, out var value)
                        || !value.GetType().IsAssignableFrom(expectedValue.GetType()))
                    return false;

                if (!AreEquivalent(expectedValue, value))
                    return false;

                return true;
            };
        }

        private class LambdaHelper : ExpressionVisitor
        {
            private readonly ParameterExpression _parameterToReplace;
            private readonly Expression _parameterReplacement;

            private LambdaHelper(ParameterExpression parameterToReplace, Expression parameterReplacement)
            {
                _parameterToReplace = parameterToReplace;
                _parameterReplacement = parameterReplacement;
            }

            public static Expression RebindBody<T, TResult>(Expression<Func<T, TResult>> lambdaExpression, Expression parameterReplacement)
            {
                return new LambdaHelper(lambdaExpression.Parameters[0], parameterReplacement).Visit(lambdaExpression.Body);
            }

            protected override Expression VisitParameter(ParameterExpression parameter)
            {
                if (parameter.Equals(_parameterToReplace))
                    return _parameterReplacement;

                return base.VisitParameter(parameter);
            }
        }
    }
}
