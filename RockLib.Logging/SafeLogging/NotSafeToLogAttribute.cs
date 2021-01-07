using System;

namespace RockLib.Logging.SafeLogging
{
    /// <summary>
    /// An attribute that signifies a property as not safe for logging. This should be used when a class is
    /// marked with the <see cref="SafeToLogAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotSafeToLogAttribute : Attribute
    {
    }
}
