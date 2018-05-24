using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace RockLib.Logging.AspNetCore.Tests
{
    public class AspNetExtensionsTests
    {
        private readonly FieldInfo _nameField;

        public AspNetExtensionsTests()
        {
            _nameField = typeof(RockLibLoggerProvider).GetField("_rockLibLoggerName", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [Fact]
        public void WebHostBuilderExtensionThrowsOnNullBuilder()
        {
            Action action = () => ((IWebHostBuilder)null).UseRockLib();

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: builder");
        }

        [Fact]
        public void WebHostBuilderExtensionAddsProvider()
        {
            ServiceDescriptor serviceDescriptor = null;
            var servicesCollectionMock = new Mock<IServiceCollection>();
            servicesCollectionMock
                .Setup(scm => scm.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(sd => serviceDescriptor = sd);

            var fakeBuilder = new FakeWebHostBuilder()
            {
                ServiceCollection = servicesCollectionMock.Object
            };

            fakeBuilder.UseRockLib("WebHostBuilderRockLibName");

            servicesCollectionMock.Verify(lfm => lfm.Add(It.IsAny<ServiceDescriptor>()), Times.Once());

            _nameField.GetValue(serviceDescriptor.ImplementationFactory.Invoke(null)).Should().Be("WebHostBuilderRockLibName");
        }

        private class FakeWebHostBuilder : IWebHostBuilder
        {
            public IServiceCollection ServiceCollection { get; set; }

            public IWebHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
            {
                configureServices(ServiceCollection);
                return this;
            }

            #region UnusedImplementations
            public IWebHost Build()
            {
                throw new NotImplementedException();
            }

            public IWebHostBuilder ConfigureAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configureDelegate)
            {
                throw new NotImplementedException();
            }

            public IWebHostBuilder ConfigureServices(Action<WebHostBuilderContext, IServiceCollection> configureServices)
            {
                throw new NotImplementedException();
            }

            public string GetSetting(string key)
            {
                throw new NotImplementedException();
            }

            public IWebHostBuilder UseSetting(string key, string value)
            {
                throw new NotImplementedException();
            }
            #endregion
        }
    }
}
