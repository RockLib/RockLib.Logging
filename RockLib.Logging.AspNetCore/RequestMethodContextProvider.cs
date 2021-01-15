using Microsoft.AspNetCore.Http;

namespace RockLib.Logging.AspNetCore
{
    /// <summary>
    /// An implementation of <see cref="IContextProvider"/> used to add the request method value to a <see cref="LogEntry"/>.
    /// </summary>
    public class RequestMethodContextProvider : IContextProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMethodContextProvider"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The http context accessor used to retreive the request method value.</param>
        public RequestMethodContextProvider(IHttpContextAccessor httpContextAccessor)
            : this(httpContextAccessor?.HttpContext?.GetMethod())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMethodContextProvider"/> class.
        /// </summary>
        /// <param name="requestMethod">The request method value.</param>
        public RequestMethodContextProvider(string requestMethod)
        {
            RequestMethod = requestMethod;
        }

        /// <summary>
        /// Gets the request method value.
        /// </summary>
        public string RequestMethod { get; }

        /// <summary>
        /// Add custom context to the <see cref="LogEntry"/> object.
        /// </summary>
        /// <param name="logEntry">The log entry to add custom context to.</param>
        public void AddContext(LogEntry logEntry)
        {
            logEntry.SetRequestMethod(RequestMethod);
        }
    }
}
