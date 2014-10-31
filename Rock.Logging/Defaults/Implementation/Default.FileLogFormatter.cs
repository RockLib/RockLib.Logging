using System;
using Rock.Defaults;

namespace Rock.Logging.Defaults.Implementation
{
    public static partial class Default
    {
        private const string _defaultFileTemplate = "----------------------------------------------------------------------------------------------------{newLine}LOG INFO{newLine}{newLine}Message: {message}{newLine}Create Time: {createTime(O)}{newLine}Type of Message: {level} {newLine}Environment: {environment}{newLine}Application ID: {applicationId} {newLine}Application User ID: {applicationUserId} {newLine}Machine Name: {machineName}{newLine}{newLine}EXTENDED PROPERTY INFO{newLine}{newLine}{extendedProperties({key}: {value})}{newLine}EXCEPTION INFO{newLine}{newLine}Exception Type: {exceptionType}{newLine}Exception Context: {exceptionContext}{newLine}{newLine}{exception}";

        private static readonly DefaultHelper<ILogFormatter> _fileLogFormatter = new DefaultHelper<ILogFormatter>(() => new TemplateLogFormatter(_defaultFileTemplate));

        public static ILogFormatter FileLogFormatter
        {
            get { return _fileLogFormatter.Current; }
        }

        public static ILogFormatter DefaultFileLogFormatter
        {
            get { return _fileLogFormatter.DefaultInstance; }
        }

        public static void SetFileLogFormatter(Func<ILogFormatter> getFileLogFormatterInstance)
        {
            _fileLogFormatter.SetCurrent(getFileLogFormatterInstance);
        }

        public static void RestoreFileLogFormatter()
        {
            _fileLogFormatter.RestoreDefault();
        }
    }
}
