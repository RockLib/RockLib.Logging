using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Primitives;
using Moq;
using RockLib.DistributedTracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

using static Microsoft.Net.Http.Headers.HeaderNames;
using static RockLib.DistributedTracing.AspNetCore.HeaderNames;

namespace RockLib.Logging.AspNetCore.Tests
{
    using static HeaderNames;

    public class ContextProviderTests
    {
        [Fact(DisplayName = "CorrelationIdContextProvider uses IHttpContextAccessor correctly")]
        public void CorrelationIdContextProviderConstructor1()
        {
            var correlationId = new StringValues("CorrelationId1");
            
            var headers = new RequestHeaders(new HeaderDictionary());
            headers.Set(CorrelationId, correlationId);

            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(rm => rm.Headers).Returns(headers.Headers);

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(cm => cm.Request).Returns(requestMock.Object);
            contextMock.Setup(cm => cm.Items).Returns(new Dictionary<object, object>());

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(am => am.HttpContext).Returns(contextMock.Object);

            var contextProvider = new CorrelationIdContextProvider(accessorMock.Object);
            contextProvider.Accessor.CorrelationId.Should().Be(correlationId);
        }

        [Fact(DisplayName = "CorrelationIdContextProvider uses ICorrelationIdAccessor correctly")]
        public void CorrelationIdContextProviderConstructor2()
        {
            var correlationId = "CorrelationId2";
            var idAccessorMock = new Mock<ICorrelationIdAccessor>();
            idAccessorMock.Setup(am => am.CorrelationId).Returns(correlationId);

            var contextProvider = new CorrelationIdContextProvider(idAccessorMock.Object);
            contextProvider.Accessor.CorrelationId.Should().Be(correlationId);
        }

        [Fact(DisplayName = "CorrelationIdContextProvider.AddContext adds correlation id to a LogEntry")]
        public void CorrelationIdContextProviderAddContext()
        {
            var correlationId = "CorrelationId3";
            var idAccessorMock = new Mock<ICorrelationIdAccessor>();
            idAccessorMock.Setup(am => am.CorrelationId).Returns(correlationId);

            var contextProvider = new CorrelationIdContextProvider(idAccessorMock.Object);
            var logEntry = new LogEntry();

            contextProvider.AddContext(logEntry);

            logEntry.CorrelationId.Should().Be(correlationId);
        }

        [Fact(DisplayName = "ForwardedForContextProvider uses IHttpContextAccessor correctly")]
        public void ForwardedForContextProviderConstructor1()
        {
            var forwardedFor = new StringValues("SomeForwardedForValue1");

            var headers = new RequestHeaders(new HeaderDictionary());
            headers.Set(HeaderNames.ForwardedFor, forwardedFor);

            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(rm => rm.Headers).Returns(headers.Headers);

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(cm => cm.Request).Returns(requestMock.Object);
            contextMock.Setup(cm => cm.Items).Returns(new Dictionary<object, object>());

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(am => am.HttpContext).Returns(contextMock.Object);

            var contextProvider = new ForwardedForContextProvider(accessorMock.Object);
            contextProvider.ForwardedFor.Should().BeEquivalentTo(forwardedFor);
        }

        [Fact(DisplayName = "ForwardedForContextProvider sets forwardedfor correctly")]
        public void ForwardedForContextProviderConstructor2()
        {
            var forwardedFor = new StringValues("SomeForwardedForValue2");
            var contextProvider = new ForwardedForContextProvider(forwardedFor);
            contextProvider.ForwardedFor.Should().BeEquivalentTo(forwardedFor);
        }

        [Fact(DisplayName = "ForwardedForContextProvider.SetContext sets forwardedfor correctly on a log entry")]
        public void ForwardedForContextProviderAddContext()
        {
            var forwardedFor = "SomeForwardedForValue3";
            
            var contextProvider = new ForwardedForContextProvider(forwardedFor);
            var logEntry = new LogEntry();

            contextProvider.AddContext(logEntry);

            logEntry.ExtendedProperties["X-Forwarded-For"].Should().Be(forwardedFor);
        }

        [Fact(DisplayName = "PathContextProvider uses IHttpContextAccessor correctly")]
        public void PathContextProviderConstructor1()
        {
            var path = new StringValues("SomePathValue1");

            var endpointMock = new Mock<IEndpointFeature>();
            endpointMock.Setup(em => em.Endpoint).Returns(new RouteEndpoint(rd => Task.CompletedTask, RoutePatternFactory.Pattern(path), 0, null, null) { });

            var featureMock = new Mock<IFeatureCollection>();
            featureMock.Setup(fm => fm[typeof(IEndpointFeature)]).Returns(endpointMock.Object);

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(cm => cm.Features).Returns(featureMock.Object);

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(am => am.HttpContext).Returns(contextMock.Object);

            var contextProvider = new PathContextProvider(accessorMock.Object);
            contextProvider.Path.Should().Be(path);
        }

        [Fact(DisplayName = "PathContextProvider sets path correctly")]
        public void PathContextProviderConstructor2()
        {
            var path = new StringValues("SomePathValue2");
            var contextProvider = new PathContextProvider(path);
            contextProvider.Path.Should().BeEquivalentTo(path);
        }

        [Fact(DisplayName = "PathContextProvider.SetContext sets path correctly on a log entry")]
        public void PathContextProviderAddContext()
        {
            var path = "SomePathValue3";

            var contextProvider = new PathContextProvider(path);
            var logEntry = new LogEntry();

            contextProvider.AddContext(logEntry);

            logEntry.ExtendedProperties["Path"].Should().Be(path);
        }

        [Fact(DisplayName = "ReferrerContextProvider uses IHttpContextAccessor correctly")]
        public void ReferrerContextProviderConstructor1()
        {
            var referrer = new Uri("http://SomeReferrerValue1");

            var headers = new RequestHeaders(new HeaderDictionary());
            headers.Referer = referrer;

            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(rm => rm.Headers).Returns(headers.Headers);

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(cm => cm.Request).Returns(requestMock.Object);

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(am => am.HttpContext).Returns(contextMock.Object);

            var contextProvider = new ReferrerContextProvider(accessorMock.Object);
            contextProvider.Referrer.Should().BeEquivalentTo(referrer);
        }

        [Fact(DisplayName = "ReferrerContextProvider sets Uri referrer correctly")]
        public void ReferrerContextProviderConstructor2()
        {
            var referrer = new Uri("http://SomeReferrerValue2");
            var contextProvider = new ReferrerContextProvider(referrer);
            contextProvider.Referrer.Should().BeEquivalentTo(referrer);
        }

        [Fact(DisplayName = "ReferrerContextProvider sets string referrer correctly")]
        public void ReferrerContextProviderConstructor3()
        {
            var referrer = "http://SomeReferrerValue2/";
            var contextProvider = new ReferrerContextProvider(referrer);
            contextProvider.Referrer.AbsoluteUri.Should().BeEquivalentTo(referrer);
        }

        [Fact(DisplayName = "ReferrerContextProvider.SetContext sets referrer correctly on a log entry")]
        public void ReferrerContextProviderAddContext()
        {
            var referrer = "SomeReferrerValue3";

            var contextProvider = new ReferrerContextProvider(referrer);
            var logEntry = new LogEntry();

            contextProvider.AddContext(logEntry);

            logEntry.ExtendedProperties["Referrer"].Should().Be(referrer);
        }

        [Fact(DisplayName = "RemoteIpAddressContextProvider uses IHttpContextAccessor correctly")]
        public void RemoteIpAddressContextProviderConstructor1()
        {
            var remoteIpAddress = "10.0.0.1";

            var connectionMock = new Mock<ConnectionInfo>();
            connectionMock.Setup(cm => cm.RemoteIpAddress).Returns(IPAddress.Parse(remoteIpAddress));

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(cm => cm.Connection).Returns(connectionMock.Object);

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(am => am.HttpContext).Returns(contextMock.Object);

            var contextProvider = new RemoteIpAddressContextProvider(accessorMock.Object);
            contextProvider.RemoteIpAddress.ToString().Should().Be(remoteIpAddress);
        }

        [Fact(DisplayName = "RemoteIpAddressContextProvider sets Uri remoteIpAddress correctly")]
        public void RemoteIpAddressContextProviderConstructor2()
        {
            var remoteIpAddress = "10.0.0.1";
            var contextProvider = new RemoteIpAddressContextProvider(remoteIpAddress);
            contextProvider.RemoteIpAddress.ToString().Should().BeEquivalentTo(remoteIpAddress);
        }

        [Fact(DisplayName = "RemoteIpAddressContextProvider sets string remoteIpAddress correctly")]
        public void RemoteIpAddressContextProviderConstructor3()
        {
            var remoteIpAddress = IPAddress.Parse("10.0.0.1");
            var contextProvider = new RemoteIpAddressContextProvider(remoteIpAddress);
            contextProvider.RemoteIpAddress.Should().Be(remoteIpAddress);
        }

        [Fact(DisplayName = "RemoteIpAddressContextProvider.SetContext sets remoteIpAddress correctly on a log entry")]
        public void RemoteIpAddressContextProviderAddContext()
        {
            var remoteIpAddress = "10.0.0.1";

            var contextProvider = new RemoteIpAddressContextProvider(remoteIpAddress);
            var logEntry = new LogEntry();

            contextProvider.AddContext(logEntry);

            logEntry.ExtendedProperties["RemoteIpAddress"].Should().Be(remoteIpAddress);
        }

        [Fact(DisplayName = "RequestMethodContextProvider uses IHttpContextAccessor correctly")]
        public void RequestMethodContextProviderConstructor1()
        {
            var requestMethod = "SomeRequestMethodValue1";

            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(rm => rm.Method).Returns(requestMethod);

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(cm => cm.Request).Returns(requestMock.Object);

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(am => am.HttpContext).Returns(contextMock.Object);

            var contextProvider = new RequestMethodContextProvider(accessorMock.Object);
            contextProvider.RequestMethod.ToString().Should().Be(requestMethod);
        }

        [Fact(DisplayName = "RequestMethodContextProvider sets requestMethod correctly")]
        public void RequestMethodContextProviderConstructor2()
        {
            var requestMethod = "SomeRequestMethodValue2";
            var contextProvider = new RequestMethodContextProvider(requestMethod);
            contextProvider.RequestMethod.ToString().Should().BeEquivalentTo(requestMethod);
        }

        [Fact(DisplayName = "RequestMethodContextProvider.SetContext sets requestMethod correctly on a log entry")]
        public void RequestMethodContextProviderAddContext()
        {
            var requestMethod = "SomeRequestMethodValue3";

            var contextProvider = new RequestMethodContextProvider(requestMethod);
            var logEntry = new LogEntry();

            contextProvider.AddContext(logEntry);

            logEntry.ExtendedProperties["Method"].Should().Be(requestMethod.ToUpperInvariant());
        }

        [Fact(DisplayName = "UserAgentContextProvider uses IHttpContextAccessor correctly")]
        public void UserAgentContextProviderConstructor1()
        {
            var userAgent = new StringValues("SomeUserAgentValue1");

            var headers = new RequestHeaders(new HeaderDictionary());
            headers.Set(Microsoft.Net.Http.Headers.HeaderNames.UserAgent, userAgent);

            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(rm => rm.Headers).Returns(headers.Headers);

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(cm => cm.Request).Returns(requestMock.Object);

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(am => am.HttpContext).Returns(contextMock.Object);

            var contextProvider = new UserAgentContextProvider(accessorMock.Object);
            contextProvider.UserAgent.ToString().Should().Be(userAgent);
        }

        [Fact(DisplayName = "UserAgentContextProvider sets userAgent correctly")]
        public void UserAgentContextProviderConstructor2()
        {
            var userAgent = "SomeUserAgentValue2";
            var contextProvider = new UserAgentContextProvider(userAgent);
            contextProvider.UserAgent.ToString().Should().Be(userAgent);
        }

        [Fact(DisplayName = "UserAgentContextProvider.SetContext sets userAgent correctly on a log entry")]
        public void UserAgentContextProviderAddContext()
        {
            var userAgent = "SomeUserAgentValue3";

            var contextProvider = new UserAgentContextProvider(userAgent);
            var logEntry = new LogEntry();

            contextProvider.AddContext(logEntry);

            logEntry.ExtendedProperties["UserAgent"].Should().Be(userAgent);
        }

        [Fact(DisplayName = "HttpContextProvider uses IHttpContextAccessor correctly")]
        public void HttpContextProviderConstructor()
        {
            var correlationId = new StringValues("CorrelationId1");
            var forwardedFor = new StringValues("SomeForwardedForValue1");
            var path = new StringValues("SomePathValue1");
            var referrer = new Uri("http://SomeReferrerValue1");
            var remoteIpAddress = "10.0.0.1";
            var requestMethod = "SomeRequestMethodValue1";
            var userAgent = new StringValues("SomeUserAgentValue1");
            
            var accessorMock = SetUpFullHttpContextAccessor(correlationId, forwardedFor, path, referrer, remoteIpAddress, requestMethod, userAgent);

            var contextProvider = new HttpContextProvider(accessorMock.Object);

            contextProvider.ContextProviders.First(p => p is CorrelationIdContextProvider)
                .As<CorrelationIdContextProvider>().Accessor.CorrelationId.Should().Be(correlationId);
            contextProvider.ContextProviders.First(p => p is ForwardedForContextProvider)
                .As<ForwardedForContextProvider>().ForwardedFor.Should().BeEquivalentTo(forwardedFor);
            contextProvider.ContextProviders.First(p => p is PathContextProvider)
                .As<PathContextProvider>().Path.Should().Be(path);
            contextProvider.ContextProviders.First(p => p is ReferrerContextProvider)
                .As<ReferrerContextProvider>().Referrer.Should().Be(referrer);
            contextProvider.ContextProviders.First(p => p is RemoteIpAddressContextProvider)
                .As<RemoteIpAddressContextProvider>().RemoteIpAddress.ToString().Should().Be(remoteIpAddress);
            contextProvider.ContextProviders.First(p => p is RequestMethodContextProvider)
                .As<RequestMethodContextProvider>().RequestMethod.Should().Be(requestMethod);
            contextProvider.ContextProviders.First(p => p is UserAgentContextProvider)
                .As<UserAgentContextProvider>().UserAgent.Should().Be(userAgent);
        }

        [Fact(DisplayName = "HttpContextProvider uses AddContext of all providers correctly")]
        public void HttpContextProviderAddContext()
        {
            var correlationId = "CorrelationId1";
            var forwardedFor = "SomeForwardedForValue1";
            var path = "SomePathValue1";
            var referrer = new Uri("http://SomeReferrerValue1");
            var remoteIpAddress = "10.0.0.1";
            var requestMethod = "SomeRequestMethodValue1";
            var userAgent = "SomeUserAgentValue1";

            var accessorMock = SetUpFullHttpContextAccessor(correlationId, forwardedFor, path, referrer, remoteIpAddress, requestMethod, userAgent);

            var contextProvider = new HttpContextProvider(accessorMock.Object);

            var logEntry = new LogEntry();

            contextProvider.AddContext(logEntry);

            logEntry.CorrelationId.Should().Be(correlationId);
            logEntry.ExtendedProperties["X-Forwarded-For"].Should().Be(forwardedFor);
            logEntry.ExtendedProperties["Path"].Should().Be(path);
            logEntry.ExtendedProperties["Referrer"].Should().Be(referrer.ToString());
            logEntry.ExtendedProperties["RemoteIpAddress"].Should().Be(remoteIpAddress);
            logEntry.ExtendedProperties["Method"].Should().Be(requestMethod.ToUpperInvariant());
            logEntry.ExtendedProperties["UserAgent"].Should().Be(userAgent);
        }

        private static Mock<IHttpContextAccessor> SetUpFullHttpContextAccessor(StringValues correlationId, StringValues forwardedFor, StringValues path, Uri referrer, string remoteIpAddress, string requestMethod, StringValues userAgent)
        {
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

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(am => am.HttpContext).Returns(contextMock.Object);
            return accessorMock;
        }
    }
}
