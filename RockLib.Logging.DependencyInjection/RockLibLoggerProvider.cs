using Microsoft.Extensions.Logging;
using System;

namespace RockLib.Logging.DependencyInjection
{
    /// <summary>
    /// Represents a type that can create instances of <see cref="RockLibLogger"/>.
    /// </summary>
    [Obsolete("Please use the RockLib.Logging.RockLibLoggerProvider class from the RockLib.Logging.Microsoft.Extensions package.")]
    public class RockLibLoggerProvider : ILoggerProvider
    {
        private readonly Func<ILogger> _getLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RockLibLoggerProvider"/> class.
        /// </summary>
        /// <param name="getLogger">
        /// A delegate that returns instances of <see cref="ILogger"/>. Invoked from the
        /// <see cref="CreateLogger"/> method.
        /// </param>
        public RockLibLoggerProvider(Func<ILogger> getLogger) =>
            _getLogger = getLogger ?? throw new ArgumentNullException(nameof(getLogger));

        /// <summary>
        /// Creates a new <see cref="RockLibLogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName) =>
            new RockLibLogger(_getLogger.Invoke(), categoryName);

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Dispose() { }
    }
}
