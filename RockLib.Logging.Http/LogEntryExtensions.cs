using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using RockLib.Http;
using System;
using System.Net;

namespace RockLib.Logging.Http
{
    using static HeaderNames;
 
    /// <summary>
    /// Extensions for <see cref="LogEntry"/>.
    /// </summary>
    public static class LogEntryExtensions
    {
        /// <summary>
        /// Sets all of the http context values to a <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="httpContext">The http context.</param>
        /// <param name="correlationIdHeader">The name of the correlation id header.</param>
        public static void SetHttpContext(this LogEntry logEntry, HttpContext httpContext, string correlationIdHeader = CorrelationId)
        {
            if (logEntry is null)
                throw new ArgumentNullException(nameof(logEntry));

            logEntry.SetRequestMethod(httpContext.GetMethod())
                .SetPath(httpContext.GetPath())
                .SetUserAgent(httpContext.GetUserAgent())
                .SetReferrer(httpContext.GetReferrer())
                .SetRemoteIpAddress(httpContext.GetRemoteIpAddress())
                .SetForwardedFor(httpContext.GetForwardedFor());

            logEntry.CorrelationId = httpContext.GetCorrelationId(correlationIdHeader);
        }

        /// <summary>
        /// Sets the path to a <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="url">The path url.</param>
        /// <returns>The same log entry.</returns>
        public static LogEntry SetPath(this LogEntry logEntry, string url)
        {
            if (logEntry is null)
                throw new ArgumentNullException(nameof(logEntry));

            if (!string.IsNullOrEmpty(url))
                logEntry.ExtendedProperties["Path"] = url;

            return logEntry;
        }

        /// <summary>
        /// Sets the request method to a <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="requestMethod">The request method.</param>
        /// <returns>The same log entry.</returns>
        public static LogEntry SetRequestMethod(this LogEntry logEntry, string requestMethod)
        {
            if (logEntry is null)
                throw new ArgumentNullException(nameof(logEntry));

            if (!string.IsNullOrEmpty(requestMethod))
                logEntry.ExtendedProperties["Method"] = requestMethod.ToUpperInvariant();

            return logEntry;
        }

        /// <summary>
        /// Sets the user agent to a <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <returns>The same log entry.</returns>
        public static LogEntry SetUserAgent(this LogEntry logEntry, string userAgent)
        {
            if (logEntry is null)
                throw new ArgumentNullException(nameof(logEntry));

            if (!string.IsNullOrEmpty(userAgent))
                logEntry.ExtendedProperties["UserAgent"] = userAgent;

            return logEntry;
        }

        /// <summary>
        /// Sets the referrer to a <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>The same log entry.</returns>
        public static LogEntry SetReferrer(this LogEntry logEntry, Uri referrer) =>
            logEntry.SetReferrer(referrer?.ToString());

        /// <summary>
        /// Sets the referrer to a <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="referrer">The referrer.</param>
        /// <returns>The same log entry.</returns>
        public static LogEntry SetReferrer(this LogEntry logEntry, string referrer)
        {
            if (logEntry is null)
                throw new ArgumentNullException(nameof(logEntry));

            if (!string.IsNullOrEmpty(referrer))
                logEntry.ExtendedProperties["Referrer"] = referrer;

            return logEntry;
        }

        /// <summary>
        /// Sets the remote ip address to a <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="remoteIpAddress">The remote ip address.</param>
        /// <returns>The same log entry.</returns>
        public static LogEntry SetRemoteIpAddress(this LogEntry logEntry, IPAddress remoteIpAddress) =>
            logEntry.SetRemoteIpAddress(remoteIpAddress?.ToString());

        /// <summary>
        /// Sets the remote ip address to a <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="remoteIpAddress">The remote ip address.</param>
        /// <returns>The same log entry.</returns>
        public static LogEntry SetRemoteIpAddress(this LogEntry logEntry, string remoteIpAddress)
        {
            if (logEntry is null)
                throw new ArgumentNullException(nameof(logEntry));

            if (remoteIpAddress != null)
                logEntry.ExtendedProperties["RemoteIpAddress"] = remoteIpAddress;

            return logEntry;
        }

        /// <summary>
        /// Sets the forwarded for value to a <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="forwardedFor">The forwarded for value.</param>
        /// <returns>The same log entry.</returns>
        public static LogEntry SetForwardedFor(this LogEntry logEntry, StringValues forwardedFor) =>
            logEntry.SetForwardedFor(forwardedFor.ToArray());

        /// <summary>
        /// Sets the forwarded for value to a <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="forwardedFor">The forwarded for value.</param>
        public static LogEntry SetForwardedFor(this LogEntry logEntry, string[] forwardedFor)
        {
            if (logEntry is null)
                throw new ArgumentNullException(nameof(logEntry));

            if (forwardedFor != null)
            {
                if (forwardedFor.Length > 1)
                    logEntry.ExtendedProperties["X-Forwarded-For"] = forwardedFor;
                else if (forwardedFor.Length == 1)
                    logEntry.ExtendedProperties["X-Forwarded-For"] = forwardedFor[0];
            }

            return logEntry;
        }
    }
}
