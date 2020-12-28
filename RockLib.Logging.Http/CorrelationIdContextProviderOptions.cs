using RockLib.Http;
using System;

namespace RockLib.Logging.Http
{
    /// <summary>
    /// Defines options for <see cref="CorrelationIdContextProvider"/>.
    /// </summary>
    public class CorrelationIdContextProviderOptions
    {
        private string _correlationIdHeader = HeaderNames.CorrelationId;

        /// <summary>
        /// The name of the correlation id header.
        /// </summary>
        public string CorrelationIdHeader
        {
            get => _correlationIdHeader;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));

                _correlationIdHeader = value;
            }
        }
    }
}
