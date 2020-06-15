#if !NET451
using Microsoft.Extensions.DependencyInjection;
using System;

namespace RockLib.Logging.DependencyInjection
{
    public abstract class FormattableLogProviderOptions : LogProviderOptions
    {
        public LogFormatterRegistration FormatterFactory { get; set; }

        public void SetTemplate(string template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            SetFormatter<TemplateLogFormatter>(template);
        }

        public void SetFormatter(ILogFormatter formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            FormatterFactory = serviceProvider => formatter;
        }

        public void SetFormatter<TLogFormatter>(params object[] logFormatterParameters)
            where TLogFormatter : ILogFormatter
        {
            FormatterFactory = serviceProvider => ActivatorUtilities.CreateInstance<TLogFormatter>(serviceProvider, logFormatterParameters);
        }
    }
}
#endif
