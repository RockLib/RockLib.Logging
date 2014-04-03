using System.Collections.Generic;

namespace Rock.Logging
{
    internal static class Default
    {
        public static ILoggerConfiguration LoggerConfiguration
        {
            get { throw new System.NotImplementedException(); }
        }

        public static IEnumerable<IContextProvider> ContextProviders
        {
            get
            {
                yield break;
            }
        }
    }
}