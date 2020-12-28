using Microsoft.AspNetCore.Http;
using RockLib.Http;
using System;

namespace RockLib.Logging.Http
{
    /// <summary>
    /// An implementation of <see cref="IContextProvider"/> used to add the referrer value to a <see cref="LogEntry"/>.
    /// </summary>
    public class ReferrerContextProvider : IContextProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferrerContextProvider"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The http context accessor used to retreive the referrer value.</param>
        public ReferrerContextProvider(IHttpContextAccessor httpContextAccessor)
            : this(httpContextAccessor?.HttpContext?.GetReferrer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferrerContextProvider"/> class.
        /// </summary>
        /// <param name="referrer">The referrer value.</param>
        public ReferrerContextProvider(string referrer)
            : this(referrer == null ? null : new Uri(referrer, UriKind.RelativeOrAbsolute))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferrerContextProvider"/> class.
        /// </summary>
        /// <param name="referrer">The referrer value.</param>
        public ReferrerContextProvider(Uri referrer)
        {
            Referrer = referrer;
        }

        /// <summary>
        /// Gets the referrer value.
        /// </summary>
        public Uri Referrer { get; }

        /// <summary>
        /// Add custom context to the <see cref="LogEntry"/> object.
        /// </summary>
        /// <param name="logEntry">The log entry to add custom context to.</param>
        public void AddContext(LogEntry logEntry)
        {
            logEntry.SetReferrer(Referrer);
        }
    }
}
