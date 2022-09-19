using System;

using static RockLib.DistributedTracing.AspNetCore.HeaderNames;

namespace RockLib.Logging.AspNetCore;

/// <summary>
/// Defines options for <see cref="CorrelationIdContextProvider"/>.
/// </summary>
public class CorrelationIdContextProviderOptions
{
    private string _correlationIdHeader = CorrelationId;

    /// <summary>
    /// The name of the correlation id header.
    /// </summary>
    public string CorrelationIdHeader
    {
        get => _correlationIdHeader;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            _correlationIdHeader = value;
        }
    }
}
