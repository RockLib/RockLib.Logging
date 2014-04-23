namespace Rock.Logging
{
    public class NullThrottlingRuleEvaluator : IThrottlingRuleEvaluator
    {
        public static readonly IThrottlingRuleEvaluator Instance = new NullThrottlingRuleEvaluator();

        private NullThrottlingRuleEvaluator()
        {
        }

        public bool ShouldLog(LogEntry logEntry)
        {
            return true;
        }
    }
}