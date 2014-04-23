using System.Collections.Generic;

namespace Rock.Logging
{
    /// <summary>
    /// <see cref="Logger"/> supports the concept of multiple categories. This allows developers to 
    /// create multiple categories for logging scenarios.  A category is made up of one or more log providers 
    /// and the properties those log providers need in order to be initialized.
    /// </summary>
    public class Category : ICategory
    {
        /// <summary
        /// >
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
        IThrottlingRuleConfiguration ICategory.ThrottlingRule
        {
            get { return ThrottlingRule; }
        }

        public ThrottlingRuleConfiguration ThrottlingRule { get; set; }
    }
}