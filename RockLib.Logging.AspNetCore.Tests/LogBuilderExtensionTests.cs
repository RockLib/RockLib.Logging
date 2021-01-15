using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RockLib.Logging.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace RockLib.Logging.AspNetCore.Tests
{
    public class LogBuilderExtensionTests
    {
        [Fact(DisplayName = "AddHttpContextProvider adds HttpContextProvider")]
        public void AddHttpContextProviderExtension()
        {
            var contextMock = new Mock<IHttpContextAccessor>();
            var loggerBuilder = new TestLoggerBuilder();

            var serviceProvider = new ServiceCollection()
                .AddSingleton(contextMock.Object)
                .BuildServiceProvider();

            loggerBuilder.AddHttpContextProvider();

            var registration = loggerBuilder.ContextProviderRegistrations.Should().ContainSingle().Subject;

            var contextProvider = registration.Invoke(serviceProvider);
            contextProvider.Should().BeOfType<HttpContextProvider>();
        }

        private class TestLoggerBuilder : ILoggerBuilder
        {
            public string LoggerName => Logger.DefaultName;

            public IList<Func<IServiceProvider, ILogProvider>> LogProviderRegistrations { get; } = new List<Func<IServiceProvider, ILogProvider>>();

            public IList<Func<IServiceProvider, IContextProvider>> ContextProviderRegistrations { get; } = new List<Func<IServiceProvider, IContextProvider>>();

            public ILoggerBuilder AddLogProvider(Func<IServiceProvider, ILogProvider> logProviderRegistration)
            {
                LogProviderRegistrations.Add(logProviderRegistration);
                return this;
            }

            public ILoggerBuilder AddContextProvider(Func<IServiceProvider, IContextProvider> contextProviderRegistration)
            {
                ContextProviderRegistrations.Add(contextProviderRegistration);
                return this;
            }
        }
    }
}
