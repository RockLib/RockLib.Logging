using System.Collections.Generic;
using Rock.Collections;

namespace Rock.Logging
{
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
}