using Rock.Immutable;

namespace Rock.Logging
{
    public static class DefaultFileLogFormatter
    {
        private const string _defaultFileTemplate = "----------------------------------------------------------------------------------------------------{newLine}LOG INFO{newLine}{newLine}Message: {message}{newLine}Create Time: {createTime(O)}{newLine}Type of Message: {level} {newLine}Environment: {environment}{newLine}Application ID: {applicationId} {newLine}Application User ID: {applicationUserId} {newLine}Machine Name: {machineName}{newLine}{newLine}EXTENDED PROPERTY INFO{newLine}{newLine}{extendedProperties({key}: {value})}{newLine}EXCEPTION INFO{newLine}{newLine}Exception Type: {exceptionType}{newLine}Exception Context: {exceptionContext}{newLine}{newLine}{exception}";

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
            return new TemplateLogFormatter(_defaultFileTemplate);
        }
    }
}