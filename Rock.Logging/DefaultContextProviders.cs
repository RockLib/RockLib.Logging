using System.Collections.Generic;
using Rock.Immutable;

namespace Rock.Logging
{
    public static class DefaultContextProviders
    {
        private static readonly Semimutable<IEnumerable<IContextProvider>> _contextProviders = new Semimutable<IEnumerable<IContextProvider>>(GetDefault);

        public static IEnumerable<IContextProvider> Current
        {
            get { return _contextProviders.Value; }
        }

        public static void SetCurrent(IEnumerable<IContextProvider> value)
        {
            _contextProviders.Value = value;
        }

        private static IEnumerable<IContextProvider> GetDefault()
        {
            return new IContextProvider[0];
        }
    }
}