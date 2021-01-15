using Microsoft.AspNetCore.Http;

namespace RockLib.Logging.AspNetCore
{
    /// <summary>
    /// An implementation of <see cref="IContextProvider"/> used to add the path value to a <see cref="LogEntry"/>.
    /// </summary>
    public class PathContextProvider : IContextProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathContextProvider"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The http context accessor used to retreive the path value.</param>
        public PathContextProvider(IHttpContextAccessor httpContextAccessor)
            : this(httpContextAccessor?.HttpContext?.GetPath())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathContextProvider"/> class.
        /// </summary>
        /// <param name="path">The path value.</param>
        public PathContextProvider(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Gets the path value.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Add custom context to the <see cref="LogEntry"/> object.
        /// </summary>
        /// <param name="logEntry">The log entry to add custom context to.</param>
        public void AddContext(LogEntry logEntry)
        {
            logEntry.SetPath(Path);
        }
    }
}
