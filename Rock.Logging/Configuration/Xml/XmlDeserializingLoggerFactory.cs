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

        [XmlAttribute("isLoggingEnabled")]
        public bool IsLoggingEnabled { get; set; }

        [XmlAttribute("loggingLevel")]
        public LogLevel LoggingLevel { get; set; }

        [XmlElement("auditLogProvider")]
        public LogProviderProxy AuditLogProvider { get; set; }

        [XmlArray("formatters")]
        [XmlArrayItem("formatter")]
        public LogFormatterProxy[] Formatters
        {
            get { return _formatters.ToArray(); }
            set { _formatters = new FunctionalKeyedCollection<string, LogFormatterProxy>(f => f.Name, value); }
        }

        [XmlArray("throttlingRules")]
        [XmlArrayItem("throttlingRule")]
        public ThrottlingRuleEvaluatorProxy[] ThrottlingRules
        {
            get { return _throttlingRuleEvaluators.ToArray(); }
            set { _throttlingRuleEvaluators = new FunctionalKeyedCollection<string, ThrottlingRuleEvaluatorProxy>(f => f.Name, value); }
        }

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

        [XmlArray("contextProviders")]
        [XmlArrayItem("contextProvider")]
        public ContextProviderProxy[] ContextProviders
        {
            get { return _contextProviders; }
            set { _contextProviders = value ?? new ContextProviderProxy[0]; }
        }

        public void SetApplicationInfo(IApplicationInfo applicationInfo)
        {
            _applicationInfo = applicationInfo ?? Default.ApplicationInfo;
        }

        public void SetSupplementaryContainer(IResolver supplementaryContainer)
        {
            _supplementaryContainer = supplementaryContainer;
        }

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
                    : NullThrottlingRuleEvaluator.Instance;

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