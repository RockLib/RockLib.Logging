using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RockLib.DistributedTracing;
using RockLib.DistributedTracing.AspNetCore;
using System;
using static RockLib.DistributedTracing.AspNetCore.HeaderNames;

namespace RockLib.Logging.AspNetCore;

/// <summary>
/// An implementation of <see cref="IContextProvider"/> used to add a correlation id to a <see cref="LogEntry"/>.
/// </summary>
public class CorrelationIdContextProvider : IContextProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationIdContextProvider"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The http context accessor used to retreive the correlation id.</param>
    /// <param name="options">Options containing the name of the correlation id header.</param>
    public CorrelationIdContextProvider(IHttpContextAccessor httpContextAccessor,
        IOptionsMonitor<CorrelationIdContextProviderOptions>? options = null)
        : this(httpContextAccessor?.HttpContext?.GetCorrelationIdAccessor(options?.CurrentValue.CorrelationIdHeader ?? CorrelationId))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationIdContextProvider"/> class.
    /// </summary>
    /// <param name="accessor">The accessor used to retreive the correlation id.</param>
    public CorrelationIdContextProvider(ICorrelationIdAccessor? accessor) => Accessor = accessor;

    /// <summary>
    /// Gets the accessor used to retreive the correlation id.
    /// </summary>
    public ICorrelationIdAccessor? Accessor { get; }

    /// <summary>
    /// Add custom context to the <see cref="LogEntry"/> object.
    /// </summary>
    /// <param name="logEntry">The log entry to add custom context to.</param>
    public void AddContext(LogEntry logEntry)
    {
        if (logEntry is null) { throw new ArgumentNullException(nameof(logEntry)); }

        if (Accessor is not null)
        {
            logEntry.CorrelationId ??= Accessor.CorrelationId;
            // Otel alignment for aligning logs w/metrics & traces
            logEntry.ExtendedProperties.Add("TraceId", Accessor.GetTraceId());
            logEntry.ExtendedProperties.Add("SpanId", Accessor.GetSpanId());
        }
    }
}
