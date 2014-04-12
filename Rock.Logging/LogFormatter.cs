using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Rock.Logging
{
    public class LogFormatter : ILogFormatter
    {
        #region Properties

        private static readonly Dictionary<string, Func<LogEntry, string>> simpleTokenHandlers = new Dictionary<string, Func<LogEntry, string>>();
        private static readonly Dictionary<string, Func<LogEntry, string, string>> dictionaryTokenHandlers = new Dictionary<string, Func<LogEntry, string, string>>();
        private static Regex s_regexExtendedProperties = new Regex(@"{extendedProperties\(([^{]*{([^}]*)}(\??)?[^{]*{value}[^}]*)\)}", RegexOptions.Compiled);
        private static Regex s_regexCreateTime = new Regex(@"{createTime(\(([^}]*)\))?}", RegexOptions.Compiled);

        // it will check for html encoded tags to detetect
        private static Regex s_regexLookForHtmlEncodedTags = new Regex("&lt;.+&gt;", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
        private static bool _isHtmlEncoded;

        private static readonly CultureInfo s_cultureInfo = new CultureInfo("en-US");
        private string _template;

        public LogFormatter(string template)
        {
            //_template = HttpUtility.HtmlDecode(value);
            _template = template;

            _isHtmlEncoded = false;
            if (template != null)
            {
                _isHtmlEncoded = s_regexLookForHtmlEncodedTags.IsMatch(template);
            }
        }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        public string Template
        {
            get { return _template; }
            set
            {
                //_template = HttpUtility.HtmlDecode(value);
                _template = value;

                _isHtmlEncoded = false;
                if (value != null)
                {
                    _isHtmlEncoded = s_regexLookForHtmlEncodedTags.IsMatch(value);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="LogFormatter"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        static LogFormatter()
        {
            simpleTokenHandlers["message"] = (l) => l.Message;
            //simpleTokenHandlers["userDisplayName"] = (l) => l.UserDisplayName;
            //simpleTokenHandlers["applicationId"] = (l) => l.ApplicationId.ToString();
            //simpleTokenHandlers["userCommonId"] = (l) => l.UserCommonId.ToString();
            //simpleTokenHandlers["machineName"] = (l) => l.MachineName;
            //simpleTokenHandlers["userName"] = (l) => l.UserName;
            //simpleTokenHandlers["userIPAddress"] = (l) => l.UserIPAddress;
            //simpleTokenHandlers["userDisrupted"] = (l) => l.IsUserDisrupted.ToString();
            //simpleTokenHandlers["userAgentBrowser"] = (l) => l.UserAgentBrowser;
            //simpleTokenHandlers["url"] = (l) => l.Url;
            //simpleTokenHandlers["referrer"] = (l) => l.Referrer;
            //simpleTokenHandlers["machineIPAddress"] = (l) => l.MachineIPAddress;
            //simpleTokenHandlers["requestMethod"] = (l) => l.RequestMethod;
            //simpleTokenHandlers["affectedSystem"] = (l) => l.AffectedSystem;
            //simpleTokenHandlers["level"] = (l) => l.Level.ToString();
            //simpleTokenHandlers["userScreenName"] = (l) => l.UserScreenName;
            simpleTokenHandlers["exception"] = (l) => l.Exception != null ? l.Exception.ToString() : "";
            simpleTokenHandlers["newLine"] = (l) => System.Environment.NewLine;
            //simpleTokenHandlers["category"] = (l) => l.CategoryId.ToString(CultureInfo.CurrentCulture);
            //simpleTokenHandlers["environment"] = (l) => Environment.Current.ToString();

            dictionaryTokenHandlers["extendedProperties"] = (l, t) => ExtendedPropertiesHandler(l.ExtendedProperties, t);
            //dictionaryTokenHandlers["createTime"] = (l, t) => FormattedDateTimeHandler(l.CreateTime, t);

            simpleTokenHandlers["className"] = (l) => FormatLocationInfo(l, "ClassName");
            simpleTokenHandlers["fileName"] = (l) => FormatLocationInfo(l, "FileName");
            simpleTokenHandlers["lineNumber"] = (l) => FormatLocationInfo(l, "LineNumber");
            simpleTokenHandlers["methodName"] = (l) => FormatLocationInfo(l, "MethodName");
            simpleTokenHandlers["fullInfo"] = (l) => FormatLocationInfo(l, "FullInfo");
            simpleTokenHandlers["threadName"] = (l) => FormatLocationInfo(l, "ThreadName");
            simpleTokenHandlers["threadId"] = (l) => FormatLocationInfo(l, "ThreadId");


        }

        #endregion

        #region Private Methods
        private static string FormatLocationInfo(LogEntry logEntry, string property)
        {
            //LocationInfo stackTraceInfo;
            //if (logEntry.ExtendedProperties.ContainsKey(property) == false)
            //{
            //    Stopwatch sw = new Stopwatch();
            //    sw.Start();
            //    stackTraceInfo = new LocationInfo();
            //    sw.Stop();

            //    logEntry.ExtendedProperties["ClassName"] = stackTraceInfo.ClassName;
            //    logEntry.ExtendedProperties["FileName"] = stackTraceInfo.FileName;
            //    logEntry.ExtendedProperties["LineNumber"] = stackTraceInfo.LineNumber;
            //    logEntry.ExtendedProperties["MethodName"] = stackTraceInfo.MethodName;
            //    logEntry.ExtendedProperties["FullInfo"] = stackTraceInfo.FullInfo;
            //}

            if (logEntry.ExtendedProperties.ContainsKey(property))
            {
                return logEntry.ExtendedProperties[property];
            }
            else
            {
                return "N/A";
            }
        }

        private static string FormattedDateTimeHandler(DateTime dateToFormat, string template)
        {
            MatchCollection matches = s_regexCreateTime.Matches(template);

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    string replacement;
                    if (String.IsNullOrEmpty(match.Groups[2].Value))
                    {
                        replacement = dateToFormat.ToString("f", s_cultureInfo);
                    }
                    else
                    {
                        replacement = dateToFormat.ToString(match.Groups[2].Value, s_cultureInfo);
                    }
                    template = s_regexCreateTime.Replace(template, replacement, 1);
                }
            }

            return template;
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        private static string ExtendedPropertiesHandler(IDictionary<string, string> extendedProperties, string template)
        {
            MatchCollection matches = s_regexExtendedProperties.Matches(template);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    string keyName = match.Groups[2].Value;

                    if (keyName.Equals("key") == false)
                    {
                        string valueToUse = "-Not defined-";
                        if (extendedProperties.ContainsKey(keyName))
                        {
                            valueToUse = extendedProperties[keyName];
                        }

                        if (_isHtmlEncoded)
                        {
                            //valueToUse = HttpUtility.HtmlEncode(valueToUse);
                        }

                        // to avoid a situaltions where using dollar sign inadvertently is used
                        // dollar sign ($) is a special character used to insert the regex match in the replacement string
                        valueToUse = valueToUse.Replace("$", "$$");

                        string replacement;
                        if (String.IsNullOrEmpty(match.Groups[3].Value))
                        {
                            replacement = match.Groups[1].Value.Replace("{" + keyName + "}", keyName).Replace("{value}", valueToUse);
                        }
                        else // we have a ? after the key which indicates that the key should not be displayed
                        {
                            replacement = match.Groups[1].Value.Replace("{" + keyName + "}?", String.Empty).Replace("{value}", valueToUse);

                        }

                        replacement = replacement.Replace("$", "$$");

                        template = s_regexExtendedProperties.Replace(template, replacement, 1);

                    }
                    else
                    {
                        string replacement;
                        if (String.IsNullOrEmpty(match.Groups[3].Value))
                        {
                            replacement = match.Groups[1].Value.Replace("{key}", "{0}").Replace("{value}", "{1}");
                        }
                        else
                        {
                            replacement = match.Groups[1].Value.Replace("{key}?", String.Empty).Replace("{value}", "{1}");
                        }

                        StringBuilder sb = new StringBuilder();
                        foreach (KeyValuePair<string, string> property in extendedProperties)
                        {
                            string value = property.Value;
                            if (_isHtmlEncoded)
                            {
                                //value = HttpUtility.HtmlEncode(value);
                            }

                            sb.Append(string.Format(replacement + System.Environment.NewLine, property.Key, value));
                        }

                        sb.Replace("$", "$$");
                        template = s_regexExtendedProperties.Replace(template, sb.ToString(), 1);
                    }
                }
            }

            return template;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Formats the specified log entry.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <returns>Return the template formatted to the message.</returns>
        public string Format(LogEntry logEntry)
        {
            StringBuilder sb = new StringBuilder(Template);

            foreach (var tokenHandler in simpleTokenHandlers)
                sb.Replace("{" + tokenHandler.Key + "}", tokenHandler.Value(logEntry));

            string toReturn = sb.ToString();

            foreach (var tokenHandler in dictionaryTokenHandlers)
            {
                toReturn = tokenHandler.Value(logEntry, toReturn);
            }

            return toReturn;
        }

        #endregion
    }
}