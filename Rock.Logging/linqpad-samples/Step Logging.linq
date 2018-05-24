<Query Kind="Program">
  <NuGetReference>Rock.Logging</NuGetReference>
  <Namespace>Rock.DependencyInjection</Namespace>
  <Namespace>Rock.Logging</Namespace>
  <Namespace>Rock.Logging.Diagnostics</Namespace>
  <Namespace>Rock.Serialization</Namespace>
</Query>

void Main()
{
    ILogger logger = LoggerFactory.GetInstance();
    
    // Sometimes you need to log step-by-step details, perhaps about an algorithm.
    // Sending multiple logs isn't the right thing to do - what we want is an easy
    // way to log a bunch of steps with one log entry.

    // Begin step logging by calling the CreateStepLogger extension method, found
    // in the Rock.Logging.Diagnostics namespace. The log entry is actually sent
    // when the step logger is disposed, so be sure to use it in a using block.
    using (var stepLogger = logger.CreateStepLogger())
    {
        // Add sequential messages.
        stepLogger.AddMessage("Begin Foo process.");
        
        // You can log timing information by calling IStepLogger extension
        // method, GetStopwatch().
        var stopwatch = stepLogger.GetStopwatch();
        
        for (int i = 0; i < 5; i++)
        {
            Thread.Sleep(i * 100);
            
            // Record the elapsed time of the stopwatch. In this demo,
            // we're restarting the stopwatch after each loop iteration.
            stopwatch.RecordElapsed("Bar loop index " + i, andRestartStopwatch:true);
        }
        
        // Another sequential message.
        stepLogger.AddMessage("Bar loop finished.");
        
        // The LogValue extension method operates on any type, and returns the original
        // value - similar to LINQPad's Dump extension method. The second parameter to
        // the LogValue extension method takes a step logger.
        double piSquared = (Math.PI.LogValue(stepLogger, "PI") * Math.PI).LogValue(stepLogger, "PI * PI");
    }
}