using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockLib.Logging.LogProviders
{
    /// <summary>
    /// Used to resolve the <see cref="LogLevel"/> when logging with an <see cref="ILogger"/>
    /// </summary>
    public interface ILogLevelResolver
    {
        /// <summary>
        /// Retrieves the currently active <see cref="LogLevel"/>
        /// </summary>
        /// <returns>The current <see cref="LogLevel"/></returns>
        LogLevel GetLogLevel();
    }
}
