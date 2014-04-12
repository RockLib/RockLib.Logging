namespace Rock.Logging
{
    public class LogFormatter : ILogFormatter
    {
        public string Name { get; set; }
        public string Template { get; set; }
    }
}