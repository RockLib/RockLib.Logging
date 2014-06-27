using System;

namespace Rock.Logging
{
    public static partial class LoggerExtensions
    {
        public static IExceptionHandler GetExceptionHandler(this ILogger logger, LogLevel logLevel = LogLevel.Error)
        {
            return new LoggerExceptionHandler(logger, logLevel);
        }

        private class LoggerExceptionHandler : IExceptionHandler
        {
            private readonly ILogger _logger;
            private readonly LogLevel _logLevel;

            public LoggerExceptionHandler(ILogger logger, LogLevel logLevel)
            {
                _logger = logger;
                _logLevel = logLevel;
            }

            public void HandleException(Exception ex)
            {
                _logger.Log(_logLevel, ex);
            }
        }
    }
}
