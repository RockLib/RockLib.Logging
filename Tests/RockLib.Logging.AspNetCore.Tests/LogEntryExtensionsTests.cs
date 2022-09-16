using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

using static Microsoft.Net.Http.Headers.HeaderNames;
using static RockLib.DistributedTracing.AspNetCore.HeaderNames;

namespace RockLib.Logging.AspNetCore.Tests;

using static HeaderNames;

public class LogEntryExtensionsTests
{
    [Fact(DisplayName = "SetHttpContext sets all HttpContext related extended properties")]
    public void SetHttpContextAddsAllIntendedExtendedProperties()
    {
        var correlationId = "CorrelationId1";
        var forwardedFor = "SomeForwardedForValue1";
        var path = "SomePathValue1";
        var referrer = new Uri("http://SomeReferrerValue1");
        var remoteIpAddress = "10.0.0.1";
        var requestMethod = "SomeRequestMethodValue1";
        var userAgent = "SomeUserAgentValue1";

        var endpointMock = new Mock<IEndpointFeature>();
        endpointMock.Setup(em => em.Endpoint).Returns(new RouteEndpoint(rd => Task.CompletedTask, RoutePatternFactory.Pattern(path), 0, null, null) { });

        var featureMock = new Mock<IFeatureCollection>();
        featureMock.Setup(fm => fm[typeof(IEndpointFeature)]).Returns(endpointMock.Object);

        var connectionMock = new Mock<ConnectionInfo>();
        connectionMock.Setup(cm => cm.RemoteIpAddress).Returns(IPAddress.Parse(remoteIpAddress));

        var headers = new RequestHeaders(new HeaderDictionary());
        headers.Set(CorrelationId, correlationId);
        headers.Set(ForwardedFor, forwardedFor);
        headers.Set(UserAgent, userAgent);
        headers.Referer = referrer;

        var requestMock = new Mock<HttpRequest>();
        requestMock.Setup(rm => rm.Headers).Returns(headers.Headers);
        requestMock.Setup(rm => rm.Method).Returns(requestMethod);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(cm => cm.Request).Returns(requestMock.Object);
        contextMock.Setup(cm => cm.Items).Returns(new Dictionary<object, object>());
        contextMock.Setup(cm => cm.Features).Returns(featureMock.Object);
        contextMock.Setup(cm => cm.Connection).Returns(connectionMock.Object);

        var logEntry = new LogEntry();
        logEntry.SetHttpContext(contextMock.Object);

        logEntry.CorrelationId.Should().Be(correlationId);
        logEntry.ExtendedProperties["X-Forwarded-For"].Should().Be(forwardedFor);
        logEntry.ExtendedProperties["Path"].Should().Be(path);
        logEntry.ExtendedProperties["Referrer"].Should().Be(referrer.ToString());
        logEntry.ExtendedProperties["RemoteIpAddress"].Should().Be(remoteIpAddress);
        logEntry.ExtendedProperties["Method"].Should().Be(requestMethod.ToUpperInvariant());
        logEntry.ExtendedProperties["UserAgent"].Should().Be(userAgent);
    }

    [Fact(DisplayName = "Extensions throws when LogEntry is null")]
    public void SadPaths()
    {
        Action action = () => (null as LogEntry)!.SetHttpContext(new Mock<HttpContext>().Object);
        Action action2 = () => (null as LogEntry)!.SetPath("");
        Action action3 = () => (null as LogEntry)!.SetRequestMethod("");
        Action action4 = () => (null as LogEntry)!.SetUserAgent("");
        Action action5 = () => (null as LogEntry)!.SetReferrer("");
        Action action6 = () => (null as LogEntry)!.SetReferrer("");
        Action action8 = () => (null as LogEntry)!.SetRemoteIpAddress("");
        Action action9 = () => (null as LogEntry)!.SetRemoteIpAddress("");
        Action action10 = () => (null as LogEntry)!.SetForwardedFor("");
        Action action11 = () => (null as LogEntry)!.SetForwardedFor("");
        
        action.Should().Throw<ArgumentNullException>();
        action2.Should().Throw<ArgumentNullException>();
        action3.Should().Throw<ArgumentNullException>();
        action4.Should().Throw<ArgumentNullException>();
        action5.Should().Throw<ArgumentNullException>();
        action6.Should().Throw<ArgumentNullException>();
        action8.Should().Throw<ArgumentNullException>();
        action9.Should().Throw<ArgumentNullException>();
        action10.Should().Throw<ArgumentNullException>();
        action11.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = "SetPath with value sets the path extended property")]
    public void SetPath1()
    {
        var path = "SomePathValue1";
        var logEntry = new LogEntry();

        logEntry.SetPath(path);

        logEntry.ExtendedProperties["Path"].Should().Be(path);
    }

    [Fact(DisplayName = "SetPath with null sets nothing")]
    public void SetPath2()
    {
        var logEntry = new LogEntry();

        logEntry.SetPath(null);

        logEntry.ExtendedProperties.Should().NotContainKey("Path");
    }

    [Fact(DisplayName = "SetRequestMethod with value sets the requestMethod extended property")]
    public void SetRequestMethod1()
    {
        var requestMethod = "SomeRequestMethodValue1";
        var logEntry = new LogEntry();

        logEntry.SetRequestMethod(requestMethod);

        logEntry.ExtendedProperties["Method"].Should().Be(requestMethod.ToUpperInvariant());
    }

    [Fact(DisplayName = "SetRequestMethod with null sets nothing")]
    public void SetRequestMethod2()
    {
        var logEntry = new LogEntry();

        logEntry.SetRequestMethod(null);

        logEntry.ExtendedProperties.Should().NotContainKey("Method");
    }

    [Fact(DisplayName = "SetUserAgent with value sets the userAgent extended property")]
    public void SetUserAgent1()
    {
        var userAgent = "SomeUserAgentValue1";
        var logEntry = new LogEntry();

        logEntry.SetUserAgent(userAgent);

        logEntry.ExtendedProperties["UserAgent"].Should().Be(userAgent);
    }

    [Fact(DisplayName = "SetUserAgent with null sets nothing")]
    public void SetUserAgent2()
    {
        var logEntry = new LogEntry();

        logEntry.SetUserAgent(null);

        logEntry.ExtendedProperties.Should().NotContainKey("UserAgent");
    }

    [Fact(DisplayName = "String SetReferrer with value sets the referrer extended property")]
    public void SetReferrer1()
    {
        var referrer = "SomeReferrerValue1";
        var logEntry = new LogEntry();

        logEntry.SetReferrer(referrer);

        logEntry.ExtendedProperties["Referrer"].Should().Be(referrer);
    }

    [Fact(DisplayName = "String SetReferrer with null sets nothing")]
    public void SetReferrer2()
    {
        var logEntry = new LogEntry();

        logEntry.SetReferrer((string)null!);

        logEntry.ExtendedProperties.Should().NotContainKey("Referrer");
    }

    [Fact(DisplayName = "Uri SetReferrer with value sets the referrer extended property")]
    public void SetReferrer3()
    {
        var referrer = new Uri("http://SomeReferrerValue2");
        var logEntry = new LogEntry();

        logEntry.SetReferrer(referrer);

        logEntry.ExtendedProperties["Referrer"].Should().Be(referrer.ToString());
    }

    [Fact(DisplayName = "Uri SetReferrer with null sets nothing")]
    public void SetReferrer4()
    {
        var logEntry = new LogEntry();

        logEntry.SetReferrer((Uri)null!);

        logEntry.ExtendedProperties.Should().NotContainKey("Referrer");
    }

    [Fact(DisplayName = "String SetRemoteIpAddress with value sets the remoteIpAddress extended property")]
    public void SetRemoteIpAddress1()
    {
        var remoteIpAddress = "SomeRemoteIpAddressValue";
        var logEntry = new LogEntry();

        logEntry.SetRemoteIpAddress(remoteIpAddress);

        logEntry.ExtendedProperties["RemoteIpAddress"].Should().Be(remoteIpAddress);
    }

    [Fact(DisplayName = "String SetRemoteIpAddress with null sets nothing")]
    public void SetRemoteIpAddress2()
    {
        var logEntry = new LogEntry();

        logEntry.SetRemoteIpAddress((string)null!);

        logEntry.ExtendedProperties.Should().NotContainKey("RemoteIpAddress");
    }

    [Fact(DisplayName = "IpAddress SetRemoteIpAddress with value sets the remoteIpAddress extended property")]
    public void SetRemoteIpAddress3()
    {
        var remoteIpAddress = IPAddress.Parse("10.0.0.1");
        var logEntry = new LogEntry();

        logEntry.SetRemoteIpAddress(remoteIpAddress);

        logEntry.ExtendedProperties["RemoteIpAddress"].Should().Be(remoteIpAddress.ToString());
    }

    [Fact(DisplayName = "IpAddress SetRemoteIpAddress with null sets nothing")]
    public void SetRemoteIpAddress4()
    {
        var logEntry = new LogEntry();

        logEntry.SetRemoteIpAddress((IPAddress)null!);

        logEntry.ExtendedProperties.Should().NotContainKey("RemoteIpAddress");
    }

    [Fact(DisplayName = "StringValues SetForwardedFor with value sets the forwardedFor extended property")]
    public void SetForwardedFor1()
    {
        var forwardedFor = new StringValues("SomeForwardedForValue1");
        var logEntry = new LogEntry();

        logEntry.SetForwardedFor(forwardedFor);

        logEntry.ExtendedProperties["X-Forwarded-For"].Should().Be(forwardedFor.ToString());
    }

    [Fact(DisplayName = "String array SetForwardedFor with value sets the forwardedFor extended property")]
    public void SetForwardedFor2()
    {
        var forwardedFor = new string[] { "SomeForwardedForValue1" };
        var logEntry = new LogEntry();

        logEntry.SetForwardedFor(forwardedFor);

        logEntry.ExtendedProperties["X-Forwarded-For"].Should().Be(forwardedFor[0]);
    }

    [Fact(DisplayName = "String array SetForwardedFor with null sets nothing")]
    public void SetForwardedFor3()
    {
        var logEntry = new LogEntry();

        logEntry.SetForwardedFor((string[])null!);

        logEntry.ExtendedProperties.Should().NotContainKey("X-Forwarded-For");
    }
}
