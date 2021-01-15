using Microsoft.AspNetCore.Http;

namespace RockLib.Logging.AspNetCore
{
    /// <summary>
    /// An implementation of <see cref="IContextProvider"/> used to add the UserAgent value to a <see cref="LogEntry"/>.
    /// </summary>
    public class UserAgentContextProvider : IContextProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMethodContextProvider"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The http context accessor used to retreive the UserAgent value.</param>
        public UserAgentContextProvider(IHttpContextAccessor httpContextAccessor)
            : this(httpContextAccessor?.HttpContext?.GetUserAgent())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMethodContextProvider"/> class.
        /// </summary>
        /// <param name="userAgent">The UserAgent value.</param>
        public UserAgentContextProvider(string userAgent)
        {
            UserAgent = userAgent;
        }

        /// <summary>
        /// Gets the UserAgent value.
        /// </summary>
        public string UserAgent { get; }

        /// <summary>
        /// Add custom context to the <see cref="LogEntry"/> object.
        /// </summary>
        /// <param name="logEntry">The log entry to add custom context to.</param>
        public void AddContext(LogEntry logEntry)
        {
            logEntry.SetUserAgent(UserAgent);
        }
    }
}
