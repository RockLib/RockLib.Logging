using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using Rock.Reflection;

namespace Rock.Logging
{
    internal static class FormatToStringExtension
    {
        private static readonly string[] _skipProperties = { "InnerException", "Message", "HelpLink", "Data", "StackTrace", "TargetSite", "Source" };

        private static readonly ConcurrentDictionary<Type, Func<Exception, string>> _formatExceptionFuncs =
            new ConcurrentDictionary<Type, Func<Exception, string>>();

        public static string FormatToString(this Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            var formatException =
                _formatExceptionFuncs.GetOrAdd(
                    exception.GetType(),
                    exceptionType =>
                    {
                        var appendPropertyValueFuncs =
                            exceptionType.GetProperties()
                                .Where(p => !_skipProperties.Contains(p.Name))
                                .Select(GetAppendPropertyValueFunc)
                                .ToList();

                        return
                            ex =>
                            {
                                var sb = new StringBuilder();

                                sb.AppendLine("Properties:");

                                appendPropertyValueFuncs
                                    .Aggregate(
                                        sb,
                                        (stringBuilder, appendPropertyValue) =>
                                            appendPropertyValue(stringBuilder, ex));

                                AppendStandardExceptionData(sb, ex);

                                return sb.ToString();
                            };

                    });

            return formatException(exception);
        }

        private static Func<StringBuilder, Exception, StringBuilder> GetAppendPropertyValueFunc(PropertyInfo property)
        {
            var getPropertyValue = property.GetGetFunc();
            var propertyValueBeginning = "\t" + property.Name + ": ";

            return
                (sb, loggedException) =>
                {
                    string value;
                    try
                    {
                        var propertyValue = getPropertyValue(loggedException);
                        value =
                            propertyValue != null
                                ? propertyValue.ToString()
                                : "null";
                    }
                    catch (Exception ex)
                    {
                        value = ex.Message;
                    }

                    return sb.Append(propertyValueBeginning).AppendLine(value);
                };
        }

        private static void AppendStandardExceptionData(StringBuilder sb, Exception ex)
        {
            sb.AppendLine("Type: " + ex.GetType());

            if (ex.Source != null)
            {
                sb.AppendLine("Source: " + ex.Source);
            }

            sb.AppendLine("Message: \"" + ex.Message + "\"");

            if (ex.StackTrace != null)
            {
                sb.AppendLine("Stack Trace: " + ex.StackTrace);
            }

            if (ex.Data.Count > 0)
            {
                sb.AppendLine("Exception Data:");
                foreach (DictionaryEntry data in ex.Data)
                {
                    sb.AppendLine(String.Concat("\t", data.Key, " - ", data.Value));
                }
            }

            if (ex.InnerException != null)
            {
                sb.AppendLine(String.Concat("\tInnerException", ex.InnerException.FormatToString()));
            }
        }
    }
}