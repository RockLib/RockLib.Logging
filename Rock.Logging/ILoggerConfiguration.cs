using System;
using System.Collections.Generic;
using Rock.Collections;

namespace Rock.Logging
{
    public interface ILoggerConfiguration
    {
        bool IsLoggingEnabled { get; }
        LogLevel LoggingLevel { get; }
        ILogProvider AuditLogProvider { get; }
    }

    public interface ILoggerFactoryConfiguration : ILoggerConfiguration
    {
        IKeyedEnumerable<string, ICategory> Categories { get; }
        IKeyedEnumerable<string, ILogFormatter> Formatters { get; }
        IKeyedEnumerable<string, IThrottlingRuleConfiguration> ThrottlingRules { get; }
    }

    public interface ILogFormatter
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

    public interface ICategory
    {
        /// <summary>
        /// Gets the name of the category.
        /// </summary>
        /// <remarks>At least one category is required.</remarks>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the collection of log providers specified.
        /// </summary>
        /// <value>The providers.</value>
        IEnumerable<ILogProviderConfiguration> Providers { get; }

        /// <summary>
        /// Gets the throttling rule.
        /// </summary>
        /// <value>The throttling rule.</value>
        IThrottlingRuleConfiguration ThrottlingRule { get; }
    }

    public interface ILogProviderConfiguration
    {
        Type ProviderType { get; }
        string FormatterName { get; }
    }

    /// <summary>
    /// <see cref="Logger"/> supports the concept of multiple categories. This allows developers to 
    /// create multiple categories for logging scenarios.  A category is made up of one or more log providers 
    /// and the properties those log providers need in order to be initialized.
    /// </summary>
    public class Category : ICategory
    {
        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        /// <remarks>At least one category is required.</remarks>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection of log providers specified.
        /// </summary>
        /// <value>The providers.</value>
        public IEnumerable<ILogProviderConfiguration> Providers { get; set; }

        /// <summary>
        /// Gets or sets the throttling rule.
        /// </summary>
        /// <value>The throttling rule.</value>
        public IThrottlingRuleConfiguration ThrottlingRule { get; set; }
    }
}