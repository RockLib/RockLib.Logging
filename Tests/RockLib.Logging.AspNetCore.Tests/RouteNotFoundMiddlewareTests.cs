using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using RockLib.Logging.DependencyInjection;
using RockLib.Logging.Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RockLib.Logging.AspNetCore.Tests
{
    public class RouteNotFoundMiddlewareTests
    {
        [Fact(DisplayName = "Constructor sets correct defaults when options are not present")]
        public void ConstructorHappyPath1()
        {
            var next = new RequestDelegate(async innerContext => { });

            var middleware = new RouteNotFoundMiddleware(next, null);

            middleware.LoggerName.Should().Be(Logger.DefaultName);
            middleware.LogLevel.Should().Be(RouteNotFoundMiddleware.DefaultLogLevel);
            middleware.LogMessage.Should().Be(RouteNotFoundMiddleware.DefaultLogMessage);
        }

        [Fact(DisplayName = "Constructor sets properties when options are present")]
        public void ConstructorHappyPath2()
        {
            var next = new RequestDelegate(async innerContext => { });

            var options = new RouteNotFoundMiddlewareOptions() { LoggerName = "NotDefault", LogLevel = LogLevel.Fatal, LogMessage = "DifferentLogMessage" };

            var mockOptions = new Mock<IOptionsMonitor<RouteNotFoundMiddlewareOptions>>();
            mockOptions.Setup(m => m.CurrentValue).Returns(options);

            var middleware = new RouteNotFoundMiddleware(next, mockOptions.Object);

            middleware.LoggerName.Should().Be(options.LoggerName);
            middleware.LogLevel.Should().Be(options.LogLevel);
            middleware.LogMessage.Should().Be(options.LogMessage);
        }

        [Fact(DisplayName = "Constructor throws when the request delegate is null")]
        public void ConstructorSadPath()
        {
            Action action = () => new RouteNotFoundMiddleware(null);

            action.Should().Throw<ArgumentNullException>().WithMessage("*next*");
        }

        [Fact(DisplayName = "InvokeAsync does nothing when a 404 is not found")]
        public async Task InvokeAsyncHappyPath1()
        {
            var mockLogger = new MockLogger();

            var httpContext = new DefaultHttpContext();
            httpContext.Response.OnStarting(() =>
            {
                httpContext.Response.StatusCode = 200;
                return Task.CompletedTask;
            });

            var next = new RequestDelegate(async innerContext => { });

            var middleware = new RouteNotFoundMiddleware(next);

            await middleware.InvokeAsync(httpContext, loggerName => mockLogger.Object);

            mockLogger.Invocations.Count.Should().Be(0);
        }

        [Fact(DisplayName = "InvokeAsync logs when a 404 is found")]
        public async Task InvokeAsyncHappyPath2()
        {
            var path = "/SomePathThing";
            var mockLogger = new MockLogger();

            var httpResponseMock = new Mock<HttpResponse>();
            httpResponseMock.Setup(hrm => hrm.StatusCode).Returns(404);

            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(requestMock => requestMock.Path).Returns(path);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(hcm => hcm.Response).Returns(httpResponseMock.Object);
            httpContextMock.Setup(hcm => hcm.Request).Returns(httpRequestMock.Object);

            var next = new RequestDelegate(async innerContext => { });

            var middleware = new RouteNotFoundMiddleware(next);

            await middleware.InvokeAsync(httpContextMock.Object, loggerName => mockLogger.Object);

            mockLogger.Invocations.Count.Should().Be(1);

            var extendedProperties = new Dictionary<string, object>
            {
                [RouteNotFoundMiddleware.RouteExtendedPropertiesKey] = new PathString(path)
            };

            mockLogger.VerifyWarn(RouteNotFoundMiddleware.DefaultLogMessage, extendedProperties);
        }
    }
}
