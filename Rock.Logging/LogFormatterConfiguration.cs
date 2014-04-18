namespace Rock.Logging
{
    public partial class LogFormatterConfiguration : ILogFormatterConfiguration
    {
        public static readonly ILogFormatterConfiguration Default = new DefaultLogFormatterConfiguration();

        public string Name { get; set; }
        public string Template { get; set; }
    }
}