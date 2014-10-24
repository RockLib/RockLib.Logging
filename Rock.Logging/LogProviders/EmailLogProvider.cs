using System.Net.Mail;
using System.Threading.Tasks;
using Rock.Mail;
using Rock.Logging.Defaults.Implementation;

namespace Rock.Logging
{
    public class EmailLogProvider : FormattableLogProvider
    {
        private readonly DeliveryMethod _deliveryMethod;

        public EmailLogProvider()
            : this(null, null)
        {
        }

        public EmailLogProvider(
            DeliveryMethod deliveryMethod = null,
            ILogFormatter logFormatter = null)
            : base(logFormatter ?? Default.EmailLogFormatter)
        {
            _deliveryMethod = deliveryMethod ?? DeliveryMethod.Default;
        }

        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }

        protected override async Task WriteAsync(LogEntry entry, string formattedLogEntry)
        {
            using (var mailMessage = GetMailMessage(entry, formattedLogEntry))
            {
                await mailMessage.SendAsync(_deliveryMethod);
            }
        }

        private MailMessage GetMailMessage(LogEntry entry, string body)
        {
            var to = ToEmail.Replace(';', ',');
            var subject = new TemplateLogFormatter(Subject).Format(entry);

            return new MailMessage(FromEmail, to, subject, body) { IsBodyHtml = true };
        }
    }
}