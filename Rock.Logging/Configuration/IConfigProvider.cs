namespace Rock.Logging.Configuration
{
    /// <summary>
    /// Loads configuration from a specific provider.
    /// <para>
    /// <see cref="Logger"/> can be configured to read it's configuration from  
    /// different sources. The default configuration provider is <see cref="FileConfigProvider"/>. It used 
    /// is to read configuration information from the web.config or app.config.
    /// </para>
    /// </summary>
    public interface IConfigProvider
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <returns>Return a <see cref="ILoggerFactoryConfiguration"/></returns>
        ILoggerFactoryConfiguration GetConfiguration();
    }
}