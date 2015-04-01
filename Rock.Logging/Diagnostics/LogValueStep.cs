using System.Text;

namespace Rock.Logging.Diagnostics
{
    public class LogValueStep<T> : IStep
    {
        private readonly T _value;
        private readonly string _identifier;

        public LogValueStep(T value, string identifier)
        {
            _value = value;
            _identifier = identifier;
        }

        public void AddToReport(StringBuilder report)
        {
            report.AppendLine(string.Format("{0}: {1}", _identifier, _value));
        }
    }

    public class LogValueStep : IStep
    {
        private readonly string _value;
        private readonly string _identifier;

        public LogValueStep(string value, string identifier)
        {
            _value = value;
            _identifier = identifier;
        }

        public void AddToReport(StringBuilder report)
        {
            report.AppendLine(string.Format("{0}: {1}", _identifier, _value));
        }
    }
}