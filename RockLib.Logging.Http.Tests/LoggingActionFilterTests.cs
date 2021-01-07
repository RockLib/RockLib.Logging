using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RockLib.Logging.DependencyInjection;
using RockLib.Logging.Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace RockLib.Logging.Http.Tests
{
    using static LoggingActionFilter;
    using static Logger;

    public class LoggingActionFilterTests
    {
        [Fact(DisplayName = "Constructor sets properties from non-null parameters")]
        public void ConstructorHappyPath1()
        {
            const string messageFormat = "My message format: {0}.";
            const string loggerName = "MyLogger";
            const LogLevel logLevel = LogLevel.Warn;

            var loggingActionFilter = new Mock<LoggingActionFilter>(messageFormat, loggerName, logLevel).Object;

            loggingActionFilter.MessageFormat.Should().Be(messageFormat);
            loggingActionFilter.LoggerName.Should().Be(loggerName);
            loggingActionFilter.LogLevel.Should().Be(logLevel);
        }

        [Fact(DisplayName = "Constructor sets properties from null parameters")]
        public void ConstructorHappyPath2()
        {
            var loggingActionFilter = new Mock<LoggingActionFilter>(null, null, LogLevel.Error).Object;

            loggingActionFilter.MessageFormat.Should().Be(DefaultMessageFormat);
            loggingActionFilter.LoggerName.Should().Be(DefaultName);
        }

        [Fact(DisplayName = "OnActionExecuting method sets logging context in HttpContext.Items")]
        public void OnActionExecutingMethodHappyPath()
        {
            const string messageFormat = "My message format: {0}.";
            const LogLevel logLevel = LogLevel.Info;
            const string actionName = "MyAction";
            const string actionArgumentName = "foo";
            const int actionArgument = 123;

            IActionFilter loggingActionFilter = new Mock<LoggingActionFilter>(messageFormat, null, logLevel).Object;

            var mockLogger = new MockLogger();

            var httpContext = new DefaultHttpContext() { RequestServices = GetServiceProvider(mockLogger.Object) };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor() { DisplayName = actionName });
            var context = new ActionExecutingContext(actionContext, Array.Empty<IFilterMetadata>(), new Dictionary<string, object>(), null);
            context.ActionArguments.Add(actionArgumentName, actionArgument);

            loggingActionFilter.OnActionExecuting(context);

            var (logger, logEntry) =
                httpContext.Items.Should().ContainKey(LoggingContextItemsKey)
                    .WhichValue.Should().BeOfType<(ILogger, LogEntry)>()
                    .Subject;

            logger.Should().BeSameAs(mockLogger.Object);

            logEntry.Level.Should().Be(logLevel);
            logEntry.Message.Should().Be(string.Format(messageFormat, actionName));
            logEntry.ExtendedProperties.Should().ContainKey(actionArgumentName)
                .WhichValue.Should().Be(actionArgument);
        }

        [Fact(DisplayName = "OnActionExecuted method retrieves logging context from HttpContext.Items and logs it")]
        public void OnActionExecutedMethodHappyPath1()
        {
            IActionFilter loggingActionFilter = new Mock<LoggingActionFilter>(null, null, LogLevel.Info).Object;

            var mockLogger = new MockLogger();
            var logEntry = new LogEntry();

            var httpContext = new DefaultHttpContext { Items = { [LoggingContextItemsKey] = (mockLogger.Object, logEntry) } };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new ActionExecutedContext(actionContext, Array.Empty<IFilterMetadata>(), null);

            loggingActionFilter.OnActionExecuted(context);

            mockLogger.Verify(m => m.Log(logEntry, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "OnActionExecuted method sets logEntry exception from context.Exception if present")]
        public void OnActionExecutedMethodHappyPath2()
        {
            IActionFilter loggingActionFilter = new Mock<LoggingActionFilter>(null, null, LogLevel.Info).Object;

            var mockLogger = new MockLogger();
            var logEntry = new LogEntry();

            var httpContext = new DefaultHttpContext { Items = { [LoggingContextItemsKey] = (mockLogger.Object, logEntry) } };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new ActionExecutedContext(actionContext, Array.Empty<IFilterMetadata>(), null);
            var exception = context.Exception = new Exception();

            loggingActionFilter.OnActionExecuted(context);

            logEntry.Exception.Should().BeSameAs(exception);
            mockLogger.Verify(m => m.Log(logEntry, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "OnActionExecuted method adds 'ResultObject' extended property if context.Result is ObjectResult")]
        public void OnActionExecutedMethodHappyPath3()
        {
            const int resultObject = 123;

            IActionFilter loggingActionFilter = new Mock<LoggingActionFilter>(null, null, LogLevel.Info).Object;

            var mockLogger = new MockLogger();
            var logEntry = new LogEntry();

            var httpContext = new DefaultHttpContext { Items = { [LoggingContextItemsKey] = (mockLogger.Object, logEntry) } };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new ActionExecutedContext(actionContext, Array.Empty<IFilterMetadata>(), null);
            context.Result = new ObjectResult(resultObject);

            loggingActionFilter.OnActionExecuted(context);

            logEntry.ExtendedProperties.Should().ContainKey(ResultObjectExtendedPropertiesKey)
                .WhichValue.Should().Be(resultObject);
            mockLogger.Verify(m => m.Log(logEntry, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        private static IServiceProvider GetServiceProvider(ILogger logger)
        {
            var services = new ServiceCollection();
            services.AddSingleton(logger);
            services.AddSingleton<LoggerLookup>(loggerName => logger);
            return services.BuildServiceProvider();
        }
    }
}
