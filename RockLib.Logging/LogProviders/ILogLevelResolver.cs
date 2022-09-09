using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockLib.Logging.LogProviders;

/// <summary>
/// Used to resolve the <see cref="LogLevel"/> when logging with an <see cref="ILogger"/>
/// </summary>
public interface ILogLevelResolver
{
    /// <summary>
    /// Retrieves the currently active <see cref="LogLevel"/>
    /// </summary>
    /// <returns>The current <see cref="LogLevel"/> or null if it cannot be determined</returns>
    /// <remarks>If the <see cref="LogLevel"/> cannot be determined, and is returned as null, then the default <see cref="LogLevel"/> 
    /// of the <see cref="ILogger"/> will be used instead</remarks>
    LogLevel? GetLogLevel();
}
