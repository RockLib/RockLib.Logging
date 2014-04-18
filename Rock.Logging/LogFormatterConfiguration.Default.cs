namespace Rock.Logging
{
    public partial class LogFormatterConfiguration
    {
        private class DefaultLogFormatterConfiguration : ILogFormatterConfiguration
        {
            private const string _defaultTemplate =
@"--Message--
{message}

--Exception--
{exception}

--Extended Properties--
{extendedProperties(-{key}-
{value}
)}";
//            private const string _defaultTemplate =
            //@"--Message--{newLine}{message}{newLine}{newLine}--Exception--{newLine}{exception}{newLine}{newLine}--Extended Properties--{newLine}{extendedProperties(-{key}-{value}{newLine})}";

            public string Name
            {
                get { return null; }
            }

            public string Template
            {
                get { return _defaultTemplate; }
            }
        }
    }
}