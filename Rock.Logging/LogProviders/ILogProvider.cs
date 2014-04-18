using System.Threading.Tasks;

namespace Rock.Logging
{
    /// <summary>
    /// This interface defines the contract for logging providers.  
    /// Any custom logging provider that is created will need to implement this interface 
    /// as well as the optional interface <see cref="Rock.Framework.Formatter.IFormatterTemplate"/> if the provider 
    /// is able to format messages for output.
    /// </summary>
    public interface ILogProvider
    {
        /// <summary>
        /// Writes the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        Task Write(LogEntry entry);
    }
}