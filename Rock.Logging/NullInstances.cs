using System;

namespace Rock.Logging
{
    public static class Null
    {
        private static readonly Lazy<IThrottlingRuleEvaluator> _nullThrottlingRuleEvaluator;

        static Null()
        {
            _nullThrottlingRuleEvaluator = new Lazy<IThrottlingRuleEvaluator>(() => new NullThrottlingRuleEvaluator());
        }

        /// <summary>
        /// A <see cref="IThrottlingRuleEvaluator"/> that always returns true for its
        /// <see cref="IThrottlingRuleEvaluator.ShouldLog"/> method.
        /// </summary>
        public static IThrottlingRuleEvaluator ThrottlingRuleEvaluator
        {
            get { return _nullThrottlingRuleEvaluator.Value; }
        }

        private class NullThrottlingRuleEvaluator : IThrottlingRuleEvaluator
        {
            public bool ShouldLog(LogEntry logEntry)
            {
                return true;
            }
        }
    }
}