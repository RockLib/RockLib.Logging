using Rock.Immutable;

namespace Rock.Logging
{
    public class DefaultEmailLogFormatter
    {
        private const string _defaultEmailTemplate = "&lt;p&gt;&lt;h3&gt;LOG INFO&lt;/h3&gt;&lt;b&gt;Message:&lt;/b&gt; {message}&lt;br /&gt;&lt;b&gt;Create Time:&lt;/b&gt; {createTime(O)}&lt;br /&gt;&lt;b&gt;Type of Message:&lt;/b&gt; {level} &lt;br /&gt;&lt;b&gt;Environment:&lt;/b&gt; {environment}&lt;br /&gt;&lt;b&gt;Application ID:&lt;/b&gt; {applicationId} &lt;br /&gt;&lt;b&gt;Application User ID:&lt;/b&gt; {applicationUserId} &lt;br /&gt;&lt;b&gt;Machine Name:&lt;/b&gt; {machineName}&lt;br /&gt;&lt;/p&gt;&lt;hr /&gt;&lt;p&gt;&lt;h4&gt;EXTENDED PROPERTY INFO&lt;/h4&gt;&lt;table cellpadding=3 cellspacing=0 border=1 style='background-color: cornsilk' &gt;{extendedProperties(&lt;tr&gt;&lt;td valign=top style='font-weight:900; color: navy;'&gt;&lt;pre style='margin-bottom: 0px'&gt;{key}&lt;/td&gt;&lt;td valign=top&gt;&lt;pre style='margin-bottom: 0px'&gt;{value}&lt;/td&gt;&lt;/tr&gt;)}&lt;/table&gt;&lt;/p&gt;&lt;hr /&gt;&lt;h4&gt;EXCEPTION INFO&lt;/h4&gt;&lt;b&gt;Exception Type:&lt;/b&gt; {exceptionType} &lt;br /&gt;&lt;b&gt;Exception Context:&lt;/b&gt; {exceptionContext} &lt;br /&gt;&lt;br /&gt;&lt;pre&gt;{exception}&lt;/pre&gt;&lt;/p&gt;";

        private static readonly Semimutable<ILogFormatter> _logFormatter = new Semimutable<ILogFormatter>(GetDefault);

        public static ILogFormatter Current
        {
            get { return _logFormatter.Value; }
        }

        public static void SetCurrent(ILogFormatter logFormatter)
        {
            _logFormatter.Value = logFormatter;
        }

        private static ILogFormatter GetDefault()
        {
            return new TemplateLogFormatter(_defaultEmailTemplate);
        }
    }
}