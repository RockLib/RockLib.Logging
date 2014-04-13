using System.Text;

namespace Rock.Logging.Diagnostics
{
    public class MessageStep : IStep, IStepSnapshot
    {
        private readonly string _message;

        public MessageStep(string message)
        {
            _message = message;
        }

        public IStepSnapshot GetSnapshot()
        {
            return this;
        }

        public void AddToReport(StringBuilder report)
        {
            report.AppendLine(_message);
        }
    }
}