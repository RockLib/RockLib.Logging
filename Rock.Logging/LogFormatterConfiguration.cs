namespace Rock.Logging
{
    public class LogFormatterConfiguration : ILogFormatterConfiguration
    {
        public string Name { get; set; }
        public string Template { get; set; }
    }
}