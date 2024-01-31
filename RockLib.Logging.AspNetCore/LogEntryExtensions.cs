﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using RockLib.DistributedTracing.AspNetCore;
using System;
using System.Net;

using static RockLib.DistributedTracing.AspNetCore.HeaderNames;

namespace RockLib.Logging.AspNetCore;

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
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(logEntry);
#else
        if (logEntry is null) { throw new ArgumentNullException(nameof(logEntry)); }
#endif

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
#pragma warning disable CA1054 // URI-like parameters should not be strings
    public static LogEntry SetPath(this LogEntry logEntry, string? url)
#pragma warning restore CA1054 // URI-like parameters should not be strings
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(logEntry);
#else
        if (logEntry is null) { throw new ArgumentNullException(nameof(logEntry)); }
#endif

        if (!string.IsNullOrEmpty(url))
        {
            logEntry.ExtendedProperties["Path"] = url;
        }

        return logEntry;
    }

    /// <summary>
    /// Sets the request method to a <see cref="LogEntry"/>.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    /// <param name="requestMethod">The request method.</param>
    /// <returns>The same log entry.</returns>
    public static LogEntry SetRequestMethod(this LogEntry logEntry, string? requestMethod)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(logEntry);
#else
        if (logEntry is null) { throw new ArgumentNullException(nameof(logEntry)); }
#endif

        if (!string.IsNullOrEmpty(requestMethod))
        {
            logEntry.ExtendedProperties["Method"] = requestMethod!.ToUpperInvariant();
        }

        return logEntry;
    }

    /// <summary>
    /// Sets the user agent to a <see cref="LogEntry"/>.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    /// <param name="userAgent">The user agent.</param>
    /// <returns>The same log entry.</returns>
    public static LogEntry SetUserAgent(this LogEntry logEntry, string? userAgent)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(logEntry);
#else
        if (logEntry is null) { throw new ArgumentNullException(nameof(logEntry)); }
#endif

        if (!string.IsNullOrEmpty(userAgent))
        {
            logEntry.ExtendedProperties["UserAgent"] = userAgent;
        }

        return logEntry;
    }

    /// <summary>
    /// Sets the referrer to a <see cref="LogEntry"/>.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    /// <param name="referrer">The referrer.</param>
    /// <returns>The same log entry.</returns>
    public static LogEntry SetReferrer(this LogEntry logEntry, Uri? referrer) =>
        logEntry.SetReferrer(referrer?.ToString());

    /// <summary>
    /// Sets the referrer to a <see cref="LogEntry"/>.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    /// <param name="referrer">The referrer.</param>
    /// <returns>The same log entry.</returns>
    public static LogEntry SetReferrer(this LogEntry logEntry, string? referrer)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(logEntry);
#else
        if (logEntry is null) { throw new ArgumentNullException(nameof(logEntry)); }
#endif

        if (!string.IsNullOrEmpty(referrer))
        {
            logEntry.ExtendedProperties["Referrer"] = referrer;
        }

        return logEntry;
    }

    /// <summary>
    /// Sets the remote ip address to a <see cref="LogEntry"/>.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    /// <param name="remoteIpAddress">The remote ip address.</param>
    /// <returns>The same log entry.</returns>
    public static LogEntry SetRemoteIpAddress(this LogEntry logEntry, IPAddress? remoteIpAddress) =>
        logEntry.SetRemoteIpAddress(remoteIpAddress?.ToString());

    /// <summary>
    /// Sets the remote ip address to a <see cref="LogEntry"/>.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    /// <param name="remoteIpAddress">The remote ip address.</param>
    /// <returns>The same log entry.</returns>
    public static LogEntry SetRemoteIpAddress(this LogEntry logEntry, string? remoteIpAddress)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(logEntry);
#else
        if (logEntry is null) { throw new ArgumentNullException(nameof(logEntry)); }
#endif

        if (remoteIpAddress is not null)
        {
            logEntry.ExtendedProperties["RemoteIpAddress"] = remoteIpAddress;
        }

        return logEntry;
    }

    /// <summary>
    /// Sets the forwarded for value to a <see cref="LogEntry"/>.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    /// <param name="forwardedFor">The forwarded for value.</param>
    /// <returns>The same log entry.</returns>
    public static LogEntry SetForwardedFor(this LogEntry logEntry, StringValues forwardedFor) =>
        logEntry.SetForwardedFor(forwardedFor.ToArray()!);

    /// <summary>
    /// Sets the forwarded for value to a <see cref="LogEntry"/>.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    /// <param name="forwardedFor">The forwarded for value.</param>
    public static LogEntry SetForwardedFor(this LogEntry logEntry, string[] forwardedFor)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(logEntry);
#else
        if (logEntry is null) { throw new ArgumentNullException(nameof(logEntry)); }
#endif

        if (forwardedFor is not null)
        {
            if (forwardedFor.Length > 1)
            {
                logEntry.ExtendedProperties["X-Forwarded-For"] = forwardedFor;
            }
            else if (forwardedFor.Length == 1)
            {
                logEntry.ExtendedProperties["X-Forwarded-For"] = forwardedFor[0];
            }
        }

        return logEntry;
    }
}
