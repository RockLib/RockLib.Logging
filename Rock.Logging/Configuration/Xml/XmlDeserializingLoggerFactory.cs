using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Rock.Collections;
using Rock.Defaults.Implementation;
using Rock.DependencyInjection;

namespace Rock.Logging.Configuration
{
    /// <summary>
    /// An implementation of <see cref="ILoggerFactory"/> that is initialized via xml serialization.
    /// </summary>
    public class XmlDeserializingLoggerFactory : ILoggerFactory
    {
        private const string _defaultCategoryXml =
@"<category name=""Default"">
  <providers>
    <provider type=""Rock.Logging.ConsoleLogProvider, Rock.Logging"" />
  </providers>
</category>";

        private static readonly IKeyedEnumerable<string, Category> _defaultCategories;

        private readonly ConditionalWeakTable<Category, ConcurrentDictionary<Type, IResolver>> _containerMapTable = new ConditionalWeakTable<Category, ConcurrentDictionary<Type, IResolver>>();

        private IApplicationInfo _applicationInfo = Default.ApplicationInfo;
        private IResolver _supplementaryContainer;

        private IKeyedEnumerable<string, LogFormatterProxy> _formatters = new FunctionalKeyedCollection<string, LogFormatterProxy>(f => f.Name, Enumerable.Empty<LogFormatterProxy>());
        private IKeyedEnumerable<string, ThrottlingRuleEvaluatorProxy> _throttlingRuleEvaluators = new FunctionalKeyedCollection<string, ThrottlingRuleEvaluatorProxy>(f => f.Name, Enumerable.Empty<ThrottlingRuleEvaluatorProxy>());
        private IKeyedEnumerable<string, Category> _categories = _defaultCategories;
        private ContextProviderProxy[] _contextProviders = new ContextProviderProxy[0];

        static XmlDeserializingLoggerFactory()
        {
            var serializer = new XmlSerializer(typeof(Category));

            using (var reader = new StringReader(_defaultCategoryXml))
            {
                _defaultCategories =
                    new FunctionalKeyedCollection<string, Category>(
                        x => x.Name,
                        new[]
                        {
                            (Category)serializer.Deserialize(reader)
                        });
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether loggers should be enabled.
        /// </summary>
        [XmlAttribute("isLoggingEnabled")]
        public bool IsLoggingEnabled { get; set; }

        /// <summary>
        /// Gets or sets the logging level that is specified for loggers.
        /// </summary>
        [XmlAttribute("loggingLevel")]
        public LogLevel LoggingLevel { get; set; }

        /// <summary>
        /// Gets or sets an object that creates an instance of <see cref="ILogProvider"/> that loggers use for auditing.
        /// </summary>
        [XmlElement("auditLogProvider")]
        public LogProviderProxy AuditLogProvider { get; set; }

        /// <summary>
        /// Gets or sets an array of objects that create instances of <see cref="ILogFormatter"/> that log providers use to format a log entry.
        /// </summary>
        [XmlArray("formatters")]
        [XmlArrayItem("formatter")]
        public LogFormatterProxy[] Formatters
        {
            get { return _formatters.ToArray(); }
            set { _formatters = new FunctionalKeyedCollection<string, LogFormatterProxy>(f => f.Name, value); }
        }

        /// <summary>
        /// Gets or sets an array of objects that create instances of <see cref="IThrottlingRuleEvaluator"/> that log providers use to throttle log entries.
        /// </summary>
        [XmlArray("throttlingRules")]
        [XmlArrayItem("throttlingRule")]
        public ThrottlingRuleEvaluatorProxy[] ThrottlingRules
        {
            get { return _throttlingRuleEvaluators.ToArray(); }
            set { _throttlingRuleEvaluators = new FunctionalKeyedCollection<string, ThrottlingRuleEvaluatorProxy>(f => f.Name, value); }
        }

        /// <summary>
        /// Gets or sets an array of <see cref="Category"/> objects that define specific logging scenarios.
        /// </summary>
        [XmlArray("categories")]
        [XmlArrayItem("category")]
        public Category[] Categories
        {
            get { return _categories.ToArray(); }
            set
            {
                if (value == null || value.Length == 0)
                {
                    _categories = _defaultCategories;
                }
                else
                {
                    _categories = new FunctionalKeyedCollection<string, Category>(f => f.Name, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets an array of objects that create instances of <see cref="IContextProvider"/> that a logger can use to provide additional information to log entries.
        /// </summary>
        [XmlArray("contextProviders")]
        [XmlArrayItem("contextProvider")]
        public ContextProviderProxy[] ContextProviders
        {
            get { return _contextProviders; }
            set { _contextProviders = value ?? new ContextProviderProxy[0]; }
        }

        /// <summary>
        /// Sets the <see cref="IApplicationInfo"/> that is used by this instance
        /// of <see cref="XmlDeserializingLoggerFactory"/>. If <paramref name="applicationInfo"/>
        /// is null, then <see cref="Default.ApplicationInfo"/> will be used.
        /// </summary>
        /// <param name="applicationInfo">The <see cref="IApplicationInfo"/> to use.</param>
        public void SetApplicationInfo(IApplicationInfo applicationInfo)
        {
            _applicationInfo = applicationInfo ?? Default.ApplicationInfo;
        }

        /// <summary>
        /// Sets the supplementary <see cref="IResolver"/> instance that is used by this
        /// instance of <see cref="XmlDeserializingLoggerFactory"/> to resolve dependencies
        /// not provided from xml deserialization.
        /// </summary>
        /// <param name="supplementaryContainer">The supplementary <see cref="IResolver"/> instance.</param>
        public void SetSupplementaryContainer(IResolver supplementaryContainer)
        {
            _supplementaryContainer = supplementaryContainer;
        }

        /// <summary>
        /// Get an instance of <typeparamref name="TLogger" /> for the given category.
        /// </summary>
        /// <typeparam name="TLogger">The type of <see cref="ILogger" /> to return.</typeparam>
        /// <param name="categoryName">An optional category.</param>
        /// <returns>
        /// An instance of <typeparamref name="TLogger" />.
        /// </returns>
        public TLogger Get<TLogger>(string categoryName = null) where TLogger : ILogger
        {
            var category =
                categoryName != null && _categories.Contains(categoryName)
                    ? _categories[categoryName]
                    : _categories.First();

            var containerMap = _containerMapTable.GetValue(category, c => new ConcurrentDictionary<Type, IResolver>());
            var container = containerMap.GetOrAdd(typeof(TLogger), t => GetLoggerContainer(category));
            return container.Get<TLogger>();
        }

        private IResolver GetLoggerContainer(Category category)
        {
            var configuration = new LoggerConfiguration
            {
                IsLoggingEnabled = IsLoggingEnabled,
                LoggingLevel = LoggingLevel
            };

            var logProviders =
                category.LogProviders.Select(x => x.CreateInstance(_formatters, _supplementaryContainer)).ToList();

            var applicationInfo = _applicationInfo;

            var auditLogProvider =
                AuditLogProvider != null
                    ? AuditLogProvider.CreateInstance(_formatters, _supplementaryContainer)
                    : null;

            var throttlingRuleEvaluator =
                category.ThrottlingRule != null && _throttlingRuleEvaluators.Contains(category.ThrottlingRule)
                    ? _throttlingRuleEvaluators[category.ThrottlingRule].CreateInstance(_supplementaryContainer)
                    : new NullThrottlingRuleEvaluator();

            var contextProviders = ContextProviders.Select(x => x.CreateInstance(_supplementaryContainer)).ToList();

            var container =
                new AutoContainer(
                    configuration,
                    logProviders,
                    applicationInfo,
                    auditLogProvider,
                    throttlingRuleEvaluator,
                    contextProviders);

            if (_supplementaryContainer != null)
            {
                container = container.MergeWith(_supplementaryContainer);
            }

            return container;
        }
    }
}