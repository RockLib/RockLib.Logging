#if !NET451
namespace RockLib.Logging.DependencyInjection
{
    public interface ILoggerBuilder
    {
        string LoggerName { get; }

        ILoggerBuilder AddLogProvider(LogProviderRegistration registration);

        ILoggerBuilder AddContextProvider(ContextProviderRegistration registration);
    }
}
#endif
