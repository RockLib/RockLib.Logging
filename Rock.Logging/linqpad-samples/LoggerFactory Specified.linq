<Query Kind="Program">
  <NuGetReference>Rock.Logging</NuGetReference>
  <Namespace>Rock.DependencyInjection</Namespace>
  <Namespace>Rock.Logging</Namespace>
  <Namespace>Rock.Serialization</Namespace>
</Query>

void Main()
{
    // The LoggerFactory class allows you to specify a ILoggerFactory
    // implementation for it to use.

    // One implementation that comes with Rock.Logging is SimpleLoggerFactory.
    // It is a generic type, and its generic argument is the type of log
    // provider that a logger created by the factory will use.

    // To specify the ILoggerFactory implementation that LoggerFactory
    // will use, call its SetCurrent method. In this case, we'll use
    // the ConsoleLogProvider. We're also specifying the log level (Warn) of
    // loggers created by the factory. Note: SimpleLoggerFactory requires that
    // the log provider type satisfy the 'new()' constraint - it uses
    // the log provider's default constructor.
    LoggerFactory.SetCurrent(new SimpleLoggerFactory<ConsoleLogProvider>(LogLevel.Warn));
    
    // Let's say we don't like the default format for ConsoleLogProvider. To
    // change it, call its SetDefaultLogFormatter method. The formatter passed
    // to that method will be used when the log formatter is not specified to
    // the constructor (which is the case with the default constructor).
    ConsoleLogProvider.SetDefaultLogFormatter(new CsvLogFormatter()); // CsvLogFormatter defined below
    
    // Get the logger as usual.
    ILogger logger = LoggerFactory.GetInstance();
    
    // Use the logger as usual. (but a Warn log level means this entry will be ignored)
    logger.Debug("Hello, world!");
    
    try
    {	        
        int i = 0;
        int j = 1;
        int k = j / i;
    }
    catch (DivideByZeroException ex)
    {
        // More typical logger usage.
        logger.Error(ex, "Error in demo code.");
    }
    
    try
    {
        int[] data = new int[10];
        data[11] = 123;
    }
    catch (IndexOutOfRangeException ex)
    {
        // Even more typical usage. Note that extended properties are ignored by the log provider.
        if (logger.IsWarnEnabled())
        {
            LogEntry logEntry = new LogEntry("Another error in demo code.", ex, "Attempting to grault the corge.");
            
            logEntry.ExtendedProperties.Add("Foo", "Bar");
            logEntry.ExtendedProperties.Add("Baz", "Qux");
            
            logger.Warn(logEntry);
        }
    }
}

private class CsvLogFormatter : ILogFormatter
{
    public string Format(LogEntry logEntry)
    {
        return string.Format(
            "{0}, {1:O}{2}{3}",
            logEntry.Level,
            logEntry.CreateTime,
            GetCsvAppender(logEntry.Message),
            GetCsvAppender(logEntry.ExceptionDetails));
    }
    
    public static string GetCsvAppender(string value)
    {
        if (value == null)
        {
            return "";
        }
        return ", \"" + value.Replace("\"", "'").Replace("\r", "").Replace("\n", "**") + "\"";
    }
}