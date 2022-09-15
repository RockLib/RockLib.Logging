using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RockLib.Logging.Moq;
using System;
using Xunit;

namespace RockLib.Logging.Microsoft.Extensions.Tests;

public static class RockLibLoggerProviderTests
{
    [Fact(DisplayName = "RockLibLoggerProvider class is decorated with ProviderAliasAttribute")]
    public static void ProviderAliasAttributeTest()
    {
        var attribute = Attribute.GetCustomAttribute(
            typeof(RockLibLoggerProvider), typeof(ProviderAliasAttribute));

        attribute.Should().NotBeNull().And.BeOfType<ProviderAliasAttribute>()
            .Which.Alias.Should().Be(nameof(RockLibLogger));
    }

    [Fact(DisplayName = "Constructor sets Logger property")]
    public static void Create()
    {
        var logger = new MockLogger().Object;

        using var provider = new RockLibLoggerProvider(logger);

        provider.Logger.Should().BeSameAs(logger);
        provider.IncludeScopes.Should().BeFalse();
    }

    [Fact(DisplayName = "Constructor sets IncludeScopes from options")]
    public static void CreateWithOptions()
    {
        Action<RockLibLoggerOptions, string>? capturedCallback = null;

        var options = new RockLibLoggerOptions { IncludeScopes = true };

        var logger = new MockLogger().Object;
        var mockOptionsMonitor = new Mock<IOptionsMonitor<RockLibLoggerOptions>>();
        mockOptionsMonitor.Setup(m => m.OnChange(It.IsAny<Action<RockLibLoggerOptions, string>>()))
            .Returns(new Mock<IDisposable>().Object)
            .Callback(new Action<Action<RockLibLoggerOptions, string>>(OnChange));
        mockOptionsMonitor.Setup(m => m.Get("")).Returns(options);

        using var provider = new RockLibLoggerProvider(logger, mockOptionsMonitor.Object);

        provider.IncludeScopes.Should().BeTrue();
        provider.Logger.Should().BeSameAs(logger);
        capturedCallback.Should().NotBeNull();
        capturedCallback!.Target.Should().BeSameAs(provider);
        capturedCallback.Method.Name.Should().Be("ReloadLoggerOptions");

        void OnChange(Action<RockLibLoggerOptions, string> callback)
        {
            capturedCallback = callback;
        }
    }

    [Fact(DisplayName = "IncludeScopes setter updates ScopeProvider of previously created loggers")]
    public static void IncludeScopesSetter()
    {
        var scopeProvider = new Mock<IExternalScopeProvider>().Object;
        
        using var loggerProvider = new RockLibLoggerProvider(new MockLogger().Object);

        loggerProvider.ScopeProvider = scopeProvider;

        var logger = loggerProvider.GetLogger("Category1");

        logger.ScopeProvider.Should().BeNull();

        loggerProvider.IncludeScopes = true;

        logger.ScopeProvider.Should().BeSameAs(scopeProvider);

        loggerProvider.IncludeScopes = false;
        
        logger.ScopeProvider.Should().BeNull();
    }

    [Fact(DisplayName = "ScopeProvider getter only returns the scope provider when IncludeScopes is true")]
    public static void ScopeProviderGetterWhenIncludeScopesIsTrue()
    {
        using var loggerProvider = new RockLibLoggerProvider(new MockLogger().Object);

        var scopeProvider = new Mock<IExternalScopeProvider>().Object;

        loggerProvider.ScopeProvider = scopeProvider;

        loggerProvider.ScopeProvider.Should().BeNull();

        loggerProvider.IncludeScopes = true;

        loggerProvider.ScopeProvider.Should().BeSameAs(scopeProvider);
    }

    [Fact(DisplayName = "ScopeProvider getter returns a LoggerExternalScopeProvider if not set explicitly")]
    public static void ScopeProviderGetterWhenProviderIsNotSet()
    {
        using var loggerProvider = new RockLibLoggerProvider(new MockLogger().Object);

        loggerProvider.IncludeScopes = true;

        loggerProvider.ScopeProvider.Should().BeOfType<LoggerExternalScopeProvider>();
    }

    [Fact(DisplayName = "ScopeProvider setter updates the ScopeProvider of previously created loggers")]
    public static void ScopeProviderSetterUpdatesProvider()
    {
        var scopeProvider = new Mock<IExternalScopeProvider>().Object;

        using var loggerProvider = new RockLibLoggerProvider(new MockLogger().Object);
        
        loggerProvider.IncludeScopes = true;

        var logger = loggerProvider.GetLogger("Category1");
        
        logger.ScopeProvider.Should().NotBeSameAs(scopeProvider);

        loggerProvider.ScopeProvider = scopeProvider;

        logger.ScopeProvider.Should().BeSameAs(scopeProvider);
    }

    [Fact(DisplayName = "ScopeProvider setter throws if value is null")]
    public static void ScopeProviderSetterToNull()
    {
        using var loggerProvider = new RockLibLoggerProvider(new MockLogger().Object);

        var act = () => loggerProvider.ScopeProvider = null!;

        act.Should().ThrowExactly<ArgumentNullException>().WithMessage("*value*");
    }

    [Fact(DisplayName = "GetLogger method returns a RockLibLogger with the correct properties")]
    public static void GetLoggerMethod()
    {
        var logger = new MockLogger().Object;
        var scopeProvider = new Mock<IExternalScopeProvider>().Object;

        using var provider = new RockLibLoggerProvider(logger);

        provider.IncludeScopes = true;
        provider.ScopeProvider = scopeProvider;

        var rockLibLogger = provider.GetLogger("MyCategoryName");

        rockLibLogger.Logger.Should().BeSameAs(logger);
        rockLibLogger.CategoryName.Should().Be("MyCategoryName");
        rockLibLogger.ScopeProvider.Should().BeSameAs(scopeProvider);
    }

    [Fact(DisplayName = "GetLogger method returns the same RockLibLogger given the same categoryName")]
    public static void GetLoggerMethodWithSameName()
    {
        using var provider = new RockLibLoggerProvider(new MockLogger().Object);

        var logger1 = provider.GetLogger("MyCategoryName");
        var logger2 = provider.GetLogger("MyCategoryName");

        logger1.Should().BeSameAs(logger2);
    }

    [Fact(DisplayName = "GetLogger method throws when categoryName is null")]
    public static void GetLoggerMethodWhenNameIsNull()
    {
        using var provider = new RockLibLoggerProvider(new MockLogger().Object);

        provider.Invoking(m => m.GetLogger(null!))
            .Should().ThrowExactly<ArgumentNullException>().WithMessage("*categoryName*");
    }

    [Fact(DisplayName = "Dispose method disposes _changeReloadToken if options were provided to constructor")]
    public static void Dispose()
    {
        var options = new RockLibLoggerOptions();

        var logger = new MockLogger().Object;
        var mockOptionsMonitor = new Mock<IOptionsMonitor<RockLibLoggerOptions>>();
        var mockChangeReloadToken = new Mock<IDisposable>();
        mockOptionsMonitor.Setup(m => m.OnChange(It.IsAny<Action<RockLibLoggerOptions, string>>()))
            .Returns(mockChangeReloadToken.Object);
        mockOptionsMonitor.Setup(m => m.Get("")).Returns(options);

        using var provider = new RockLibLoggerProvider(logger, mockOptionsMonitor.Object);

        provider.Dispose();

        mockChangeReloadToken.Verify(m => m.Dispose(), Times.Once());
    }

    [Fact(DisplayName = "Dispose method does nothing if options were not provided to constructor")]
    public static void DisposeWithNoConstructorOptions()
    {
        var options = new RockLibLoggerOptions();

        var logger = new MockLogger().Object;

        using var provider = new RockLibLoggerProvider(logger);

        provider.Invoking(m => m.Dispose()).Should().NotThrow();
    }
}
