using System;

namespace RockLib.Logging.DependencyInjection;

/// <summary>
/// Defines an options class for creating a <see cref="FileLogProvider"/>.
/// </summary>
public class FileLogProviderOptions : FormattableLogProviderOptions
{
    private string _file = string.Empty;

    /// <summary>
    /// The file to write to.
    /// </summary>
    public string File
    {
        get => _file;
        set => _file = value ?? throw new ArgumentNullException(nameof(value));
    }
}