namespace Rock.Logging.Defaults.Implementation
{
    public static partial class Default
    {
        private static IContextProvider[] _contextProviders = new IContextProvider[0];

        public static IContextProvider[] ContextProviders
        {
            get { return _contextProviders; }
            set { _contextProviders = value ?? new IContextProvider[0]; }
        }
    }
}
