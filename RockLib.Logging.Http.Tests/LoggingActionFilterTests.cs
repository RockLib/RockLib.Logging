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

            loggingActionFilter.MessageFormat.Should().Be(LoggingActionFilter.DefaultMessageFormat);
            loggingActionFilter.LoggerName.Should().BeNull();
        }

        [Fact(DisplayName = "OnActionExecuting method sets logging context in HttpContext.Items")]
        public void OnActionExecutingMethodHappyPath()
        {
            const string messageFormat = "My message format: {0}.";
            const LogLevel logLevel = LogLevel.Info;
            const string actionName = "MyAction";

            IActionFilter loggingActionFilter = new Mock<LoggingActionFilter>(messageFormat, null, logLevel).Object;

            var mockLogger = new MockLogger();

            var httpContext = new DefaultHttpContext() { RequestServices = GetServiceProvider(mockLogger.Object) };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor() { DisplayName = actionName });
            var context = new ActionExecutingContext(actionContext, Array.Empty<IFilterMetadata>(), new Dictionary<string, object>(), null);
            context.ActionArguments.Add("foo", 123);

            loggingActionFilter.OnActionExecuting(context);

            var (logger, logEntry) =
                httpContext.Items.Should().ContainKey("RockLib.Logging.LogActionFilter.LoggingContext")
                    .WhichValue.Should().BeOfType<(ILogger, LogEntry)>()
                    .Subject;

            logger.Should().BeSameAs(mockLogger.Object);

            logEntry.Level.Should().Be(logLevel);
            logEntry.Message.Should().Be(string.Format(messageFormat, actionName));
            logEntry.ExtendedProperties.Should().ContainKey("foo")
                .WhichValue.Should().Be(123);
        }

        [Fact(DisplayName = "OnActionExecuted method retrieves logging context from HttpContext.Items and logs it")]
        public void OnActionExecutedMethodHappyPath1()
        {
            IActionFilter loggingActionFilter = new Mock<LoggingActionFilter>(null, null, LogLevel.Info).Object;

            var mockLogger = new MockLogger();
            var logEntry = new LogEntry();

            var httpContext = new DefaultHttpContext { Items = { ["RockLib.Logging.LogActionFilter.LoggingContext"] = (mockLogger.Object, logEntry) } };
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

            var httpContext = new DefaultHttpContext { Items = { ["RockLib.Logging.LogActionFilter.LoggingContext"] = (mockLogger.Object, logEntry) } };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new ActionExecutedContext(actionContext, Array.Empty<IFilterMetadata>(), null);
            var exception = context.Exception = new Exception();

            loggingActionFilter.OnActionExecuted(context);

            logEntry.Exception.Should().BeSameAs(exception);
            mockLogger.Verify(m => m.Log(logEntry, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "OnActionExecuted method adds 'ResponseObject' extended property if context.Result is ObjectResult")]
        public void OnActionExecutedMethodHappyPath3()
        {
            IActionFilter loggingActionFilter = new Mock<LoggingActionFilter>(null, null, LogLevel.Info).Object;

            var mockLogger = new MockLogger();
            var logEntry = new LogEntry();

            var httpContext = new DefaultHttpContext { Items = { ["RockLib.Logging.LogActionFilter.LoggingContext"] = (mockLogger.Object, logEntry) } };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new ActionExecutedContext(actionContext, Array.Empty<IFilterMetadata>(), null);
            context.Result = new ObjectResult(123);

            loggingActionFilter.OnActionExecuted(context);

            logEntry.ExtendedProperties.Should().ContainKey("ResponseObject")
                .WhichValue.Should().Be(123);
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
