using System;

namespace RockLib.Logging.SafeLogging
{
    /// <summary>
    /// An attribute that signifies a property or all the properties of a class are safe to add as extended properties
    /// in a log. When used with a class, use the <see cref="NotSafeToLogAttribute"/> to mark specific properites
    /// that should not be logged.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class SafeToLogAttribute : Attribute
    {
    }
}
