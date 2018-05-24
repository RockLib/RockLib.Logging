using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace RockLib.Logging.Extensions.Tests
{
    public class ConfigureExtensionsTests
    {
        private readonly FieldInfo _nameField;

        public ConfigureExtensionsTests()
        {
            _nameField = typeof(RockLibLoggerProvider).GetField("_rockLibName", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [Fact]
        public void LoggerFactoryExtensionAddsProvider()
        {
            ILoggerProvider rockLibProvider = null;

            var loggerFactoryMock = new Mock<ILoggerFactory>();
            loggerFactoryMock
                .Setup(lfm => lfm.AddProvider(It.IsAny<RockLibLoggerProvider>()))
                .Callback<ILoggerProvider>(rlp => rockLibProvider = rlp);

            loggerFactoryMock.Object.AddRockLib("LoggerFactoryRockLibName");

            loggerFactoryMock.Verify(lfm => lfm.AddProvider(It.IsAny<RockLibLoggerProvider>()), Times.Once());
            _nameField.GetValue(rockLibProvider).Should().Be("LoggerFactoryRockLibName");
        }

        [Fact]
        public void LoggingBuilderExtensionAddsProvider()
        {
            ServiceDescriptor serviceDescriptor = null;

            var servicesCollectionMock = new Mock<IServiceCollection>();
            servicesCollectionMock
                .Setup(scm => scm.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(sd => serviceDescriptor = sd);

            var loggingBuilderMock = new Mock<ILoggingBuilder>();
            loggingBuilderMock
                .Setup(lbm => lbm.Services)
                .Returns(servicesCollectionMock.Object);

            loggingBuilderMock.Object.AddRockLib("LoggingBuilderRockLibName");

            servicesCollectionMock.Verify(lfm => lfm.Add(It.IsAny<ServiceDescriptor>()), Times.Once());

            _nameField.GetValue(serviceDescriptor.ImplementationInstance).Should().Be("LoggingBuilderRockLibName");
        }
    }
}
