using System;
using System.Linq.Expressions;
using Moq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(RockLib.Logging.Moq.MockLoggerExtensions.GeneratedAssemblyName)]

namespace RockLib.Logging.Moq;

/// <summary>
/// Provides extension methods for setting up and verifying instances of <see cref="Mock{T}"/> of type
/// <see cref="ILogger"/>.
/// </summary>
public static partial class MockLoggerExtensions
{
    internal const string GeneratedAssemblyName = "RockLib.Logging.Moq.MockLoggerExtensions";

    /// <summary>
    /// Specifies setups on the mock logger for the <see cref="ILogger.Level"/> and <see cref="ILogger.Name"/>
    /// properties.
    /// </summary>
    /// <param name="mockLogger">The mock logger to perform setups on.</param>
    /// <param name="level">The level of the mock logger.</param>
    /// <param name="name">The name of the mock logger.</param>
    /// <returns>The same mock logger.</returns>
    public static Mock<ILogger> SetupLogger(this Mock<ILogger> mockLogger, LogLevel level = LogLevel.Debug, string name = Logger.DefaultName)
    {
        if (mockLogger is null) { throw new ArgumentNullException(nameof(mockLogger)); }
        mockLogger.Setup(m => m.Level).Returns(level);
        mockLogger.Setup(m => m.Name).Returns(name ?? Logger.DefaultName);

        return mockLogger;
    }

    /// <summary>
    /// Verifies that the mock logger logged at <see cref="LogLevel.Debug"/> the number of times specified
    /// by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyDebug(this Mock<ILogger> mockLogger, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, null, null, LogLevel.Debug, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged at <see cref="LogLevel.Info"/> the number of times specified
    /// by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyInfo(this Mock<ILogger> mockLogger, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, null, null, LogLevel.Info, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged at <see cref="LogLevel.Warn"/> the number of times specified
    /// by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyWarn(this Mock<ILogger> mockLogger, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, null, null, LogLevel.Warn, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged at <see cref="LogLevel.Error"/> the number of times specified
    /// by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyError(this Mock<ILogger> mockLogger, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, null, null, LogLevel.Error, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged at <see cref="LogLevel.Fatal"/> the number of times specified
    /// by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyFatal(this Mock<ILogger> mockLogger, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, null, null, LogLevel.Fatal, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged at <see cref="LogLevel.Audit"/> the number of times specified
    /// by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyAudit(this Mock<ILogger> mockLogger, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, null, null, LogLevel.Audit, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged at any <see cref="LogLevel"/> the number of times specified
    /// by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyLog(this Mock<ILogger> mockLogger, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, null, null, null, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// at <see cref="LogLevel.Debug"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyDebug(this Mock<ILogger> mockLogger, string message,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, null, null, LogLevel.Debug, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// at <see cref="LogLevel.Info"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyInfo(this Mock<ILogger> mockLogger, string message,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, null, null, LogLevel.Info, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// at <see cref="LogLevel.Warn"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyWarn(this Mock<ILogger> mockLogger, string message,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, null, null, LogLevel.Warn, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// at <see cref="LogLevel.Error"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyError(this Mock<ILogger> mockLogger, string message,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, null, null, LogLevel.Error, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// at <see cref="LogLevel.Fatal"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyFatal(this Mock<ILogger> mockLogger, string message,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, null, null, LogLevel.Fatal, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// at <see cref="LogLevel.Audit"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyAudit(this Mock<ILogger> mockLogger, string message,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, null, null, LogLevel.Audit, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// at any <see cref="LogLevel"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyLog(this Mock<ILogger> mockLogger, string message,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, null, null, null, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the extended properties specified by
    /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Debug"/> the number of times
    /// specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyDebug(this Mock<ILogger> mockLogger, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, null, extendedProperties, LogLevel.Debug, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the extended properties specified by
    /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Info"/> the number of times
    /// specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyInfo(this Mock<ILogger> mockLogger, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, null, extendedProperties, LogLevel.Info, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the extended properties specified by
    /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Warn"/> the number of times
    /// specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyWarn(this Mock<ILogger> mockLogger, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, null, extendedProperties, LogLevel.Warn, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the extended properties specified by
    /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Error"/> the number of times
    /// specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyError(this Mock<ILogger> mockLogger, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, null, extendedProperties, LogLevel.Error, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the extended properties specified by
    /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Fatal"/> the number of times
    /// specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyFatal(this Mock<ILogger> mockLogger, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, null, extendedProperties, LogLevel.Fatal, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the extended properties specified by
    /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Audit"/> the number of times
    /// specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyAudit(this Mock<ILogger> mockLogger, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, null, extendedProperties, LogLevel.Audit, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the extended properties specified by
    /// <paramref name="extendedProperties"/> at any <see cref="LogLevel"/> the number of times
    /// specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyLog(this Mock<ILogger> mockLogger, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, null, extendedProperties, null, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Debug"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyDebug(this Mock<ILogger> mockLogger, string message, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, null, extendedProperties, LogLevel.Debug, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Info"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyInfo(this Mock<ILogger> mockLogger, string message, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, null, extendedProperties, LogLevel.Info, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Warn"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyWarn(this Mock<ILogger> mockLogger, string message, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, null, extendedProperties, LogLevel.Warn, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Error"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyError(this Mock<ILogger> mockLogger, string message, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, null, extendedProperties, LogLevel.Error, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Fatal"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyFatal(this Mock<ILogger> mockLogger, string message, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, null, extendedProperties, LogLevel.Fatal, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Audit"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyAudit(this Mock<ILogger> mockLogger, string message, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, null, extendedProperties, LogLevel.Audit, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and the extended properties specified by <paramref name="extendedProperties"/> at any
    /// <see cref="LogLevel"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyLog(this Mock<ILogger> mockLogger, string message, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, null, extendedProperties, null, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the exception specified by <paramref name="exception"/>
    /// at <see cref="LogLevel.Debug"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyDebug(this Mock<ILogger> mockLogger, Exception exception, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, ex => ReferenceEquals(ex, exception), null, LogLevel.Debug, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the exception specified by <paramref name="exception"/>
    /// at <see cref="LogLevel.Info"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyInfo(this Mock<ILogger> mockLogger, Exception exception, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, ex => ReferenceEquals(ex, exception), null, LogLevel.Info, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the exception specified by <paramref name="exception"/>
    /// at <see cref="LogLevel.Warn"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyWarn(this Mock<ILogger> mockLogger, Exception exception, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, ex => ReferenceEquals(ex, exception), null, LogLevel.Warn, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the exception specified by <paramref name="exception"/>
    /// at <see cref="LogLevel.Error"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyError(this Mock<ILogger> mockLogger, Exception exception, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, ex => ReferenceEquals(ex, exception), null, LogLevel.Error, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the exception specified by <paramref name="exception"/>
    /// at <see cref="LogLevel.Fatal"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyFatal(this Mock<ILogger> mockLogger, Exception exception, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, ex => ReferenceEquals(ex, exception), null, LogLevel.Fatal, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the exception specified by <paramref name="exception"/>
    /// at <see cref="LogLevel.Audit"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyAudit(this Mock<ILogger> mockLogger, Exception exception, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, ex => ReferenceEquals(ex, exception), null, LogLevel.Audit, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the exception specified by <paramref name="exception"/>
    /// at any <see cref="LogLevel"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyLog(this Mock<ILogger> mockLogger, Exception exception, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, ex => ReferenceEquals(ex, exception), null, null, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and the exception specified by <paramref name="exception"/> at <see cref="LogLevel.Debug"/> the
    /// number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyDebug(this Mock<ILogger> mockLogger, string message, Exception exception,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, ex => ReferenceEquals(ex, exception), null, LogLevel.Debug, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and the exception specified by <paramref name="exception"/> at <see cref="LogLevel.Info"/> the
    /// number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyInfo(this Mock<ILogger> mockLogger, string message, Exception exception,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, ex => ReferenceEquals(ex, exception), null, LogLevel.Info, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and the exception specified by <paramref name="exception"/> at <see cref="LogLevel.Warn"/> the
    /// number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyWarn(this Mock<ILogger> mockLogger, string message, Exception exception,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, ex => ReferenceEquals(ex, exception), null, LogLevel.Warn, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and the exception specified by <paramref name="exception"/> at <see cref="LogLevel.Error"/> the
    /// number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyError(this Mock<ILogger> mockLogger, string message, Exception exception,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, ex => ReferenceEquals(ex, exception), null, LogLevel.Error, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and the exception specified by <paramref name="exception"/> at <see cref="LogLevel.Fatal"/> the
    /// number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyFatal(this Mock<ILogger> mockLogger, string message, Exception exception,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, ex => ReferenceEquals(ex, exception), null, LogLevel.Fatal, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and the exception specified by <paramref name="exception"/> at <see cref="LogLevel.Audit"/> the
    /// number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyAudit(this Mock<ILogger> mockLogger, string message, Exception exception,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, ex => ReferenceEquals(ex, exception), null, LogLevel.Audit, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and the exception specified by <paramref name="exception"/> at any <see cref="LogLevel"/> the
    /// number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyLog(this Mock<ILogger> mockLogger, string message, Exception exception,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, ex => ReferenceEquals(ex, exception), null, null, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the exception specified by <paramref name="exception"/> and
    /// the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Debug"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyDebug(this Mock<ILogger> mockLogger, Exception exception, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, ex => ReferenceEquals(ex, exception), extendedProperties, LogLevel.Debug, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the exception specified by <paramref name="exception"/> and
    /// the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Info"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyInfo(this Mock<ILogger> mockLogger, Exception exception, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, ex => ReferenceEquals(ex, exception), extendedProperties, LogLevel.Info, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the exception specified by <paramref name="exception"/> and
    /// the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Warn"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyWarn(this Mock<ILogger> mockLogger, Exception exception, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, ex => ReferenceEquals(ex, exception), extendedProperties, LogLevel.Warn, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the exception specified by <paramref name="exception"/> and
    /// the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Error"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyError(this Mock<ILogger> mockLogger, Exception exception, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, ex => ReferenceEquals(ex, exception), extendedProperties, LogLevel.Error, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the exception specified by <paramref name="exception"/> and
    /// the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Fatal"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyFatal(this Mock<ILogger> mockLogger, Exception exception, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, ex => ReferenceEquals(ex, exception), extendedProperties, LogLevel.Fatal, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the exception specified by <paramref name="exception"/> and
    /// the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Audit"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyAudit(this Mock<ILogger> mockLogger, Exception exception, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, ex => ReferenceEquals(ex, exception), extendedProperties, LogLevel.Audit, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the exception specified by <paramref name="exception"/> and
    /// the extended properties specified by <paramref name="extendedProperties"/> at any
    /// <see cref="LogLevel"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyLog(this Mock<ILogger> mockLogger, Exception exception, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, ex => ReferenceEquals(ex, exception), extendedProperties, null, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>,
    /// the exception specified by <paramref name="exception"/>,
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Debug"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyDebug(this Mock<ILogger> mockLogger, string message, Exception exception, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, ex => ReferenceEquals(ex, exception), extendedProperties, LogLevel.Debug, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>,
    /// the exception specified by <paramref name="exception"/>,
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Info"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyInfo(this Mock<ILogger> mockLogger, string message, Exception exception, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, ex => ReferenceEquals(ex, exception), extendedProperties, LogLevel.Info, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>,
    /// the exception specified by <paramref name="exception"/>,
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Warn"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyWarn(this Mock<ILogger> mockLogger, string message, Exception exception, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, ex => ReferenceEquals(ex, exception), extendedProperties, LogLevel.Warn, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>,
    /// the exception specified by <paramref name="exception"/>,
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Error"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyError(this Mock<ILogger> mockLogger, string message, Exception exception, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, ex => ReferenceEquals(ex, exception), extendedProperties, LogLevel.Error, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>,
    /// the exception specified by <paramref name="exception"/>,
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Fatal"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyFatal(this Mock<ILogger> mockLogger, string message, Exception exception, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, ex => ReferenceEquals(ex, exception), extendedProperties, LogLevel.Fatal, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>,
    /// the exception specified by <paramref name="exception"/>,
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Audit"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyAudit(this Mock<ILogger> mockLogger, string message, Exception exception, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, ex => ReferenceEquals(ex, exception), extendedProperties, LogLevel.Audit, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>,
    /// the exception specified by <paramref name="exception"/>,
    /// and the extended properties specified by <paramref name="extendedProperties"/> at any
    /// <see cref="LogLevel"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="exception">
    /// The exception instance that a log entry's <see cref="LogEntry.Exception"/> must be a
    /// reference to in order for successful verification to occur. Pass <see langword="null"/>
    /// if <see cref="LogEntry.Exception"/> is expected to be <see langword="null"/>.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyLog(this Mock<ILogger> mockLogger, string message, Exception exception, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, ex => ReferenceEquals(ex, exception), extendedProperties, null, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with an exception that returns true from the function specified
    /// by <paramref name="hasMatchingException"/> at <see cref="LogLevel.Debug"/> the number of times specified
    /// by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyDebug(this Mock<ILogger> mockLogger, Expression<Func<Exception, bool>> hasMatchingException, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, hasMatchingException, null, LogLevel.Debug, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with an exception that returns true from the function specified
    /// by <paramref name="hasMatchingException"/> at <see cref="LogLevel.Info"/> the number of times specified
    /// by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyInfo(this Mock<ILogger> mockLogger, Expression<Func<Exception, bool>> hasMatchingException, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, hasMatchingException, null, LogLevel.Info, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with an exception that returns true from the function specified
    /// by <paramref name="hasMatchingException"/> at <see cref="LogLevel.Warn"/> the number of times specified
    /// by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyWarn(this Mock<ILogger> mockLogger, Expression<Func<Exception, bool>> hasMatchingException, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, hasMatchingException, null, LogLevel.Warn, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with an exception that returns true from the function specified
    /// by <paramref name="hasMatchingException"/> at <see cref="LogLevel.Error"/> the number of times specified
    /// by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyError(this Mock<ILogger> mockLogger, Expression<Func<Exception, bool>> hasMatchingException, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, hasMatchingException, null, LogLevel.Error, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with an exception that returns true from the function specified
    /// by <paramref name="hasMatchingException"/> at <see cref="LogLevel.Fatal"/> the number of times specified
    /// by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyFatal(this Mock<ILogger> mockLogger, Expression<Func<Exception, bool>> hasMatchingException, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, hasMatchingException, null, LogLevel.Fatal, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with an exception that returns true from the function specified
    /// by <paramref name="hasMatchingException"/> at <see cref="LogLevel.Audit"/> the number of times specified
    /// by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyAudit(this Mock<ILogger> mockLogger, Expression<Func<Exception, bool>> hasMatchingException, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, hasMatchingException, null, LogLevel.Audit, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with an exception that returns true from the function specified
    /// by <paramref name="hasMatchingException"/> at any <see cref="LogLevel"/> the number of times specified
    /// by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyLog(this Mock<ILogger> mockLogger, Expression<Func<Exception, bool>> hasMatchingException, Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, hasMatchingException, null, null, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and an exception that returns true from the function specified by <paramref name="hasMatchingException"/>
    /// at <see cref="LogLevel.Debug"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyDebug(this Mock<ILogger> mockLogger, string message, Expression<Func<Exception, bool>> hasMatchingException,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, hasMatchingException, null, LogLevel.Debug, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and an exception that returns true from the function specified by <paramref name="hasMatchingException"/>
    /// at <see cref="LogLevel.Info"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyInfo(this Mock<ILogger> mockLogger, string message, Expression<Func<Exception, bool>> hasMatchingException,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, hasMatchingException, null, LogLevel.Info, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and an exception that returns true from the function specified by <paramref name="hasMatchingException"/>
    /// at <see cref="LogLevel.Warn"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyWarn(this Mock<ILogger> mockLogger, string message, Expression<Func<Exception, bool>> hasMatchingException,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, hasMatchingException, null, LogLevel.Warn, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and an exception that returns true from the function specified by <paramref name="hasMatchingException"/>
    /// at <see cref="LogLevel.Error"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyError(this Mock<ILogger> mockLogger, string message, Expression<Func<Exception, bool>> hasMatchingException,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, hasMatchingException, null, LogLevel.Error, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and an exception that returns true from the function specified by <paramref name="hasMatchingException"/>
    /// at <see cref="LogLevel.Fatal"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyFatal(this Mock<ILogger> mockLogger, string message, Expression<Func<Exception, bool>> hasMatchingException,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, hasMatchingException, null, LogLevel.Fatal, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and an exception that returns true from the function specified by <paramref name="hasMatchingException"/>
    /// at <see cref="LogLevel.Audit"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyAudit(this Mock<ILogger> mockLogger, string message, Expression<Func<Exception, bool>> hasMatchingException,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, hasMatchingException, null, LogLevel.Audit, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>
    /// and an exception that returns true from the function specified by <paramref name="hasMatchingException"/>
    /// at any <see cref="LogLevel"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyLog(this Mock<ILogger> mockLogger, string message, Expression<Func<Exception, bool>> hasMatchingException,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, hasMatchingException, null, null, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with an exception that returns true from the function
    /// specified by <paramref name="hasMatchingException"/> and the extended properties specified by
    /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Debug"/> the number of times
    /// specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyDebug(this Mock<ILogger> mockLogger, Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, hasMatchingException, extendedProperties, LogLevel.Debug, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with an exception that returns true from the function
    /// specified by <paramref name="hasMatchingException"/> and the extended properties specified by
    /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Info"/> the number of times
    /// specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyInfo(this Mock<ILogger> mockLogger, Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, hasMatchingException, extendedProperties, LogLevel.Info, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with an exception that returns true from the function
    /// specified by <paramref name="hasMatchingException"/> and the extended properties specified by
    /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Warn"/> the number of times
    /// specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyWarn(this Mock<ILogger> mockLogger, Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, hasMatchingException, extendedProperties, LogLevel.Warn, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with an exception that returns true from the function
    /// specified by <paramref name="hasMatchingException"/> and the extended properties specified by
    /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Error"/> the number of times
    /// specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyError(this Mock<ILogger> mockLogger, Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, hasMatchingException, extendedProperties, LogLevel.Error, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with an exception that returns true from the function
    /// specified by <paramref name="hasMatchingException"/> and the extended properties specified by
    /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Fatal"/> the number of times
    /// specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyFatal(this Mock<ILogger> mockLogger, Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, hasMatchingException, extendedProperties, LogLevel.Fatal, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with an exception that returns true from the function
    /// specified by <paramref name="hasMatchingException"/> and the extended properties specified by
    /// <paramref name="extendedProperties"/> at <see cref="LogLevel.Audit"/> the number of times
    /// specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyAudit(this Mock<ILogger> mockLogger, Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, hasMatchingException, extendedProperties, LogLevel.Audit, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with an exception that returns true from the function
    /// specified by <paramref name="hasMatchingException"/> and the extended properties specified by
    /// <paramref name="extendedProperties"/> at any <see cref="LogLevel"/> the number of times
    /// specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyLog(this Mock<ILogger> mockLogger, Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(null, hasMatchingException, extendedProperties, null, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>,
    /// an exception that returns true from the function specified by <paramref name="hasMatchingException"/>,
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Debug"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyDebug(this Mock<ILogger> mockLogger, string message, Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, hasMatchingException, extendedProperties, LogLevel.Debug, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>,
    /// an exception that returns true from the function specified by <paramref name="hasMatchingException"/>,
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Info"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyInfo(this Mock<ILogger> mockLogger, string message, Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, hasMatchingException, extendedProperties, LogLevel.Info, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>,
    /// an exception that returns true from the function specified by <paramref name="hasMatchingException"/>,
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Warn"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyWarn(this Mock<ILogger> mockLogger, string message, Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, hasMatchingException, extendedProperties, LogLevel.Warn, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>,
    /// an exception that returns true from the function specified by <paramref name="hasMatchingException"/>,
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Error"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyError(this Mock<ILogger> mockLogger, string message, Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, hasMatchingException, extendedProperties, LogLevel.Error, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>,
    /// an exception that returns true from the function specified by <paramref name="hasMatchingException"/>,
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Fatal"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyFatal(this Mock<ILogger> mockLogger, string message, Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, hasMatchingException, extendedProperties, LogLevel.Fatal, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>,
    /// an exception that returns true from the function specified by <paramref name="hasMatchingException"/>,
    /// and the extended properties specified by <paramref name="extendedProperties"/> at
    /// <see cref="LogLevel.Audit"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyAudit(this Mock<ILogger> mockLogger, string message, Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, hasMatchingException, extendedProperties, LogLevel.Audit, times, failMessage);

    /// <summary>
    /// Verifies that the mock logger logged with the message specified by <paramref name="message"/>,
    /// an exception that returns true from the function specified by <paramref name="hasMatchingException"/>,
    /// and the extended properties specified by <paramref name="extendedProperties"/> at any
    /// <see cref="LogLevel"/> the number of times specified by <paramref name="times"/>.
    /// </summary>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="message">
    /// The value that the message of a log must equal in order for successful verification
    /// to occur. <para>If the value of this parameter starts and ends with / (forward slash),
    /// then the contents of the value between the slashes are interpreted as a regular
    /// expression and matched against the log message.</para>
    /// </param>
    /// <param name="hasMatchingException">
    /// A function evaluated with a log entry's <see cref="LogEntry.Exception"/> that must
    /// return <see langword="true"/> in order for successful verification to occur.
    /// </param>
    /// <param name="extendedProperties">
    /// An object representing the extended properties that a log must match in order for successful
    /// verification to occur.
    /// </param>
    /// <param name="times">
    /// The number of times the mock logger is expected to have logged. If <see langword="null"/>,
    /// <see cref="Times.Once()"/> is used.
    /// </param>
    /// <param name="failMessage">Message to show if verification fails.</param>
    /// <exception cref="MockException">
    /// The logger did not log according to the specified criteria.
    /// </exception>
    public static void VerifyLog(this Mock<ILogger> mockLogger, string message, Expression<Func<Exception, bool>> hasMatchingException, object extendedProperties,
        Times? times = null, string? failMessage = null) =>
        mockLogger.VerifyLog(message, hasMatchingException, extendedProperties, null, times, failMessage);
}
