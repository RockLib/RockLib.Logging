using System;

namespace Rock.Logging
{
    /// <summary>
    /// Logging throttling rule.  Used to throttle how often <see cref="Rock.Logging.Logger"/> logs the same message.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <h1>Overview</h1>
    /// Throttling is used to stop <see cref="Rock.Logging.Logger"/> from sending out the same log message 
    /// over and over.  It isn't uncommon for systems to go down and when they do if the application is 
    /// configured to use the <see cref="Rock.Logging.Provider.EmailLogProvider"/> this can flood our 
    /// infrastructure including our Exchange servers and network.  To stop getting repeat messages from an application 
    /// throttling can be placed on a <see cref="Rock.Logging.Configuration.Category"/>.  It is not required. 
    /// The format is "hh:mm:ss" and the default is 00:00:00.  The uniqueness of the log entry
    /// is determined by the combination of Message, ExceptionData, Level, IsUserDisrupted, Referrer, AffectedSystem,
    /// RequestMethod, CategoryId, Url.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// <throttlingRules>
    ///	  <throttlingRule name="Default" minInterval="00:05:00"/>
    ///	  <throttlingRule name="Critical" minInterval="00:01:00"/>
    /// </throttlingRules>
    /// <categories>
    ///	<category name="File" throttlingRule="Default">
    ///		<providers>
    ///			<provider type="Rock.Logging.Provider.FileLogProvider, Rock.Framework" formatter="default" >
    ///				<propertyMapper>
    ///					<mapper property="File" value="Log.txt" />
    ///				</propertyMapper>
    ///			</provider>
    ///		</providers>
    ///	</category>
    ///	</categories>
    ///	]]>
    ///	</code>
    /// </example>
    public class ThrottlingRuleConfiguration : IThrottlingRuleConfiguration
    {
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the minimum time interval between logging the same <see cref="Rock.Logging.LogEntry"/>.
        /// </summary>
        /// <value>The min interval.</value>
        public TimeSpan MinInterval { get; set; }

        /// <summary>
        /// Gets or sets the min event threshold before firing an equivalent <see cref="Rock.Logging.LogEntry"/>. 
        /// For example if this value is set to 3 then every third event will be logged.
        /// If both MinInterval and MinEventThreshold are set then a OR logical operation is performed to see if a log entry will be logged.
        /// If the log entry passes at least one of the rule then it will be logged.
        /// Setting this value to 0 or 1 has no effect on throttling. If set to -1 and MinInterval is set to a non-zero value then
        /// the first message in a series will be skipped.
        /// </summary>
        /// <value>The min event threshold.</value>
        public int MinEventThreshold { get; set; }
    }
}