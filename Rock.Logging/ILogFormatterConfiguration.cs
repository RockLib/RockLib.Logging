namespace Rock.Logging
{
    public interface ILogFormatterConfiguration
    {
        /// <summary>
        /// Gets the name of the log formatter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the template to be used for logging messages.
        /// </summary>
        string Template { get; }
    }
}