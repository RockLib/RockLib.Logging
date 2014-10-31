namespace Rock.Logging
{
    public class NullThrottlingRuleEvaluator : IThrottlingRuleEvaluator
    {
        public bool ShouldLog(LogEntry logEntry)
        {
            return true;
        }
    }
}