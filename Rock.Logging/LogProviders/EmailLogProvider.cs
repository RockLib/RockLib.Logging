using System.Net.Mail;
using System.Threading.Tasks;
using Rock.Mail;

namespace Rock.Logging
{
    public class EmailLogProvider : FormattableLogProvider
    {
        private readonly DeliveryMethod _deliveryMethod;

        public EmailLogProvider(
            ILogFormatterFactory logFormatterFactory,
            DeliveryMethod deliveryMethod = null)
            : base(logFormatterFactory)
        {
            _deliveryMethod = deliveryMethod ?? DeliveryMethod.Default;
        }

        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }

        protected override async Task Write(LogEntry entry, string formattedLogEntry)
        {
            using (var mailMessage = GetMailMessage(entry, formattedLogEntry))
            {
                await mailMessage.Send(_deliveryMethod);
            }
        }

        private MailMessage GetMailMessage(LogEntry entry, string body)
        {
            var to = ToEmail.Replace(';', ',');
            var subject = new LogFormatter(Subject).Format(entry);

            return new MailMessage(FromEmail, to, subject, body) { IsBodyHtml = true };
        }
    }
}