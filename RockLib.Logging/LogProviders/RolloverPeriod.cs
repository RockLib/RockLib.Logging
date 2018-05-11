namespace RockLib.Logging
{
    /// <summary>
    /// Defines the various rollover periods.
    /// </summary>
    public enum RolloverPeriod
    {
        /// <summary>
        /// The rolling file log provider should never archive logs on a periodic basis.
        /// </summary>
        Never,

        /// <summary>
        /// The rolling file provider should archive logs daily.
        /// </summary>
        Daily,

        /// <summary>
        /// The rolling file provider should archive logs hourly.
        /// </summary>
        Hourly
    }
}