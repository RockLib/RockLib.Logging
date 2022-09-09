using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace RockLib.Logging.DependencyInjection;

/// <summary>
/// Defines extension methods for <see cref="ILoggerBuilder"/> for adding log providers and
/// context providers.
/// </summary>
public static class LoggerBuilderExtensions
{
    /// <summary>
    /// Adds an <see cref="ILogProvider"/> of type <typeparamref name="TLogProvider"/> to the logger.
    /// </summary>
    /// <typeparam name="TLogProvider">The type of <see cref="ILogProvider"/> to add to the logger.</typeparam>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="parameters">
    /// Constructor arguments for type <typeparamref name="TLogProvider"/> that are not provided by the
    /// <see cref="IServiceProvider"/>.
    /// </param>
    /// <returns>The same <see cref="ILoggerBuilder"/></returns>
    public static ILoggerBuilder AddLogProvider<TLogProvider>(this ILoggerBuilder builder, params object[] parameters)
        where TLogProvider : ILogProvider
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddLogProvider(serviceProvider => ActivatorUtilities.CreateInstance<TLogProvider>(serviceProvider, parameters));
    }

    /// <summary>
    /// Adds an <see cref="IContextProvider"/> of type <typeparamref name="TContextProvider"/> to the logger.
    /// </summary>
    /// <typeparam name="TContextProvider">The type of <see cref="IContextProvider"/> to add to the logger.</typeparam>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="parameters">
    /// Constructor arguments for type <typeparamref name="TContextProvider"/> that are not provided by the
    /// <see cref="IServiceProvider"/>.
    /// </param>
    /// <returns>The same <see cref="ILoggerBuilder"/></returns>
    public static ILoggerBuilder AddContextProvider<TContextProvider>(this ILoggerBuilder builder, params object[] parameters)
        where TContextProvider : IContextProvider
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddContextProvider(serviceProvider => ActivatorUtilities.CreateInstance<TContextProvider>(serviceProvider, parameters));
    }

    #region ConsoleLogProvider

    /// <summary>
    /// Adds a <see cref="ConsoleLogProvider"/>, formatted with a <see cref="TemplateLogFormatter"/>,
    /// to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="template">
    /// The template of the <see cref="TemplateLogFormatter"/> that the log provider uses for formatting
    /// logs.
    /// </param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="output">The type of console output stream to write to.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddConsoleLogProvider(this ILoggerBuilder builder,
        string template,
        LogLevel? level = null,
        ConsoleLogProvider.Output? output = null,
        TimeSpan? timeout = null)
    {
        if (template is null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        return builder.AddConsoleLogProvider<TemplateLogFormatter>(level, output, timeout, template);
    }

    /// <summary>
    /// Adds a <see cref="ConsoleLogProvider"/>, formatted with the specified <see cref="ILogFormatter"/>,
    /// to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="formatter">
    /// The <see cref="ILogFormatter"/> that the log provider uses for formatting logs.
    /// </param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="output">The type of console output stream to write to.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddConsoleLogProvider(this ILoggerBuilder builder,
        ILogFormatter formatter,
        LogLevel? level = null,
        ConsoleLogProvider.Output? output = null,
        TimeSpan? timeout = null)
    {
        if (formatter is null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        return builder.AddConsoleLogProvider(serviceProvider => formatter, level, output, timeout);
    }

    /// <summary>
    /// Adds a <see cref="ConsoleLogProvider"/>, formatted with a <see cref="ILogFormatter"/> of
    /// type <typeparamref name="TLogFormatter"/>, to the logger.
    /// </summary>
    /// <typeparam name="TLogFormatter">
    /// The type of <see cref="ILogFormatter"/> that the log provider uses for formatting logs.
    /// </typeparam>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="output">The type of console output stream to write to.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <param name="logFormatterParameters">
    /// Constructor arguments for type <typeparamref name="TLogFormatter"/> that are not provided
    /// by the <see cref="IServiceProvider"/>.
    /// </param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddConsoleLogProvider<TLogFormatter>(this ILoggerBuilder builder,
        LogLevel? level = null,
        ConsoleLogProvider.Output? output = null,
        TimeSpan? timeout = null,
        params object[] logFormatterParameters)
        where TLogFormatter : ILogFormatter => builder.AddConsoleLogProvider(serviceProvider =>
                                                        ActivatorUtilities.CreateInstance<TLogFormatter>(serviceProvider, logFormatterParameters),
            level, output, timeout);

    /// <summary>
    /// Adds a <see cref="ConsoleLogProvider"/>, formatted with the <see cref="ILogFormatter"/>
    /// returned by the formatter registration, to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="formatterRegistration">
    /// The method used to create the <see cref="ILogFormatter"/> of the log provider.
    /// </param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="output">The type of console output stream to write to.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddConsoleLogProvider(this ILoggerBuilder builder,
        Func<IServiceProvider, ILogFormatter> formatterRegistration,
        LogLevel? level = null,
        ConsoleLogProvider.Output? output = null,
        TimeSpan? timeout = null)
    {
        if (formatterRegistration is null)
        {
            throw new ArgumentNullException(nameof(formatterRegistration));
        }

        return builder.AddConsoleLogProvider(options =>
        {
            options.FormatterRegistration = formatterRegistration;
            if (level is not null) options.Level = level.Value;
            if (output is not null) options.Output = output.Value;
            if (timeout is not null) options.Timeout = timeout;
        });
    }

    /// <summary>
    /// Adds a <see cref="ConsoleLogProvider"/> to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="configureOptions">
    /// A delegate to configure the <see cref="ConsoleLogProviderOptions"/> object that is used to
    /// configure the log provider.
    /// </param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddConsoleLogProvider(this ILoggerBuilder builder,
        Action<ConsoleLogProviderOptions>? configureOptions = null)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddLogProvider(serviceProvider =>
        {
#pragma warning disable CA1820 // Test for empty strings using string length
            var optionsLoggerName = builder.LoggerName == Logger.DefaultName
                ? Options.DefaultName
                : builder.LoggerName;
#pragma warning restore CA1820 // Test for empty strings using string length

            var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<ConsoleLogProviderOptions>>();
            var options = optionsMonitor?.Get(optionsLoggerName) ?? new ConsoleLogProviderOptions();
            configureOptions?.Invoke(options);

            var formatter = options.FormatterRegistration?.Invoke(serviceProvider)
                ?? new TemplateLogFormatter(ConsoleLogProvider.DefaultTemplate);

            if (optionsMonitor is not null && options.ReloadOnChange)
            {
                return new ReloadingLogProvider<ConsoleLogProviderOptions>(
                    optionsMonitor, options, CreateLogProvider, builder.LoggerName, configureOptions!);
            }

            return CreateLogProvider(options);

            ILogProvider CreateLogProvider(ConsoleLogProviderOptions o) =>
                new ConsoleLogProvider(formatter, o.Level, o.Output, o.Timeout);
        });
    }

    #endregion
    #region DebugLogProvider

    /// <summary>
    /// Adds a <see cref="DebugLogProvider"/>, formatted with a <see cref="TemplateLogFormatter"/>,
    /// to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="template">
    /// The template of the <see cref="TemplateLogFormatter"/> that the log provider uses for formatting
    /// logs.
    /// </param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddDebugLogProvider(this ILoggerBuilder builder,
        string template,
        LogLevel? level = null,
        TimeSpan? timeout = null)
    {
        if (template is null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        return builder.AddDebugLogProvider<TemplateLogFormatter>(level, timeout, template);
    }

    /// <summary>
    /// Adds a <see cref="DebugLogProvider"/>, formatted with the specified <see cref="ILogFormatter"/>,
    /// to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="formatter">
    /// The <see cref="ILogFormatter"/> that the log provider uses for formatting logs.
    /// </param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddDebugLogProvider(this ILoggerBuilder builder,
        ILogFormatter formatter,
        LogLevel? level = null,
        TimeSpan? timeout = null)
    {
        if (formatter is null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        return builder.AddDebugLogProvider(serviceProvider => formatter, level, timeout);
    }

    /// <summary>
    /// Adds a <see cref="DebugLogProvider"/>, formatted with a <see cref="ILogFormatter"/> of
    /// type <typeparamref name="TLogFormatter"/>, to the logger.
    /// </summary>
    /// <typeparam name="TLogFormatter">
    /// The type of <see cref="ILogFormatter"/> that the log provider uses for formatting logs.
    /// </typeparam>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <param name="logFormatterParameters">
    /// Constructor arguments for type <typeparamref name="TLogFormatter"/> that are not provided
    /// by the <see cref="IServiceProvider"/>.
    /// </param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddDebugLogProvider<TLogFormatter>(this ILoggerBuilder builder,
        LogLevel? level = null,
        TimeSpan? timeout = null,
        params object[] logFormatterParameters)
        where TLogFormatter : ILogFormatter => builder.AddDebugLogProvider(serviceProvider =>
                                                        ActivatorUtilities.CreateInstance<TLogFormatter>(serviceProvider, logFormatterParameters),
            level, timeout);

    /// <summary>
    /// Adds a <see cref="DebugLogProvider"/>, formatted with the <see cref="ILogFormatter"/>
    /// returned by the formatter registration, to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="formatterRegistration">
    /// The method used to create the <see cref="ILogFormatter"/> of the log provider.
    /// </param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddDebugLogProvider(this ILoggerBuilder builder,
        Func<IServiceProvider, ILogFormatter> formatterRegistration,
        LogLevel? level = null,
        TimeSpan? timeout = null)
    {
        if (formatterRegistration is null)
        {
            throw new ArgumentNullException(nameof(formatterRegistration));
        }

        return builder.AddDebugLogProvider(options =>
        {
            options.FormatterRegistration = formatterRegistration;
            if (level is not null) options.Level = level.Value;
            if (timeout is not null) options.Timeout = timeout;
        });
    }

    /// <summary>
    /// Adds a <see cref="DebugLogProvider"/> to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="configureOptions">
    /// A delegate to configure the <see cref="DebugLogProviderOptions"/> object that is used to
    /// configure the log provider.
    /// </param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddDebugLogProvider(this ILoggerBuilder builder,
        Action<DebugLogProviderOptions>? configureOptions = null)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddLogProvider(serviceProvider =>
        {
#pragma warning disable CA1820 // Test for empty strings using string length
            var optionsLoggerName = builder.LoggerName == Logger.DefaultName
                ? Options.DefaultName
                : builder.LoggerName;
#pragma warning restore CA1820 // Test for empty strings using string length

            var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<DebugLogProviderOptions>>();
            var options = optionsMonitor?.Get(optionsLoggerName) ?? new DebugLogProviderOptions();
            configureOptions?.Invoke(options);

            var formatter = options.FormatterRegistration?.Invoke(serviceProvider)
                ?? new TemplateLogFormatter(DebugLogProvider.DefaultTemplate);

            if (optionsMonitor is not null && options.ReloadOnChange)
            {
                return new ReloadingLogProvider<DebugLogProviderOptions>(
                    optionsMonitor, options, CreateLogProvider, builder.LoggerName, configureOptions!);
            }

            return CreateLogProvider(options);

            ILogProvider CreateLogProvider(DebugLogProviderOptions o) =>
                new DebugLogProvider(formatter, options.Level, options.Timeout);
        });
    }

    #endregion
    #region FileLogProvider

    /// <summary>
    /// Adds a <see cref="FileLogProvider"/>, formatted with a <see cref="TemplateLogFormatter"/>,
    /// to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="template">
    /// The template of the <see cref="TemplateLogFormatter"/> that the log provider uses for formatting
    /// logs.
    /// </param>
    /// <param name="file">The file to write to.</param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddFileLogProvider(this ILoggerBuilder builder,
        string template,
        string? file = null,
        LogLevel? level = null,
        TimeSpan? timeout = null)
    {
        if (template is null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        return builder.AddFileLogProvider<TemplateLogFormatter>(file, level, timeout, template);
    }

    /// <summary>
    /// Adds a <see cref="FileLogProvider"/>, formatted with the specified <see cref="ILogFormatter"/>,
    /// to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="formatter">
    /// The <see cref="ILogFormatter"/> that the log provider uses for formatting logs.
    /// </param>
    /// <param name="file">The file to write to.</param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddFileLogProvider(this ILoggerBuilder builder,
        ILogFormatter formatter,
        string? file = null,
        LogLevel? level = null,
        TimeSpan? timeout = null)
    {
        if (formatter is null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        return builder.AddFileLogProvider(serviceProvider => formatter, file, level, timeout);
    }

    /// <summary>
    /// Adds a <see cref="FileLogProvider"/>, formatted with a <see cref="ILogFormatter"/> of
    /// type <typeparamref name="TLogFormatter"/>, to the logger.
    /// </summary>
    /// <typeparam name="TLogFormatter">
    /// The type of <see cref="ILogFormatter"/> that the log provider uses for formatting logs.
    /// </typeparam>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="file">The file to write to.</param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <param name="logFormatterParameters">
    /// Constructor arguments for type <typeparamref name="TLogFormatter"/> that are not provided
    /// by the <see cref="IServiceProvider"/>.
    /// </param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddFileLogProvider<TLogFormatter>(this ILoggerBuilder builder,
        string? file = null,
        LogLevel? level = null,
        TimeSpan? timeout = null,
        params object[] logFormatterParameters)
        where TLogFormatter : ILogFormatter => builder.AddFileLogProvider(serviceProvider =>
                                                        ActivatorUtilities.CreateInstance<TLogFormatter>(serviceProvider, logFormatterParameters),
            file, level, timeout);

    /// <summary>
    /// Adds a <see cref="FileLogProvider"/>, formatted with the <see cref="ILogFormatter"/>
    /// returned by the formatter registration, to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="formatterRegistration">
    /// The method used to create the <see cref="ILogFormatter"/> of the log provider.
    /// </param>
    /// <param name="file">The file to write to.</param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddFileLogProvider(this ILoggerBuilder builder,
        Func<IServiceProvider, ILogFormatter> formatterRegistration,
        string? file = null,
        LogLevel? level = null,
        TimeSpan? timeout = null)
    {
        if (formatterRegistration is null)
        {
            throw new ArgumentNullException(nameof(formatterRegistration));
        }

        return builder.AddFileLogProvider(options =>
        {
            options.FormatterRegistration = formatterRegistration;
            if (file is not null) options.File = file;
            if (level is not null) options.Level = level.Value;
            if (timeout is not null) options.Timeout = timeout;
        });
    }

    /// <summary>
    /// Adds a <see cref="FileLogProvider"/> to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="configureOptions">
    /// A delegate to configure the <see cref="FileLogProviderOptions"/> object that is used to
    /// configure the log provider.
    /// </param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddFileLogProvider(this ILoggerBuilder builder,
        Action<FileLogProviderOptions>? configureOptions = null)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddLogProvider(serviceProvider =>
        {
#pragma warning disable CA1820 // Test for empty strings using string length
            var optionsLoggerName = builder.LoggerName == Logger.DefaultName
                ? Options.DefaultName
                : builder.LoggerName;
#pragma warning restore CA1820 // Test for empty strings using string length

            var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<FileLogProviderOptions>>();
            var options = optionsMonitor?.Get(optionsLoggerName) ?? new FileLogProviderOptions();
            configureOptions?.Invoke(options);

            var formatter = options.FormatterRegistration?.Invoke(serviceProvider)
                ?? new TemplateLogFormatter(FileLogProvider.DefaultTemplate);

            if (optionsMonitor is not null && options.ReloadOnChange)
            {
                return new ReloadingLogProvider<FileLogProviderOptions>(
                    optionsMonitor, options, CreateLogProvider, builder.LoggerName, configureOptions!);
            }

            return CreateLogProvider(options);

            ILogProvider CreateLogProvider(FileLogProviderOptions o) =>
                new FileLogProvider(o.File, formatter, o.Level, o.Timeout);
        });
    }

    #endregion
    #region RollingFileLogProvider

    /// <summary>
    /// Adds a <see cref="RollingFileLogProvider"/>, formatted with a <see cref="TemplateLogFormatter"/>,
    /// to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="template">
    /// The template of the <see cref="TemplateLogFormatter"/> that the log provider uses for formatting
    /// logs.
    /// </param>
    /// <param name="file">The file to write to.</param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <param name="maxFileSizeKilobytes">
    /// The maximum file size, in bytes, of the file. If the file size is greater than this value,
    /// it is archived.
    /// </param>
    /// <param name="maxArchiveCount">
    /// The maximum number of archive files that will be kept. If the number of archive files is
    /// greater than this value, then they are deleted, oldest first.
    /// </param>
    /// <param name="rolloverPeriod">
    /// The rollover period, indicating if/how the file should archived on a periodic basis.
    /// </param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddRollingFileLogProvider(this ILoggerBuilder builder,
        string template,
        string? file = null,
        LogLevel? level = null,
        TimeSpan? timeout = null,
        int? maxFileSizeKilobytes = null,
        int? maxArchiveCount = null,
        RolloverPeriod? rolloverPeriod = null)
    {
        if (template is null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        return builder.AddRollingFileLogProvider<TemplateLogFormatter>(
            file, level, timeout, maxFileSizeKilobytes, maxArchiveCount, rolloverPeriod, template);
    }

    /// <summary>
    /// Adds a <see cref="RollingFileLogProvider"/>, formatted with the specified <see cref="ILogFormatter"/>,
    /// to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="formatter">
    /// The <see cref="ILogFormatter"/> that the log provider uses for formatting logs.
    /// </param>
    /// <param name="file">The file to write to.</param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <param name="maxFileSizeKilobytes">
    /// The maximum file size, in bytes, of the file. If the file size is greater than this value,
    /// it is archived.
    /// </param>
    /// <param name="maxArchiveCount">
    /// The maximum number of archive files that will be kept. If the number of archive files is
    /// greater than this value, then they are deleted, oldest first.
    /// </param>
    /// <param name="rolloverPeriod">
    /// The rollover period, indicating if/how the file should archived on a periodic basis.
    /// </param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddRollingFileLogProvider(this ILoggerBuilder builder,
        ILogFormatter formatter,
        string? file = null,
        LogLevel? level = null,
        TimeSpan? timeout = null,
        int? maxFileSizeKilobytes = null,
        int? maxArchiveCount = null,
        RolloverPeriod? rolloverPeriod = null)
    {
        if (formatter is null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        return builder.AddRollingFileLogProvider(serviceProvider => formatter,
             file, level, timeout, maxFileSizeKilobytes, maxArchiveCount, rolloverPeriod);
    }

    /// <summary>
    /// Adds a <see cref="RollingFileLogProvider"/>, formatted with a <see cref="ILogFormatter"/> of
    /// type <typeparamref name="TLogFormatter"/>, to the logger.
    /// </summary>
    /// <typeparam name="TLogFormatter">
    /// The type of <see cref="ILogFormatter"/> that the log provider uses for formatting logs.
    /// </typeparam>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="file">The file to write to.</param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <param name="maxFileSizeKilobytes">
    /// The maximum file size, in bytes, of the file. If the file size is greater than this value,
    /// it is archived.
    /// </param>
    /// <param name="maxArchiveCount">
    /// The maximum number of archive files that will be kept. If the number of archive files is
    /// greater than this value, then they are deleted, oldest first.
    /// </param>
    /// <param name="rolloverPeriod">
    /// The rollover period, indicating if/how the file should archived on a periodic basis.
    /// </param>
    /// <param name="logFormatterParameters">
    /// Constructor arguments for type <typeparamref name="TLogFormatter"/> that are not provided
    /// by the <see cref="IServiceProvider"/>.
    /// </param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddRollingFileLogProvider<TLogFormatter>(this ILoggerBuilder builder,
        string? file = null,
        LogLevel? level = null,
        TimeSpan? timeout = null,
        int? maxFileSizeKilobytes = null,
        int? maxArchiveCount = null,
        RolloverPeriod? rolloverPeriod = null,
        params object[] logFormatterParameters)
        where TLogFormatter : ILogFormatter => builder.AddRollingFileLogProvider(serviceProvider =>
                                                        ActivatorUtilities.CreateInstance<TLogFormatter>(serviceProvider, logFormatterParameters),
            file, level, timeout, maxFileSizeKilobytes, maxArchiveCount, rolloverPeriod);

    /// <summary>
    /// Adds a <see cref="RollingFileLogProvider"/>, formatted with the <see cref="ILogFormatter"/>
    /// returned by the formatter registration, to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="formatterRegistration">
    /// The method used to create the <see cref="ILogFormatter"/> of the log provider.
    /// </param>
    /// <param name="file">The file to write to.</param>
    /// <param name="level">The logging level of the log provider.</param>
    /// <param name="timeout">The timeout of the log provider.</param>
    /// <param name="maxFileSizeKilobytes">
    /// The maximum file size, in bytes, of the file. If the file size is greater than this value,
    /// it is archived.
    /// </param>
    /// <param name="maxArchiveCount">
    /// The maximum number of archive files that will be kept. If the number of archive files is
    /// greater than this value, then they are deleted, oldest first.
    /// </param>
    /// <param name="rolloverPeriod">
    /// The rollover period, indicating if/how the file should archived on a periodic basis.
    /// </param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddRollingFileLogProvider(this ILoggerBuilder builder,
        Func<IServiceProvider, ILogFormatter> formatterRegistration,
        string? file = null,
        LogLevel? level = null,
        TimeSpan? timeout = null,
        int? maxFileSizeKilobytes = null,
        int? maxArchiveCount = null,
        RolloverPeriod? rolloverPeriod = null)
    {
        if (formatterRegistration is null)
        {
            throw new ArgumentNullException(nameof(formatterRegistration));
        }

        return builder.AddRollingFileLogProvider(options =>
        {
            options.FormatterRegistration = formatterRegistration;
            if (file is not null) options.File = file;
            if (level is not null) options.Level = level.Value;
            if (timeout is not null) options.Timeout = timeout;
            if (maxFileSizeKilobytes is not null) options.MaxFileSizeKilobytes = maxFileSizeKilobytes.Value;
            if (maxArchiveCount is not null) options.MaxArchiveCount = maxArchiveCount.Value;
            if (rolloverPeriod is not null) options.RolloverPeriod = rolloverPeriod.Value;
        });
    }

    /// <summary>
    /// Adds a <see cref="RollingFileLogProvider"/> to the logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggerBuilder"/>.</param>
    /// <param name="configureOptions">
    /// A delegate to configure the <see cref="RollingFileLogProviderOptions"/> object that is used to
    /// configure the log provider.
    /// </param>
    /// <returns>The same <see cref="ILoggerBuilder"/>.</returns>
    public static ILoggerBuilder AddRollingFileLogProvider(this ILoggerBuilder builder,
        Action<RollingFileLogProviderOptions>? configureOptions = null)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddLogProvider(serviceProvider =>
        {
#pragma warning disable CA1820 // Test for empty strings using string length
            var optionsLoggerName = builder.LoggerName == Logger.DefaultName
                ? Options.DefaultName
                : builder.LoggerName;
#pragma warning restore CA1820 // Test for empty strings using string length

            var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<RollingFileLogProviderOptions>>();
            var options = optionsMonitor?.Get(optionsLoggerName) ?? new RollingFileLogProviderOptions();
            configureOptions?.Invoke(options);

            var formatter = options.FormatterRegistration?.Invoke(serviceProvider)
                ?? new TemplateLogFormatter(FileLogProvider.DefaultTemplate);

            if (optionsMonitor is not null && options.ReloadOnChange)
            {
                return new ReloadingLogProvider<RollingFileLogProviderOptions>(
                    optionsMonitor, options, CreateLogProvider, builder.LoggerName, configureOptions!);
            }

            return CreateLogProvider(options);

            ILogProvider CreateLogProvider(RollingFileLogProviderOptions o) =>
                new RollingFileLogProvider(o.File, formatter, o.Level, o.Timeout,
                    o.MaxFileSizeKilobytes, o.MaxArchiveCount, o.RolloverPeriod);
        });
    }

    #endregion
}