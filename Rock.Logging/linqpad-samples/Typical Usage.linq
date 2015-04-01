<Query Kind="Program">
  <NuGetReference Prerelease="true">Rock.Logging</NuGetReference>
  <Namespace>Rock.DependencyInjection</Namespace>
  <Namespace>Rock.Logging</Namespace>
  <Namespace>Rock.Serialization</Namespace>
</Query>

void Main()
{
    // Rock.Logging is designed to work out-of-the-box, using absolutely zero
    // configuration or setup code. When this is the case (no config or setup
    // code), it just writes logs to the console.

    // To get an instance of a logger, use the LoggerFactory class.
    ILogger logger = LoggerFactory.GetInstance();
    
    // There are many extension methods that operate on the ILogger interface.
    
    // When you know your logging level and all you need to log is a string...
    logger.Debug("Hello, world!");
    
    try
    {	        
        int i = 0;
        int j = 1;
        int k = j / i;
    }
    catch (DivideByZeroException ex)
    {
        // When you have an exception...
        logger.Error(ex, "Error in demo code.");
    }
    
    try
    {
        int[] data = new int[10];
        data[11] = 123;
    }
    catch (IndexOutOfRangeException ex)
    {
        // When you need to add extended properties...
        if (logger.IsWarnEnabled())
        {
            LogEntry logEntry = new LogEntry("Another error in demo code.", ex, "Attempting to grault the corge.");
            
            logEntry.ExtendedProperties.Add("Foo", "Bar");
            logEntry.ExtendedProperties.Add("Baz", "Qux");
            
            logger.Warn(logEntry);
        }
    }
}