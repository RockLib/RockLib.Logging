namespace RockLib.Logging
{
    public interface ILogFormatter
    {
        string Format(LogEntry entry);
    }
}
