using System;
using System.Collections.Generic;

namespace Rock.Logging
{
    public interface ILogProviderConfiguration
    {
        string FormatterName { get; }
        Type ProviderType { get; }
        IEnumerable<IMapper> Mappers { get; }
    }
}