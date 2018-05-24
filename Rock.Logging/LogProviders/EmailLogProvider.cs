using System.Net.Mail;
using System.Threading.Tasks;
using Rock.Immutable;
using Rock.Mail;

namespace Rock.Logging
{
    public class EmailLogProvider : FormattableLogProvider
    {
        private const string _defaultTemplate = "&lt;p&gt;&lt;h3&gt;LOG INFO&lt;/h3&gt;&lt;b&gt;Message:&lt;/b&gt; {message}&lt;br /&gt;&lt;b&gt;Create Time:&lt;/b&gt; {createTime(O)}&lt;br /&gt;&lt;b&gt;Type of Message:&lt;/b&gt; {level} &lt;br /&gt;&lt;b&gt;Environment:&lt;/b&gt; {environment}&lt;br /&gt;&lt;b&gt;Application ID:&lt;/b&gt; {applicationId} &lt;br /&gt;&lt;b&gt;Application User ID:&lt;/b&gt; {applicationUserId} &lt;br /&gt;&lt;b&gt;Machine Name:&lt;/b&gt; {machineName}&lt;br /&gt;&lt;/p&gt;&lt;hr /&gt;&lt;p&gt;&lt;h4&gt;EXTENDED PROPERTY INFO&lt;/h4&gt;&lt;table cellpadding=3 cellspacing=0 border=1 style='background-color: cornsilk' &gt;{extendedProperties(&lt;tr&gt;&lt;td valign=top style='font-weight:900; color: navy;'&gt;&lt;pre style='margin-bottom: 0px'&gt;{key}&lt;/td&gt;&lt;td valign=top&gt;&lt;pre style='margin-bottom: 0px'&gt;{value}&lt;/td&gt;&lt;/tr&gt;)}&lt;/table&gt;&lt;/p&gt;&lt;hr /&gt;&lt;h4&gt;EXCEPTION INFO&lt;/h4&gt;&lt;b&gt;Exception Type:&lt;/b&gt; {exceptionType} &lt;br /&gt;&lt;b&gt;Exception Context:&lt;/b&gt; {exceptionContext} &lt;br /&gt;&lt;br /&gt;&lt;pre&gt;{exception}&lt;/pre&gt;&lt;/p&gt;";

        private static readonly Semimutable<DeliveryMethod> _defaultDeliveryMethod = new Semimutable<DeliveryMethod>(GetDefaultDefaultDeliveryMethod);
        private static readonly Semimutable<ILogFormatter> _defaultLogFormatter = new Semimutable<ILogFormatter>(GetDefaultDefaultLogFormatter);

        private readonly DeliveryMethod _deliveryMethod;

        public EmailLogProvider()
            : this(null, null)
        {
        }

        public EmailLogProvider(
            DeliveryMethod deliveryMethod = null,
            ILogFormatter logFormatter = null)
            : base(logFormatter ?? DefaultLogFormatter)
        {
            _deliveryMethod = deliveryMethod ?? DefaultDeliveryMethod;
        }

        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }

        protected override async Task WriteAsync(ILogEntry entry, string formattedLogEntry)
        {
            using (var mailMessage = GetMailMessage(entry, formattedLogEntry))
            {
                await mailMessage.SendAsync(_deliveryMethod).ConfigureAwait(false);
            }
        }

        private MailMessage GetMailMessage(ILogEntry entry, string body)
        {
            var to = ToEmail.Replace(';', ',');
            var subject = new TemplateLogFormatter(Subject).Format(entry);

            return new MailMessage(FromEmail, to, subject, body) { IsBodyHtml = true };
        }

        public static DeliveryMethod DefaultDeliveryMethod
        {
            get { return _defaultDeliveryMethod.Value; }
        }

        public static void SetDefaultDeliveryMethod(DeliveryMethod deliveryMethod)
        {
            _defaultDeliveryMethod.Value = deliveryMethod;
        }

        private static DeliveryMethod GetDefaultDefaultDeliveryMethod()
        {
            return DeliveryMethod.Default;
        }

        public static ILogFormatter DefaultLogFormatter
        {
            get { return _defaultLogFormatter.Value; }
        }

        public static void SetDefaultLogFormatter(ILogFormatter logFormatter)
        {
            _defaultLogFormatter.Value = logFormatter;
        }

        private static ILogFormatter GetDefaultDefaultLogFormatter()
        {
            return new TemplateLogFormatter(_defaultTemplate);
        }
    }
}