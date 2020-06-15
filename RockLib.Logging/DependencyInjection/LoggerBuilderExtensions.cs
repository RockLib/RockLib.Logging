#if !NET451
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace RockLib.Logging.DependencyInjection
{
    public static class LoggerBuilderExtensions
    {
        public static ILoggerBuilder AddLogProvider<TLogProvider>(this ILoggerBuilder builder, params object[] parameters)
            where TLogProvider : ILogProvider =>
            builder.AddLogProvider(serviceProvider => ActivatorUtilities.CreateInstance<TLogProvider>(serviceProvider, parameters));

        public static ILoggerBuilder AddContextProvider<TContextProvider>(this ILoggerBuilder builder, params object[] parameters)
            where TContextProvider : IContextProvider =>
            builder.AddContextProvider(serviceProvider => ActivatorUtilities.CreateInstance<TContextProvider>(serviceProvider, parameters));

        #region ConsoleLogProvider

        public static ILoggerBuilder AddConsoleLogProvider(this ILoggerBuilder builder,
            string template = ConsoleLogProvider.DefaultTemplate,
            LogLevel? level = null,
            ConsoleLogProvider.Output? output = null,
            TimeSpan? timeout = null)
        {
            if (template is null)
                throw new ArgumentNullException(nameof(template));

            return builder.AddConsoleLogProvider(new TemplateLogFormatter(template), level, output, timeout);
        }

        public static ILoggerBuilder AddConsoleLogProvider(this ILoggerBuilder builder,
            ILogFormatter formatter,
            LogLevel? level = null,
            ConsoleLogProvider.Output? output = null,
            TimeSpan? timeout = null)
        {
            if (formatter is null)
                throw new ArgumentNullException(nameof(formatter));

            return builder.AddConsoleLogProvider(serviceProvider => formatter, level, output, timeout);
        }

        public static ILoggerBuilder AddConsoleLogProvider<TLogFormatter>(this ILoggerBuilder builder,
            LogLevel? level = null,
            ConsoleLogProvider.Output? output = null,
            TimeSpan? timeout = null,
            params object[] logFormatterParameters)
            where TLogFormatter : ILogFormatter
        {
            return builder.AddConsoleLogProvider(serviceProvider =>
                ActivatorUtilities.CreateInstance<TLogFormatter>(serviceProvider, logFormatterParameters),
                level, output, timeout);
        }

        public static ILoggerBuilder AddConsoleLogProvider(this ILoggerBuilder builder,
            LogFormatterRegistration formatterFactory,
            LogLevel? level = null,
            ConsoleLogProvider.Output? output = null,
            TimeSpan? timeout = null)
        {
            if (formatterFactory is null)
                throw new ArgumentNullException(nameof(formatterFactory));

            return builder.AddConsoleLogProvider(options =>
            {
                options.FormatterFactory = formatterFactory;
                if (level != null) options.Level = level.Value;
                if (output != null) options.Output = output.Value;
                if (timeout != null) options.Timeout = timeout;
            });
        }

        public static ILoggerBuilder AddConsoleLogProvider(this ILoggerBuilder builder,
            Action<ConsoleLogProviderOptions> configureOptions)
        {
            return builder.AddLogProvider(serviceProvider =>
            {
                var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<ConsoleLogProviderOptions>>();
                var options = optionsMonitor?.Get(builder.LoggerName) ?? new ConsoleLogProviderOptions();
                configureOptions?.Invoke(options);

                var formatter = options.FormatterFactory?.Invoke(serviceProvider)
                    ?? new TemplateLogFormatter(ConsoleLogProvider.DefaultTemplate);

                return new ConsoleLogProvider(formatter, options.Level, options.Output, options.Timeout);
            });
        }

        #endregion
        #region FileLogProvider

        public static ILoggerBuilder AddFileLogProvider(this ILoggerBuilder builder,
            string template = FileLogProvider.DefaultTemplate,
            string file = null,
            LogLevel? level = null,
            TimeSpan? timeout = null)
        {
            if (template is null)
                throw new ArgumentNullException(nameof(template));

            return builder.AddFileLogProvider(new TemplateLogFormatter(template), file, level, timeout);
        }

        public static ILoggerBuilder AddFileLogProvider(this ILoggerBuilder builder,
            ILogFormatter formatter,
            string file = null,
            LogLevel? level = null,
            TimeSpan? timeout = null)
        {
            if (formatter is null)
                throw new ArgumentNullException(nameof(formatter));

            return builder.AddFileLogProvider(serviceProvider => formatter, file, level, timeout);
        }

        public static ILoggerBuilder AddFileLogProvider<TLogFormatter>(this ILoggerBuilder builder,
            string file = null,
            LogLevel? level = null,
            TimeSpan? timeout = null,
            params object[] parameters)
            where TLogFormatter : ILogFormatter
        {
            return builder.AddFileLogProvider(serviceProvider =>
                ActivatorUtilities.CreateInstance<TLogFormatter>(serviceProvider, parameters),
                file, level, timeout);
        }

        public static ILoggerBuilder AddFileLogProvider(this ILoggerBuilder builder,
            LogFormatterRegistration formatterFactory,
            string file = null,
            LogLevel? level = null,
            TimeSpan? timeout = null)
        {
            if (formatterFactory is null)
                throw new ArgumentNullException(nameof(formatterFactory));

            return builder.AddFileLogProvider(options =>
            {
                options.FormatterFactory = formatterFactory;
                if (file != null) options.File = file;
                if (level != null) options.Level = level.Value;
                if (timeout != null) options.Timeout = timeout;
            });
        }

        public static ILoggerBuilder AddFileLogProvider(this ILoggerBuilder builder,
            Action<FileLogProviderOptions> configureOptions)
        {
            return builder.AddLogProvider(serviceProvider =>
            {
                var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<FileLogProviderOptions>>();
                var options = optionsMonitor?.Get(builder.LoggerName) ?? new FileLogProviderOptions();
                configureOptions?.Invoke(options);

                var formatter = options.FormatterFactory?.Invoke(serviceProvider)
                    ?? new TemplateLogFormatter(FileLogProvider.DefaultTemplate);

                return new FileLogProvider(options.File, formatter, options.Level, options.Timeout);
            });
        }

        #endregion
        #region RollingFileLogProvider

        public static ILoggerBuilder AddRollingFileLogProvider(this ILoggerBuilder builder,
            string template = FileLogProvider.DefaultTemplate,
            string file = null,
            LogLevel? level = null,
            TimeSpan? timeout = null,
            int? maxFileSizeKilobytes = null,
            int? maxArchiveCount = null,
            RolloverPeriod? rolloverPeriod = null)
        {
            if (template is null)
                throw new ArgumentNullException(nameof(template));

            return builder.AddRollingFileLogProvider(new TemplateLogFormatter(template),
                file, level, timeout, maxFileSizeKilobytes, maxArchiveCount, rolloverPeriod);
        }

        public static ILoggerBuilder AddRollingFileLogProvider(this ILoggerBuilder builder,
            ILogFormatter formatter,
            string file = null,
            LogLevel? level = null,
            TimeSpan? timeout = null,
            int? maxFileSizeKilobytes = null,
            int? maxArchiveCount = null,
            RolloverPeriod? rolloverPeriod = null)
        {
            if (formatter is null)
                throw new ArgumentNullException(nameof(formatter));

            return builder.AddRollingFileLogProvider(serviceProvider => formatter,
                 file, level, timeout, maxFileSizeKilobytes, maxArchiveCount, rolloverPeriod);
        }

        public static ILoggerBuilder AddRollingFileLogProvider<TLogFormatter>(this ILoggerBuilder builder,
            string file = null,
            LogLevel? level = null,
            TimeSpan? timeout = null,
            int? maxFileSizeKilobytes = null,
            int? maxArchiveCount = null,
            RolloverPeriod? rolloverPeriod = null,
            params object[] parameters)
            where TLogFormatter : ILogFormatter
        {
            return builder.AddRollingFileLogProvider(serviceProvider =>
                ActivatorUtilities.CreateInstance<TLogFormatter>(serviceProvider, parameters),
                file, level, timeout, maxFileSizeKilobytes, maxArchiveCount, rolloverPeriod);
        }

        public static ILoggerBuilder AddRollingFileLogProvider(this ILoggerBuilder builder,
            LogFormatterRegistration formatterFactory,
            string file = null,
            LogLevel? level = null,
            TimeSpan? timeout = null,
            int? maxFileSizeKilobytes = null,
            int? maxArchiveCount = null,
            RolloverPeriod? rolloverPeriod = null)
        {
            if (formatterFactory is null)
                throw new ArgumentNullException(nameof(formatterFactory));

            return builder.AddRollingFileLogProvider(options =>
            {
                options.FormatterFactory = formatterFactory;
                if (file != null) options.File = file;
                if (level != null) options.Level = level.Value;
                if (timeout != null) options.Timeout = timeout;
                if (maxFileSizeKilobytes != null) options.MaxFileSizeKilobytes = maxFileSizeKilobytes.Value;
                if (maxArchiveCount != null) options.MaxArchiveCount = maxArchiveCount.Value;
                if (rolloverPeriod != null) options.RolloverPeriod = rolloverPeriod.Value;
            });
        }

        public static ILoggerBuilder AddRollingFileLogProvider(this ILoggerBuilder builder,
            Action<RollingFileLogProviderOptions> configureOptions)
        {
            return builder.AddLogProvider(serviceProvider =>
            {
                var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<RollingFileLogProviderOptions>>();
                var options = optionsMonitor?.Get(builder.LoggerName) ?? new RollingFileLogProviderOptions();
                configureOptions?.Invoke(options);

                var formatter = options.FormatterFactory?.Invoke(serviceProvider)
                    ?? new TemplateLogFormatter(FileLogProvider.DefaultTemplate);

                return new RollingFileLogProvider(options.File, formatter, options.Level,
                options.Timeout, options.MaxFileSizeKilobytes, options.MaxArchiveCount, options.RolloverPeriod);
            });
        }

        #endregion
    }
}
#endif
