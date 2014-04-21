using System;

namespace Rock.Logging.Configuration
{
    /// <summary>
    /// Provides access to an instance of <see cref="IConfigProvider"/> that is
    /// globally configured for an application.
    /// </summary>
    public static class ConfigProvider
    {
        private static readonly Lazy<IConfigProvider> _default;
        private static Lazy<IConfigProvider> _current;

        static ConfigProvider()
        {
            _default = new Lazy<IConfigProvider>(() => new FileConfigProvider());
            _current = _default;
        }

        /// <summary>
        /// Gets or sets the globally configured instance of <see cref="IConfigProvider"/>.
        /// </summary>
        public static IConfigProvider Current
        {
            get { return _current.Value; }
            set
            {
                if (value == null)
                {
                    _current = _default;
                }
                else if (!CurrentConfigProviderIsSameAs(value))
                {
                    _current = new Lazy<IConfigProvider>(() => value);
                }
            }
        }

        private static bool CurrentConfigProviderIsSameAs(IConfigProvider value)
        {
            return _current.IsValueCreated && ReferenceEquals(_current.Value, value);
        }
    }
}