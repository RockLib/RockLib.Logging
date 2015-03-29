using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Rock.Conversion;
using Rock.StringFormatting;

namespace Rock.Logging
{
    /// <summary>
    /// Represents a single logging data point.
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = XmlNamespace)]
    [DataContractAttribute(Namespace = XmlNamespace)]
    public class LogEntry
    {
        /// <summary>
        /// The namespace that should be used for xml documents that describe a log entry.
        /// </summary>
        public const string XmlNamespace = "http://rockframework.org/logging";

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        public LogEntry()
            : this(null, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="message">The log entry's message.</param>
        /// <example>
        /// <code>
        /// LogEntry entry = new LogEntry("Some message");
        /// </code>
        /// </example>
        public LogEntry(string message)
            : this(message, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="message">The log entry's message.</param>
        /// <param name="exception">The log entry's exception.</param>
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
        ///     DoSomething(loanNumber);
        ///   }
        ///   catch (Exception ex)
        ///   {
        ///     if (logger.IsErrorEnabled())
        ///     {
        ///       LogEntry entry = new LogEntry("Error doing something", ex);
        ///       entry.ExtendedProperties.Add("LoanNumber", loanNumber);
        ///       logger.Error(entry);
        ///     }
        ///   }
        /// }
        /// </code>
        /// </example>
        public LogEntry(
            string message,
            Exception exception,
            string exceptionContext = null)
            : this(message, null, exception, exceptionContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LogEntry class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="extendedProperties">The extended properties as an anonymous type.</param>
        /// <example>
        /// <code>
        /// LogEntry entry = new LogEntry("Hello, world!", new { Foo = "abc", Bar = 123});
        /// </code>
        /// </example>
        public LogEntry(string message, object extendedProperties)
            : this(
                message,
                extendedProperties.ToDictionaryOfStringToString(),
                null,
                null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LogEntry class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="extendedProperties">The extended properties as anonymous type.</param>
        /// <param name="exception">The exception.</param>
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
        ///     DoSomething(loanNumber);
        ///   }
        ///   catch (Exception ex)
        ///   {
        ///     if (logger.IsErrorEnabled())
        ///     {
        ///       LogEntry entry = new LogEntry("Error doing something", new { LoanNumber = loanNumber }, ex);
        ///       logger.Error(entry);
        ///     }
        ///   }
        /// }
        /// </code>
        /// </example>
        public LogEntry(
            string message,
            object extendedProperties,
            Exception exception,
            string exceptionContext = null)
            : this(
                message,
                extendedProperties.ToDictionaryOfStringToString(),
                exception,
                exceptionContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LogEntry class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="extendedProperties">The extended properties.</param>
        /// <example>
        /// <code>
        /// IDictionary&lt;string, string&gt; extendedProperties =
        ///   new  Dictionary&lt;string, string&gt;();
        /// extendedProperties.Add("Foo", "abc");
        /// extendedProperties.Add("Bar", "123");
        /// LogEntry entry = new LogEntry("Hello, world!", extendedProperties);
        /// </code>
        /// </example>
        public LogEntry(string message, IDictionary<string, string> extendedProperties)
            : this(message, extendedProperties, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LogEntry class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="extendedProperties">The extended properties.</param>
        /// <param name="exception">The exception.</param>
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
        ///     DoSomething(loanNumber);
        ///   }
        ///   catch (Exception ex)
        ///   {
        ///     if (logger.IsErrorEnabled())
        ///     {
        ///       IDictionary&lt;string, string&gt; extendedProperties =
        ///         new  Dictionary&lt;string, string&gt;();
        ///       extendedProperties.Add("LoanNumber", loanNumber.ToString());
        ///       LogEntry entry = new LogEntry("Error doing something", extendedProperties, ex);
        ///       logger.Error(entry);
        ///     }
        ///   }
        /// }
        /// </code>
        /// </example>
        public LogEntry(
            string message,
            IDictionary<string, string> extendedProperties,
            Exception exception,
            string exceptionContext = null)
        {
            Message = message;
            ExtendedProperties = new LogEntryExtendedProperties(extendedProperties);
            SetException(exception, exceptionContext);

            MachineName = System.Environment.MachineName;
            ApplicationUserId = System.Environment.UserName;
            CreateTime = DateTime.UtcNow;

            UniqueId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets or sets the message that needs to be logged.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        [DataMember]
        public string ApplicationId { get; set; }

        /// <summary>
        /// The ID of the account that is running the application. By default, this is set to <see cref="System.Environment.UserName"/>.
        /// </summary>
        [DataMember]
        public string ApplicationUserId { get; set; }

        /// <summary>
        /// Gets or sets the time when the entry was created. This is automatically set when a new LogEntry is initialized.
        /// </summary>
        [DataMember]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Gets or sets the environment (e.g. Test or Prod) in which the log entry was created.
        /// </summary>
        [DataMember]
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the name of the machine name the log entry was created on.
        /// </summary>
        [DataMember]
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the log level of the log entry (e.g. Debug or Error).
        /// </summary>
        [DataMember]
        public LogLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the details of an exception.
        /// </summary>
        [DataMember]
        public string ExceptionDetails { get; set; }

        /// <summary>
        /// Gets or sets arbitrary contextual information related to a thrown exception. This value
        /// should give a developer additional information to help debug or fix the issue.
        /// </summary>
        [DataMember]
        public string ExceptionContext { get; set; }

        /// <summary>
        /// Gets or sets the type of the exception. While the value of this property can be
        /// arbitrary, when <see cref="SetException"/> is called, its value will be set to 
        /// the full name of the type of the exception.
        /// </summary>
        /// <remarks>
        /// When this log entry is processed (e.g. a web service), this value of this property
        /// can be used for a variety of purposes, such as tagging/labelling or indexing.
        /// </remarks>
        [DataMember]
        public string ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the extended properties. This property is used to add any additional 
        /// information into the log entry.
        /// </summary>
        [DataMember]
        public LogEntryExtendedProperties ExtendedProperties { get; set; }

        /// <summary>
        /// Gets or sets an arbitrary unique identifier for the log entry. Its default value is
        /// a string representation of a new GUID. This value allows a log entry to be identified
        /// on the client-side. For example, a link to this log entry can be generated, client-side,
        /// before the log entry is added to a database. It is assumed that a database will index
        /// this value.
        /// </summary>
        [DataMember]
        public string UniqueId { get; set; }

        /// <summary>
        /// Gets a hash of this instance of <see cref="LogEntry"/> for the purpose of
        /// throttling log entries. If the value returned from this log entry is equal
        /// to the value from another log entry, then they are considered duplicates.
        /// Depending on the throttling configuration of a logger, duplicate log etries
        /// may or may not be sent its log providers.
        /// </summary>
        /// <returns>A hash code to be used for throttling purposes.</returns>
        public virtual int GetThrottlingHashCode()
        {
            unchecked
            {
                // TODO: determine exactly which properties should be included in the throttling hash code.
                var key = Level.GetHashCode();
                key = (key * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                key = (key * 397) ^ (ExceptionType != null ? ExceptionType.GetHashCode() : 0);
                key = (key * 397) ^ (ExceptionDetails != null ? ExceptionDetails.GetHashCode() : 0);
                key = (key * 397) ^ (MachineName != null ? MachineName.GetHashCode() : 0);
                key = (key * 397) ^ (Environment != null ? Environment.GetHashCode() : 0);
                return key;
            }
        }

        /// <summary>
        /// Sets the value of <see cref="ExceptionDetails"/> to a string representation
        /// of the provided exception. Also sets various extended properties
        /// (inner exception messages and items in <see cref="Exception.Data"/>).
        /// </summary>
        /// <param name="exception">The exception to be added to this instance of <see cref="LogEntry"/>.</param>
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
        public void SetException(Exception exception, string exceptionContext = null)
        {
            if (exception != null)
            {
                ExceptionType = exception.GetType().ToString();
                ExceptionDetails = exception.FormatToString();
                ExceptionContext = exceptionContext;
                SetExtendedPropertiesFor(exception);
            }
        }

        private void SetExtendedPropertiesFor(Exception ex)
        {
            var innerLevel = 0;

            while (ex != null)
            {
                var exceptionKey =
                    innerLevel != 0
                        ? string.Format("Inner Exception {0}: {1}", innerLevel, ex.GetType())
                        : ex.GetType().ToString();

                if (!ExtendedProperties.ContainsKey(exceptionKey))
                {
                    ExtendedProperties.Add(exceptionKey, ex.Message);
                }

                foreach (var dataKey in ex.Data.Keys)
                {
                    if (dataKey == null) { continue; }
                    
                    var dataKeyString = "((" + ex.GetType() + ")Exception.Data)[\"" + dataKey + "\"]";
                        
                    if (ExtendedProperties.ContainsKey(dataKeyString)) { continue; }
                        
                    var dataValue = ex.Data[dataKey];

                    var dataValueString =
                        dataValue == null
                            ? null
                            : dataValue.ToString();

                    ExtendedProperties.Add(dataKeyString, dataValueString);
                }

                innerLevel++;
                ex = ex.InnerException;
            }
        }
    }
}