namespace Rock.Logging
{
    public static class ToThrottlingRuleEvaluatorExtension
    {
        public static IThrottlingRuleEvaluator ToThrottlingRuleEvaluator(this IThrottlingRuleConfiguration throttlingRule)
        {
            return new ThrottlingRuleEvaluator(throttlingRule);
        }
    }
}