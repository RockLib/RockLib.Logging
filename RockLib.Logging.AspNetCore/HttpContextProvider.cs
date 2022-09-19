using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace RockLib.Logging.AspNetCore;

/// <summary>
/// An implementation of <see cref="IContextProvider"/> used to add various http context values to a <see cref="LogEntry"/>.
/// </summary>
public class HttpContextProvider : IContextProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpContextProvider"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The http context accessor used to retreive the various context values.</param>
    /// <param name="correlationIdOptions">Options for the <see cref="CorrelationIdContextProvider"/>.</param>
    public HttpContextProvider(IHttpContextAccessor httpContextAccessor,
        IOptionsMonitor<CorrelationIdContextProviderOptions>? correlationIdOptions = null)
        : this(new IContextProvider[]
        {
            new RequestMethodContextProvider(httpContextAccessor),
            new PathContextProvider(httpContextAccessor),
            new UserAgentContextProvider(httpContextAccessor),
            new ReferrerContextProvider(httpContextAccessor),
            new RemoteIpAddressContextProvider(httpContextAccessor),
            new ForwardedForContextProvider(httpContextAccessor),
            new CorrelationIdContextProvider(httpContextAccessor, correlationIdOptions)
        })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpContextProvider"/> class.
    /// </summary>
    /// <param name="contextProviders">The set of providers used to retreive the various context values.</param>
    protected HttpContextProvider(IReadOnlyCollection<IContextProvider> contextProviders) => ContextProviders = contextProviders;

    /// <summary>
    /// Gets the set of providers used to retrieve the context values.
    /// </summary>
    public IReadOnlyCollection<IContextProvider> ContextProviders { get; }

    /// <summary>
    /// Add custom context to the <see cref="LogEntry"/> object.
    /// </summary>
    /// <param name="logEntry">The log entry to add custom context to.</param>
    public void AddContext(LogEntry logEntry)
    {
        foreach (var contextProvider in ContextProviders)
        {
            contextProvider.AddContext(logEntry);
        }
    }
}
