using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Rock.Logging
{
    public class LogFormatter : ILogFormatter
    {
        private static readonly Dictionary<string, Func<LogEntry, string>> _simpleTokenHandlers = new Dictionary<string, Func<LogEntry, string>>();
        private static readonly Dictionary<string, Func<LogEntry, string, bool, string>> _dictionaryTokenHandlers = new Dictionary<string, Func<LogEntry, string, bool, string>>();

        private static readonly Regex _extendedPropertiesRegex = new Regex(@"{extendedProperties\(([^{]*{([^}]*)}(\??)?[^{]*{value}[^}]*)\)}", RegexOptions.Compiled);
        private static readonly Regex _createTimeRegex = new Regex(@"{createTime(\(([^}]*)\))?}", RegexOptions.Compiled);
        private static readonly Regex _containsHtmlTagsRegex = new Regex("&lt;.+?&gt;", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
        
        private static readonly CultureInfo _culture = new CultureInfo("en-US");
        
        private readonly bool _isHtmlEncoded;
        private readonly string _template;

        /// <summary>
        /// Initializes the <see cref="LogFormatter"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        static LogFormatter()
        {
            AddSimpleTokenHandler("message", logEntry => logEntry.Message);
            //AddSimpleTokenHandler("userDisplayName", logEntry => logEntry.UserDisplayName);
            AddSimpleTokenHandler("applicationId", logEntry => logEntry.ApplicationId);
            //AddSimpleTokenHandler("userCommonId", logEntry => logEntry.UserCommonId.ToString());
            //AddSimpleTokenHandler("machineName", logEntry => logEntry.MachineName);
            //AddSimpleTokenHandler("userName", logEntry => logEntry.UserName);
            //AddSimpleTokenHandler("userIPAddress", logEntry => logEntry.UserIPAddress);
            //AddSimpleTokenHandler("userDisrupted", logEntry => logEntry.IsUserDisrupted.ToString());
            //AddSimpleTokenHandler("userAgentBrowser", logEntry => logEntry.UserAgentBrowser);
            //AddSimpleTokenHandler("url", logEntry => logEntry.Url);
            //AddSimpleTokenHandler("referrer", logEntry => logEntry.Referrer);
            //AddSimpleTokenHandler("machineIPAddress", logEntry => logEntry.MachineIPAddress);
            //AddSimpleTokenHandler("requestMethod", logEntry => logEntry.RequestMethod);
            //AddSimpleTokenHandler("affectedSystem", logEntry => logEntry.AffectedSystem);
            //AddSimpleTokenHandler("level", logEntry => logEntry.Level.ToString());
            //AddSimpleTokenHandler("userScreenName", logEntry => logEntry.UserScreenName);
            AddSimpleTokenHandler("exception", logEntry => logEntry.ExceptionDetails);
            AddSimpleTokenHandler("newLine", logEntry => Environment.NewLine);
            //AddSimpleTokenHandler("category", logEntry => logEntry.CategoryId.ToString(CultureInfo.CurrentCulture));
            //AddSimpleTokenHandler("environment", logEntry => Environment.Current.ToString());

            AddComplexTokenHandler("extendedProperties", (logEntry, template, isHtmlEncoded) => ExtendedPropertiesHandler(logEntry.ExtendedProperties, template, isHtmlEncoded));
            //AddComplexTokenHandler("createTime", (logEntry, template, isHtmlEncoded) => FormattedDateTimeHandler(logEntry.CreateTime, template);

            //AddSimpleTokenHandler("className", logEntry => FormatLocationInfo(logEntry, "ClassName"));
            AddSimpleTokenHandler("fileName", logEntry => FormatLocationInfo(logEntry, "FileName"));
            AddSimpleTokenHandler("lineNumber", logEntry => FormatLocationInfo(logEntry, "LineNumber"));
            AddSimpleTokenHandler("methodName", logEntry => FormatLocationInfo(logEntry, "MethodName"));
            //AddSimpleTokenHandler("fullInfo", logEntry => FormatLocationInfo(logEntry, "FullInfo"));
            //AddSimpleTokenHandler("threadName", logEntry => FormatLocationInfo(logEntry, "ThreadName"));
            //AddSimpleTokenHandler("threadId", logEntry => FormatLocationInfo(logEntry, "ThreadId"));
        }

        public LogFormatter(string template)
        {
            _template = WebUtility.HtmlDecode(template);

            _isHtmlEncoded =
                template != null
                && _containsHtmlTagsRegex.IsMatch(template);
        }

        public static void AddSimpleTokenHandler(string key, Func<LogEntry, string> getTokenReplacement)
        {
            _simpleTokenHandlers[key] = getTokenReplacement;
        }

        public static void AddSimpleTokenHandler<TLogEntry>(string key, Func<TLogEntry, string> getTokenReplacement)
            where TLogEntry : LogEntry
        {
            _simpleTokenHandlers[key] =
                logEntry =>
                {
                    var tLogEntry = logEntry as TLogEntry;
                    return tLogEntry == null ? null : getTokenReplacement(tLogEntry);
                };
        }

        public static void AddComplexTokenHandler(string key, Func<LogEntry, string, bool, string> getTokenReplacement)
        {
            _dictionaryTokenHandlers[key] = getTokenReplacement;
        }

        public static void AddComplexTokenHandler<TLogEntry>(string key, Func<TLogEntry, string, bool, string> getTokenReplacement)
            where TLogEntry : LogEntry
        {
            _dictionaryTokenHandlers[key] =
                (logEntry, template, isHtmlEncoded) =>
                {
                    var tLogEntry = logEntry as TLogEntry;
                    return tLogEntry == null ? null : getTokenReplacement(tLogEntry, template, isHtmlEncoded);
                };
        }

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
            MatchCollection matches = _createTimeRegex.Matches(template);

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    string replacement;
                    if (String.IsNullOrEmpty(match.Groups[2].Value))
                    {
                        replacement = dateToFormat.ToString("f", _culture);
                    }
                    else
                    {
                        replacement = dateToFormat.ToString(match.Groups[2].Value, _culture);
                    }
                    template = _createTimeRegex.Replace(template, replacement, 1);
                }
            }

            return template;
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        private static string ExtendedPropertiesHandler(IDictionary<string, string> extendedProperties, string template, bool isHtmlEncoded)
        {
            MatchCollection matches = _extendedPropertiesRegex.Matches(template);
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

                        if (isHtmlEncoded)
                        {
                            valueToUse = WebUtility.HtmlEncode(valueToUse);
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

                        template = _extendedPropertiesRegex.Replace(template, replacement, 1);
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

                        var sb = new StringBuilder();
                        foreach (var property in extendedProperties)
                        {
                            var value = property.Value;
                            if (isHtmlEncoded)
                            {
                                value = WebUtility.HtmlEncode(value);
                            }

                            sb.Append(string.Format(replacement + System.Environment.NewLine, property.Key, value));
                        }

                        sb.Replace("$", "$$");
                        template = _extendedPropertiesRegex.Replace(template, sb.ToString(), 1);
                    }
                }
            }

            return template;
        }

        /// <summary>
        /// Formats the specified log entry.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <returns>Return the template formatted to the message.</returns>
        public string Format(LogEntry logEntry)
        {
            StringBuilder sb = new StringBuilder(_template);

            foreach (var tokenHandler in _simpleTokenHandlers)
                sb.Replace("{" + tokenHandler.Key + "}", tokenHandler.Value(logEntry));

            string toReturn = sb.ToString();

            foreach (var tokenHandler in _dictionaryTokenHandlers)
            {
                toReturn = tokenHandler.Value(logEntry, toReturn, _isHtmlEncoded);
            }

            return toReturn;
        }
    }
}