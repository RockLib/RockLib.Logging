using System;
using Rock.StringFormatting;

namespace Rock.Logging
{
    /// <summary>
    /// Defines a <see cref="SetException"/> extension method.
    /// </summary>
    public static class SetExceptionExtensionMethod
    {
        /// <summary>
        /// Sets the value of <see cref="ILogEntry.ExceptionDetails"/> to a string representation
        /// of the provided exception. Also sets various extended properties
        /// (inner exception messages and items in <see cref="Exception.Data"/>).
        /// </summary>
        /// <param name="source">The instance of <see cref="ILogEntry"/> to set exception details for.</param>
        /// <param name="exception">The exception to be added to this instance of <see cref="ILogEntry"/>.</param>
        /// <param name="exceptionContext">Contextual information about the exception. This value
        /// should give a developer additional information to help debug or fix the issue.</param>
        /// <example>
        /// <code>
        /// ILogger logger = LoggerFactory.GetInstance();
        /// 
        /// public void ProcessLoan(int loanNumber)
        /// {
        ///   try
        ///   {
        ///     LogEntry entry = new LogEntry("Doing Something");
        ///     entry.ExtendedProperties.Add("LoanNumber", loanNumber);
        /// 
        ///     DoSomething(loanNumber);
        /// 
        ///     if(logger.IsInfoEnabled)
        ///     {
        ///       logger.Info(entry);
        ///     }
        ///   }
        ///   catch (Exception ex)
        ///   {
        ///     if (logger.IsErrorEnabled)
        ///     {
        ///       entry.SetException(ex);
        ///       logger.Error(entry);
        ///     }
        ///   }
        /// }
        /// </code>
        /// </example>
        public static void SetException(this ILogEntry source, Exception exception, string exceptionContext = null)
        {
            if (exception != null)
            {
                source.ExceptionType = exception.GetType().ToString();
                source.ExceptionDetails = exception.FormatToString();
                source.ExceptionContext = exceptionContext;
                source.SetExtendedPropertiesFor(exception);
            }
        }

        private static void SetExtendedPropertiesFor(this ILogEntry logEntry, Exception ex)
        {
            var innerLevel = 0;

            while (ex != null)
            {
                var exceptionKey =
                    innerLevel != 0
                        ? string.Format("Inner Exception {0}: {1}", innerLevel, ex.GetType())
                        : ex.GetType().ToString();

                if (!logEntry.ExtendedProperties.ContainsKey(exceptionKey))
                {
                    logEntry.ExtendedProperties.Add(exceptionKey, ex.Message);
                }

                foreach (var dataKey in ex.Data.Keys)
                {
                    if (dataKey == null) { continue; }

                    var dataKeyString = "((" + ex.GetType() + ")Exception.Data)[\"" + dataKey + "\"]";

                    if (logEntry.ExtendedProperties.ContainsKey(dataKeyString)) { continue; }

                    var dataValue = ex.Data[dataKey];

                    var dataValueString =
                        dataValue == null
                            ? null
                            : dataValue.ToString();

                    logEntry.ExtendedProperties.Add(dataKeyString, dataValueString);
                }

                innerLevel++;
                ex = ex.InnerException;
            }
        }
    }
}
