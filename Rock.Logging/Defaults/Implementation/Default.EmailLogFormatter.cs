using System;
using Rock.Defaults;

namespace Rock.Logging.Defaults.Implementation
{
    public static partial class Default
    {
        private const string _defaultEmailTemplate = "&lt;p&gt;&lt;h3&gt;LOG INFO&lt;/h3&gt;&lt;b&gt;Message:&lt;/b&gt; {message}&lt;br /&gt;&lt;b&gt;Create Time:&lt;/b&gt; {createTime(O)}&lt;br /&gt;&lt;b&gt;Type of Message:&lt;/b&gt; {level} &lt;br /&gt;&lt;b&gt;Environment:&lt;/b&gt; {environment}&lt;br /&gt;&lt;b&gt;Application ID:&lt;/b&gt; {applicationId} &lt;br /&gt;&lt;b&gt;Application User ID:&lt;/b&gt; {applicationUserId} &lt;br /&gt;&lt;b&gt;Machine Name:&lt;/b&gt; {machineName}&lt;br /&gt;&lt;/p&gt;&lt;hr /&gt;&lt;p&gt;&lt;h4&gt;EXTENDED PROPERTY INFO&lt;/h4&gt;&lt;table cellpadding=3 cellspacing=0 border=1 style='background-color: cornsilk' &gt;{extendedProperties(&lt;tr&gt;&lt;td valign=top style='font-weight:900; color: navy;'&gt;&lt;pre style='margin-bottom: 0px'&gt;{key}&lt;/td&gt;&lt;td valign=top&gt;&lt;pre style='margin-bottom: 0px'&gt;{value}&lt;/td&gt;&lt;/tr&gt;)}&lt;/table&gt;&lt;/p&gt;&lt;hr /&gt;&lt;h4&gt;EXCEPTION INFO&lt;/h4&gt;&lt;b&gt;Exception Type:&lt;/b&gt; {exceptionType} &lt;br /&gt;&lt;b&gt;Exception Context:&lt;/b&gt; {exceptionContext} &lt;br /&gt;&lt;br /&gt;&lt;pre&gt;{exception}&lt;/pre&gt;&lt;/p&gt;";

        private static readonly DefaultHelper<ILogFormatter> _emailLogFormatter = new DefaultHelper<ILogFormatter>(() => new TemplateLogFormatter(_defaultEmailTemplate));

        public static ILogFormatter EmailLogFormatter
        {
            get { return _emailLogFormatter.Current; }
        }

        public static ILogFormatter DefaultEmailLogFormatter
        {
            get { return _emailLogFormatter.DefaultInstance; }
        }

        public static void SetEmailLogFormatter(Func<ILogFormatter> getEmailLogFormatterInstance)
        {
            _emailLogFormatter.SetCurrent(getEmailLogFormatterInstance);
        }

        public static void RestoreEmailLogFormatter()
        {
            _emailLogFormatter.RestoreDefault();
        }
    }
}
