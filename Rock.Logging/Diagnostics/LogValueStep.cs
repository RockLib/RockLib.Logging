using System;
using System.Text;

namespace Rock.Logging.Diagnostics
{
    public class LogValueStep<T> : IStep, IStepSnapshot
        where T : struct
    {
        private readonly T _value;
        private readonly string _identifier;

        public LogValueStep(T value, string identifier)
        {
            _value = value;
            _identifier = identifier;
        }

        public IStepSnapshot GetSnapshot()
        {
            return this;
        }

        public void AddToReport(StringBuilder report)
        {
            report.AppendLine(String.Format("{0}: {1}", _identifier, _value));
        }
    }
}