using Microsoft.AspNetCore.Http;
using System.Net;

namespace RockLib.Logging.AspNetCore;

/// <summary>
/// An implementation of <see cref="IContextProvider"/> used to add the remote ip address value to a <see cref="LogEntry"/>.
/// </summary>
public class RemoteIpAddressContextProvider : IContextProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReferrerContextProvider"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The http context accessor used to retreive the remote ip address value.</param>
    public RemoteIpAddressContextProvider(IHttpContextAccessor httpContextAccessor)
        : this(httpContextAccessor?.HttpContext?.GetRemoteIpAddress())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferrerContextProvider"/> class.
    /// </summary>
    /// <param name="remoteIpAddress">The remote ip address value.</param>
    public RemoteIpAddressContextProvider(string remoteIpAddress)
        : this(remoteIpAddress is null ? null : IPAddress.Parse(remoteIpAddress))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferrerContextProvider"/> class.
    /// </summary>
    /// <param name="remoteIpAddress">The remote ip address value.</param>
    public RemoteIpAddressContextProvider(IPAddress? remoteIpAddress) => RemoteIpAddress = remoteIpAddress;

    /// <summary>
    /// Gets the remote ip address value.
    /// </summary>
    public IPAddress? RemoteIpAddress { get; }

    /// <summary>
    /// Add custom context to the <see cref="LogEntry"/> object.
    /// </summary>
    /// <param name="logEntry">The log entry to add custom context to.</param>
    public void AddContext(LogEntry logEntry) => logEntry.SetRemoteIpAddress(RemoteIpAddress);
}
