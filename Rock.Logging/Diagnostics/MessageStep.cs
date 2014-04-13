using System.Text;

namespace Rock.Logging.Diagnostics
{
    public class MessageStep : IStep
    {
        private readonly string _message;

        public MessageStep(string message)
        {
            _message = message;
        }

        public void AddToReport(StringBuilder report)
        {
            report.AppendLine(_message);
        }
    }
}