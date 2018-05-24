using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace RockLib.Logging
{
    /// <summary>
    /// An implementation of the <see cref="ILogFormatter"/> interface that uses a template
    /// with replacement tokens to format a log entry.
    /// </summary>
    public class TemplateLogFormatter : ILogFormatter
    {
        private static readonly Dictionary<string, Func<LogEntry, string>> _simpleTokenHandlers = new Dictionary<string, Func<LogEntry, string>>();
        private static readonly Dictionary<string, Func<LogEntry, DateTime>> _dateTimeTokenHandlers = new Dictionary<string, Func<LogEntry, DateTime>>();

        private static readonly Regex _simpleTokenRegex = new Regex(@"{(?<token>[a-zA-Z_][a-zA-Z0-9_]*?)}", RegexOptions.Compiled);
        private static readonly Regex _parentheticalTokenRegex = new Regex(@"{(?<token>[a-zA-Z_][a-zA-Z0-9_]*?)(?:\((?<format>.*?)\))?}", RegexOptions.Compiled);

        private static readonly Regex _extendedPropertiesRegex =
            new Regex(@"
        {                                       # Opening curly brace
            extendedProperties                  # extendedProperties
                \(                              # Opening parenthesis
                    (?<before>(?:[^{]|{{)*)     # Text after the opening curly brace and parenthesis and before '{key}'
                    {(?<key>key|[^}]+)}         # The exact string, '{key}', or '{key_name}' where 'key_name' is the name of an extended property
                    (?<omitKey>\?)?             # The optional '?' after '{key}' (if present, omit the key name)
                    (?<between>(?:[^{]|{{)*)    # Text after '{key}' and before '{value}'
                    {value}                     # The exact string, '{value}'
                    (?<after>.*?)               # Text after '{value}' and before the closing parenthesis and curly brace
                \)                              # Closing parenthesis
        }                                       # Closing curly brace",
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex _containsHtmlTagsRegex = new Regex("&lt;.+?&gt;", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly CultureInfo _culture = new CultureInfo("en-US");

        private readonly bool _isHtmlEncoded;

        static TemplateLogFormatter()
        {
            AddSimpleTokenHandler("newLine", logEntry => Environment.NewLine);
            AddSimpleTokenHandler("tab", logEntry => "\t");

            AddSimpleTokenHandler("message", logEntry => logEntry.Message);
            AddSimpleTokenHandler("userName", logEntry => logEntry.UserName);
            AddSimpleTokenHandler("machineName", logEntry => logEntry.MachineName);
            AddSimpleTokenHandler("machineIpAddress", logEntry => logEntry.MachineIpAddress);
            AddSimpleTokenHandler("level", logEntry => logEntry.Level.ToString());
            AddSimpleTokenHandler("exception", logEntry => logEntry.GetExceptionData());
            AddSimpleTokenHandler("uniqueId", logEntry => logEntry.UniqueId);

            AddExtendedPropertyTokenHandler("callerInfo", "CallerInfo");

            AddDateTimeTokenHandler("createTime", logEntry => logEntry.CreateTime);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateLogFormatter"/> class.
        /// </summary>
        /// <param name="template">The template to use when formatting logs.</param>
        public TemplateLogFormatter(string template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            Template = WebUtility.HtmlDecode(template);

            _isHtmlEncoded =
                template != null
                && _containsHtmlTagsRegex.IsMatch(template);
        }

        /// <summary>
        /// Gets the template used for formatting logs.
        /// </summary>
        public string Template { get; }

        /// <summary>
        /// Add a token handler for a specific extended property.
        /// </summary>
        /// <param name="key">The key of the token.</param>
        /// <param name="extendedProperty">The extended property to retrieve.</param>
        public static void AddExtendedPropertyTokenHandler(string key, string extendedProperty)
        {
            AddSimpleTokenHandler(key, logEntry => FormatSpecificExtendedProperty(logEntry, extendedProperty));
        }

        /// <summary>
        /// Add a token handler for a log entry.
        /// </summary>
        /// <param name="key">The key of the token.</param>
        /// <param name="getValue">A function that returns the token value.</param>
        public static void AddSimpleTokenHandler(string key, Func<LogEntry, string> getValue)
        {
            _simpleTokenHandlers[key] = getValue;
        }

        private static void AddDateTimeTokenHandler(string key, Func<LogEntry, DateTime> getDateTime)
        {
            _dateTimeTokenHandlers[key] = getDateTime;
        }

        private static string FormatSpecificExtendedProperty(LogEntry logEntry, string extendedProperty)
        {
            return
                logEntry.ExtendedProperties.ContainsKey(extendedProperty)
                    ? ConvertToString(logEntry.ExtendedProperties[extendedProperty])
                    : "N/A";
        }

        /// <summary>
        /// Formats the specified log entry according to the <see cref="Template"/> property.
        /// </summary>
        /// <param name="logEntry">The log entry to format.</param>
        /// <returns>The formatted log entry.</returns>
        public string Format(LogEntry logEntry)
        {
            var formattedLogEntry = _simpleTokenRegex.Replace(
                Template,
                match =>
                {
                    Func<LogEntry, string> getValue;

                    return
                        HtmlEncodeIfNecessary(
                            _simpleTokenHandlers.TryGetValue(match.Groups["token"].Value, out getValue)
                                ? getValue(logEntry)
                                : match.Value);
                });

            formattedLogEntry = _parentheticalTokenRegex.Replace(
                formattedLogEntry,
                match =>
                {
                    Func<LogEntry, DateTime> getValue;

                    if (!_dateTimeTokenHandlers.TryGetValue(match.Groups["token"].Value, out getValue))
                    {
                        return match.Value;
                    }

                    var value = getValue(logEntry);

                    return
                        HtmlEncodeIfNecessary(
                            match.Groups["format"].Success
                                ? value.ToString(match.Groups["format"].Value, _culture)
                                : value.ToString("f", _culture));
                });

            formattedLogEntry =
                _extendedPropertiesRegex.Replace(
                    formattedLogEntry,
                    match =>
                    {
                        var before = match.Groups["before"].Value;
                        var between = match.Groups["between"].Value;
                        var after = match.Groups["after"].Value;

                        var key = match.Groups["key"].Value;
                        var omitKey = match.Groups["omitKey"].Success;

                        if (key == "key")
                        {
                            return
                                logEntry.ExtendedProperties
                                    .Aggregate(
                                        new StringBuilder(),
                                        (sb, kvp) => sb.Append(before).Append(omitKey ? null : kvp.Key).Append(between).Append(HtmlEncodeIfNecessary(ConvertToString(kvp.Value))).AppendLine(after))
                                    .ToString();
                        }

                        object value;

                        if (!logEntry.ExtendedProperties.TryGetValue(key, out value))
                        {
                            value = "N/A";
                        }

                        return HtmlEncodeIfNecessary(before + (omitKey ? null : key) + between + HtmlEncodeIfNecessary(ConvertToString(value)) + after);
                    });

            return formattedLogEntry;
        }

        private string HtmlEncodeIfNecessary(string value)
        {
            return _isHtmlEncoded ? WebUtility.HtmlEncode(value) : value;
        }

        private static string ConvertToString(object value)
        {
            if (value is string stringValue)
                return stringValue;
            return JsonConvert.SerializeObject(value);
        }
    }
}
