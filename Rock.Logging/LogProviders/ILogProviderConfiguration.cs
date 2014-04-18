using System;

namespace Rock.Logging
{
    public interface ILogProviderConfiguration
    {
        string FormatterName { get; }
        Type ProviderType { get; }
    }
}