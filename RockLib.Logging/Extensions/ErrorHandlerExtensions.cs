using System;
using System.Linq.Expressions;

namespace RockLib.Logging;

/// <summary>
/// Defines extension methods for the <see cref="ILogger"/> interface for setting
/// its <see cref="ILogger.ErrorHandler"/> property.
/// </summary>
public static class ErrorHandlerExtensions
{
    /// <summary>
    /// Sets the <see cref="ILogger.ErrorHandler"/> property to an implementation of
    /// the <see cref="IErrorHandler"/> interface that invokes the <paramref name="errorHandler"/>
    /// delegate when its <see cref="IErrorHandler.HandleError"/> method is called.
    /// </summary>
    /// <param name="logger">
    /// The logger whose <see cref="ILogger.ErrorHandler"/> property is to be set.
    /// </param>
    /// <param name="errorHandler">
    /// The delegate to be invoked when the <see cref="IErrorHandler.HandleError"/> method
    /// of the logger's <see cref="ILogger.ErrorHandler"/> property is called.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is <c>null</c></exception>
    public static void SetErrorHandler(this ILogger logger, Action<Error> errorHandler)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(logger);
#else
        if (logger is null) { throw new ArgumentNullException(nameof(logger)); }
#endif
        logger.ErrorHandler = new DelegateErrorHandler(errorHandler);
    }

    private sealed class DelegateErrorHandler : IErrorHandler
    {
        private readonly Action<Error> _handleError;
        public DelegateErrorHandler(Action<Error> handleError) => _handleError = handleError;
        public void HandleError(Error error) => _handleError(error);
    }
}
