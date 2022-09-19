﻿using FluentAssertions;
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
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.AspNetCore.Tests;

using static LoggingActionFilterAttribute;
using static Logger;

public class LoggingActionFilterAttributeTests
{
    [Fact(DisplayName = "Constructor sets properties from non-null parameters")]
    public void ConstructorHappyPath1()
    {
        const string messageFormat = "My message format: {0}.";
        const string loggerName = "MyLogger";
        const LogLevel logLevel = LogLevel.Warn;

        var loggingActionFilter = new Mock<LoggingActionFilterAttribute>(messageFormat, loggerName, logLevel).Object;

        loggingActionFilter.MessageFormat.Should().Be(messageFormat);
        loggingActionFilter.LoggerName.Should().Be(loggerName);
        loggingActionFilter.LogLevel.Should().Be(logLevel);
    }

    [Fact(DisplayName = "Constructor sets properties from null parameters")]
    public void ConstructorHappyPath2()
    {
        var loggingActionFilter = new Mock<LoggingActionFilterAttribute>(null, null, LogLevel.Error).Object;

        loggingActionFilter.MessageFormat.Should().Be(DefaultMessageFormat);
        loggingActionFilter.LoggerName.Should().Be(DefaultName);
    }

    [Fact(DisplayName = "OnActionExecutionAsync method logs the action")]
    public async Task OnActionExecutionAsyncMethodHappyPath1()
    {
        const string messageFormat = "My message format: {0}";
        const LogLevel logLevel = LogLevel.Info;
        const string actionName = "MyAction";
        const string actionArgumentName = "foo";
        const int actionArgument = 123;

        IAsyncActionFilter loggingActionFilter = new Mock<LoggingActionFilterAttribute>(messageFormat, null, logLevel).Object;

        var mockLogger = new MockLogger();

        var httpContext = new DefaultHttpContext() { RequestServices = GetServiceProvider(mockLogger.Object) };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor() { DisplayName = actionName });
        var context = new ActionExecutingContext(actionContext, Array.Empty<IFilterMetadata>(), new Dictionary<string, object>(), null);
        context.ActionArguments.Add(actionArgumentName, actionArgument);

        var actionExecutedContext = new ActionExecutedContext(actionContext, Array.Empty<IFilterMetadata>(), null)
        {
            Result = new AcceptedResult()
        };

        ActionExecutionDelegate next = () => Task.FromResult(actionExecutedContext);

        await loggingActionFilter.OnActionExecutionAsync(context, next).ConfigureAwait(false);

        mockLogger.VerifyInfo(string.Format(CultureInfo.CurrentCulture, messageFormat, actionName),
            new { foo = 123, ResultType = nameof(AcceptedResult), ResponseStatusCode = 202 }, Times.Once());
    }

    [Fact(DisplayName = "OnActionExecutionAsync method elevates log level and sets logEntry.Exception from context.Exception if present")]
    public async Task OnActionExecutionAsyncMethodHappyPath2()
    {
        const string messageFormat = "My message format: {0}";
        const LogLevel logLevel = LogLevel.Info;
        const string exceptionMessageFormat = "My exception message format: {0}.";
        const LogLevel exceptionLogLevel = LogLevel.Fatal;
        const string actionName = "MyAction";
        const string actionArgumentName = "foo";
        const int actionArgument = 123;

        var mockActionFilter = new Mock<LoggingActionFilterAttribute>(messageFormat, null, logLevel);
        mockActionFilter.Object.ExceptionMessageFormat = exceptionMessageFormat;
        mockActionFilter.Object.ExceptionLogLevel = exceptionLogLevel;

        IAsyncActionFilter loggingActionFilter = mockActionFilter.Object;

        var mockLogger = new MockLogger();

        var httpContext = new DefaultHttpContext() { RequestServices = GetServiceProvider(mockLogger.Object) };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor() { DisplayName = actionName });
        var context = new ActionExecutingContext(actionContext, Array.Empty<IFilterMetadata>(), new Dictionary<string, object>(), null);
        context.ActionArguments.Add(actionArgumentName, actionArgument);

        var actionExecutedContext = new ActionExecutedContext(actionContext, Array.Empty<IFilterMetadata>(), null);
        var exception = actionExecutedContext.Exception = new NotSupportedException();

        ActionExecutionDelegate next = () => Task.FromResult(actionExecutedContext);

        await loggingActionFilter.OnActionExecutionAsync(context, next).ConfigureAwait(false);

        mockLogger.VerifyFatal(string.Format(CultureInfo.CurrentCulture, exceptionMessageFormat, actionName), exception,
            new { foo = 123, ResponseStatusCode = 500 }, Times.Once());
    }

    [Fact(DisplayName = "OnActionExecutionAsync method adds 'ResultObject' extended property if context.Result is ObjectResult")]
    public async Task OnActionExecutionAsyncMethodHappyPath3()
    {
        const string messageFormat = "My message format: {0}";
        const LogLevel logLevel = LogLevel.Info;
        const string actionName = "MyAction";
        const string actionArgumentName = "foo";
        const int actionArgument = 123;
        const int resultObject = 123;

        IAsyncActionFilter loggingActionFilter = new Mock<LoggingActionFilterAttribute>(messageFormat, null, logLevel).Object;

        var mockLogger = new MockLogger();

        var httpContext = new DefaultHttpContext() { RequestServices = GetServiceProvider(mockLogger.Object) };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor() { DisplayName = actionName });
        var context = new ActionExecutingContext(actionContext, Array.Empty<IFilterMetadata>(), new Dictionary<string, object>(), null);
        context.ActionArguments.Add(actionArgumentName, actionArgument);

        var actionExecutedContext = new ActionExecutedContext(actionContext, Array.Empty<IFilterMetadata>(), null)
        {
            Result = new ObjectResult(resultObject)
        };

        ActionExecutionDelegate next = () => Task.FromResult(actionExecutedContext);

        await loggingActionFilter.OnActionExecutionAsync(context, next).ConfigureAwait(false);

        mockLogger.VerifyInfo(string.Format(CultureInfo.CurrentCulture, messageFormat, actionName),
            new { foo = 123, ResultType = nameof(ObjectResult), ResultObject = resultObject, ResponseStatusCode = 200 }, Times.Once());
    }

    [Fact(DisplayName = "OnActionExecutionAsync method adds [null] 'ResultObject' extended property if context.Result is ObjectResult with null Value")]
    public async Task OnActionExecutionAsyncMethodHappyPath4()
    {
        const string messageFormat = "My message format: {0}";
        const LogLevel logLevel = LogLevel.Info;
        const string actionName = "MyAction";
        const string actionArgumentName = "foo";
        const int actionArgument = 123;

        IAsyncActionFilter loggingActionFilter = new Mock<LoggingActionFilterAttribute>(messageFormat, null, logLevel).Object;

        var mockLogger = new MockLogger();

        var httpContext = new DefaultHttpContext() { RequestServices = GetServiceProvider(mockLogger.Object) };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor() { DisplayName = actionName });
        var context = new ActionExecutingContext(actionContext, Array.Empty<IFilterMetadata>(), new Dictionary<string, object>(), null);
        context.ActionArguments.Add(actionArgumentName, actionArgument);

        var actionExecutedContext = new ActionExecutedContext(actionContext, Array.Empty<IFilterMetadata>(), null)
        {
            Result = new ObjectResult(null)
        };

        ActionExecutionDelegate next = () => Task.FromResult(actionExecutedContext);

        await loggingActionFilter.OnActionExecutionAsync(context, next).ConfigureAwait(false);

        mockLogger.VerifyInfo(string.Format(CultureInfo.CurrentCulture, messageFormat, actionName),
            new { foo = 123, ResultType = nameof(ObjectResult), ResultObject = "[null]" }, Times.Once());
    }

    private static IServiceProvider GetServiceProvider(ILogger logger)
    {
        var services = new ServiceCollection();
        services.AddSingleton(logger);
        services.AddSingleton<LoggerLookup>(loggerName => logger);
        return services.BuildServiceProvider();
    }
}
