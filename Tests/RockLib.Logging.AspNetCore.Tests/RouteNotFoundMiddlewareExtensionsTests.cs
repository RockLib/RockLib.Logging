using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
#if !NET5_0
using Microsoft.AspNetCore.Builder.Internal;
#endif
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RockLib.Logging.DependencyInjection;
using RockLib.Logging.Moq;
using Xunit;

namespace RockLib.Logging.AspNetCore.Tests;

public class RouteNotFoundMiddlewareExtensionsTests
{
    [Fact(DisplayName = "UseRouteNotFound adds the RouteNotFoundMiddleware to the pipline")]
    public async Task UseRouteNotFoundHappyPath()
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

        var services = new ServiceCollection();
        services.AddSingleton<LoggerLookup>(loggerName => mockLogger.Object);

        var applicationBuilder = new ApplicationBuilder(services.BuildServiceProvider());

        applicationBuilder.UseRouteNotFoundLogging();
        applicationBuilder.Use(async (c, f) => { });

        var pipeline = applicationBuilder.Build();

        await pipeline(httpContextMock.Object);

        mockLogger.Invocations.Count.Should().Be(1);

        var extendedProperties = new Dictionary<string, object>
        {
            [RouteNotFoundMiddleware.RouteExtendedPropertiesKey] = new PathString(path)
        };

        mockLogger.VerifyWarn(RouteNotFoundMiddleware.DefaultLogMessage, extendedProperties);
    }
}
