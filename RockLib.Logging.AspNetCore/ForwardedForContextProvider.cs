using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace RockLib.Logging.AspNetCore
{
    /// <summary>
    /// An implementation of <see cref="IContextProvider"/> used to add the ForwardedFor value to a <see cref="LogEntry"/>.
    /// </summary>
    public class ForwardedForContextProvider : IContextProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardedForContextProvider"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The http context accessor used to retreive the ForwardedFor value.</param>
        public ForwardedForContextProvider(IHttpContextAccessor httpContextAccessor)
            : this(httpContextAccessor?.HttpContext?.GetForwardedFor() ?? default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardedForContextProvider"/> class.
        /// </summary>
        /// <param name="forwardedFor">The FowardedFor value.</param>
        public ForwardedForContextProvider(StringValues forwardedFor)
        {
            ForwardedFor = forwardedFor;
        }

        /// <summary>
        /// Gets the ForwardedFor value.
        /// </summary>
        public StringValues ForwardedFor { get; }

        /// <summary>
        /// Add custom context to the <see cref="LogEntry"/> object.
        /// </summary>
        /// <param name="logEntry">The log entry to add custom context to.</param>
        public void AddContext(LogEntry logEntry)
        {
            logEntry.SetForwardedFor(ForwardedFor);
        }
    }
}
