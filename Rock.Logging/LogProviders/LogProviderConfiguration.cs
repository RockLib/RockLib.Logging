using System;
using System.Collections.Generic;

namespace Rock.Logging
{
    public class LogProviderConfiguration : ILogProviderConfiguration
    {
        public string FormatterName { get; set; }
        public Type ProviderType { get; set; }
        public IEnumerable<IMapper> Mappers { get; set; }
    }
}