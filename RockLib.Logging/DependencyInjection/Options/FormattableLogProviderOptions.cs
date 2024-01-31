using Microsoft.Extensions.DependencyInjection;
using System;

namespace RockLib.Logging.DependencyInjection;

/// <summary>
/// Defines an options class for creating an <see cref="ILogProvider"/> that is
/// formatted by an <see cref="ILogFormatter"/>.
/// </summary>
public abstract class FormattableLogProviderOptions : LogProviderOptions
{
    /// <summary>
    /// The method used to create the <see cref="ILogFormatter"/> of the log provider.
    /// </summary>
    public Func<IServiceProvider, ILogFormatter>? FormatterRegistration { get; set; }

    /// <summary>
    /// Sets <see cref="FormatterRegistration"/> to a method that creates a
    /// <see cref="TemplateLogFormatter"/> with the specified template.
    /// </summary>
    /// <param name="template">The template to use when formatting logs.</param>
    public void SetTemplate(string template)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(template);
#else
        if (template is null)
        {
            throw new ArgumentNullException(nameof(template));
        }
#endif

        SetFormatter<TemplateLogFormatter>(template);
    }

    /// <summary>
    /// Sets <see cref="FormatterRegistration"/> to a method that returns the specified
    /// <see cref="ILogFormatter"/>.
    /// </summary>
    /// <param name="formatter">The <see cref="ILogFormatter"/> to use for formatting logs.</param>
    public void SetFormatter(ILogFormatter formatter)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(formatter);
#else
        if (formatter is null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }
#endif

        FormatterRegistration = serviceProvider => formatter;
    }

    /// <summary>
    /// Sets <see cref="FormatterRegistration"/> to a method that returns a new instance
    /// of type <typeparamref name="TLogFormatter"/>.
    /// </summary>
    /// <typeparam name="TLogFormatter">
    /// The type of <see cref="ILogFormatter"/> that the log provider uses for formatting logs.
    /// </typeparam>
    /// <param name="logFormatterParameters">
    /// Constructor arguments for type <typeparamref name="TLogFormatter"/> that are not provided
    /// by the <see cref="IServiceProvider"/>.
    /// </param>
    public void SetFormatter<TLogFormatter>(params object[] logFormatterParameters)
        where TLogFormatter : ILogFormatter => FormatterRegistration = serviceProvider => ActivatorUtilities.CreateInstance<TLogFormatter>(serviceProvider, logFormatterParameters);
}