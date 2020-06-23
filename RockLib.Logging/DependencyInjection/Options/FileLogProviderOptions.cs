#if !NET451
using System;

namespace RockLib.Logging.DependencyInjection
{
    /// <summary>
    /// Defines an options class for creating a <see cref="FileLogProvider"/>.
    /// </summary>
    public class FileLogProviderOptions : FormattableLogProviderOptions
    {
        private string _file;

        /// <summary>
        /// The file to write to.
        /// </summary>
        public string File
        {
            get => _file;
            set => _file = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Whether to create a log provider that automatically reloads itself when its options change.
        /// </summary>
        public bool ReloadOnChange { get; set; }
    }
}
#endif
