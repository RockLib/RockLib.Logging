namespace RockLib.Logging
{
    /// <summary>
    /// Defines types of processing modes used by logger objects.
    /// </summary>
    public enum ProcessingMode
    {
        /// <summary>
        /// The logger should process and track logs on dedicated non-threadpool
        /// background threads.
        /// </summary>
        Background,

        /// <summary>
        /// The logger should process logs on the same thread as the caller.
        /// </summary>
        Synchronous,

        /// <summary>
        /// The logger should process logs asynchronously, but without any
        /// task tracking.
        /// </summary>
        FireAndForget
    }
}
