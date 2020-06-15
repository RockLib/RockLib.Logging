#if !NET451
using System;

namespace RockLib.Logging.DependencyInjection
{
    public class FileLogProviderOptions : FormattableLogProviderOptions
    {
        private string _file;

        public string File
        {
            get => _file;
            set => _file = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
#endif
