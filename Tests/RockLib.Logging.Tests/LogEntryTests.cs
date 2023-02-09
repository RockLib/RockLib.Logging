using FluentAssertions;
using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using Xunit;

namespace RockLib.Logging.Tests;

public sealed class ClassWithIndexer
{
    public int Id { get; set; }
    public string this[int id] { get => string.Empty; }
}

public static class LogEntryTests
{
    [Fact]
    public static void SetExtendedPropertiesWhenTargetHasIndexers()
    {
        var logEntry = new LogEntry();
        var properties = new ClassWithIndexer() { Id = 3 };

        logEntry.SetExtendedProperties(properties);

        logEntry.ExtendedProperties.Count.Should().Be(1);
        logEntry.ExtendedProperties[nameof(ClassWithIndexer.Id)].Should().Be(3);
    }

    [Fact]
    public static void SetSanitizedExtendedPropertiesWhenTargetHasIndexers()
    {
        var logEntry = new LogEntry();
        var properties = new ClassWithIndexer() { Id = 3 };

        logEntry.SetSanitizedExtendedProperties(properties);

        logEntry.ExtendedProperties.Count.Should().Be(1);
        logEntry.ExtendedProperties[nameof(ClassWithIndexer.Id)].Should().Be(3);
    }

    [Fact]
    public static void LevelIsSetFromConstructor1()
    {
        var logEntry = new LogEntry("Hello, world!", LogLevel.Error);

        logEntry.Level.Should().Be(LogLevel.Error);
    }

    [Fact]
    public static void LevelIsSetFromConstructor2()
    {
        var exception = new NotSupportedException();

        var logEntry = new LogEntry("Hello, world!", exception, LogLevel.Error);

        logEntry.Level.Should().Be(LogLevel.Error);
    }

    [Fact]
    public static void MessageIsSetFromConstructor1()
    {
        var logEntry = new LogEntry("Hello, world!", LogLevel.Error);

        logEntry.Message.Should().Be("Hello, world!");
    }

    [Fact]
    public static void MessageIsSetFromConstructor2()
    {
        var exception = new NotSupportedException();

        var logEntry = new LogEntry("Hello, world!", exception, LogLevel.Error);

        logEntry.Message.Should().BeSameAs("Hello, world!");
    }

    [Fact]
    public static void ExceptionIsNotSetFromConstructor1()
    {
        var logEntry = new LogEntry("Hello, world!", LogLevel.Error);

        logEntry.Exception.Should().BeNull();
    }

    [Fact]
    public static void ExceptionIsSetFromConstructor2()
    {
        var exception = new NotSupportedException();

        var logEntry = new LogEntry("Hello, world!", exception, LogLevel.Error);

        logEntry.Exception.Should().BeSameAs(exception);
    }

    [Fact]
    public static void ExtendedPropertiesAreSetFromConstructor1()
    {
        var foo = 123;
        var bar = "abc";

        var logEntry = new LogEntry("Hello, world!", LogLevel.Error, new { foo, bar });

        logEntry.ExtendedProperties[nameof(foo)].Should().Be(foo);
        logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
    }

    [Fact]
    public static void ExtendedPropertiesAreSetFromConstructor2()
    {
        var foo = 123;
        var bar = "abc";
        var exception = new NotSupportedException();

        var logEntry = new LogEntry("Hello, world!", exception, LogLevel.Error, new { foo, bar });

        logEntry.ExtendedProperties[nameof(foo)].Should().Be(foo);
        logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
    }

    [Fact]
    public static void ExtendedPropertiesOfTypeDictionaryAreSetFromConstructor1()
    {
        var foo = 123;
        var bar = "abc";

        var logEntry = new LogEntry("Hello, world!", LogLevel.Error, new Dictionary<string, object> { { "foo", foo }, { "bar", bar } });

        logEntry.ExtendedProperties[nameof(foo)].Should().Be(foo);
        logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
    }

    [Fact]
    public static void ExtendedPropertiesOfTypeDictionaryAreSetFromConstructor2()
    {
        var foo = 123;
        var bar = "abc";
        var exception = new NotSupportedException();

        var logEntry = new LogEntry("Hello, world!", exception, LogLevel.Error, new Dictionary<string, object> { { "foo", foo }, { "bar", bar } });

        logEntry.ExtendedProperties[nameof(foo)].Should().Be(foo);
        logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
    }

    [Fact]
    public static void SetExtendedPropertiesMapsObjectPropertiesToExtendedProperties()
    {
        var logEntry = new LogEntry("Hello, world!", LogLevel.Error);

        var foo = 123;
        var bar = "abc";

        logEntry.SetExtendedProperties(new { foo, bar });

        logEntry.ExtendedProperties[nameof(foo)].Should().Be(foo);
        logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
    }

    [Fact]
    public static void SetExtendedPropertiesMapsStringObjectDictionaryItemsToExtendedProperties()
    {
        var logEntry = new LogEntry("Hello, world!", LogLevel.Error);

        var foo = 123;
        var bar = "abc";

        logEntry.SetExtendedProperties(new Dictionary<string, object> { { nameof(foo), foo }, { nameof(bar), bar } });

        logEntry.ExtendedProperties[nameof(foo)].Should().Be(foo);
        logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
    }

    [Fact]
    public static void SetExtendedPropertiesMapsStringStringDictionaryItemsToExtendedProperties()
    {
        var logEntry = new LogEntry("Hello, world!", LogLevel.Error);

        var foo = "123";
        var bar = "abc";

        logEntry.SetExtendedProperties(new Dictionary<string, string> { { nameof(foo), foo }, { nameof(bar), bar } });

        logEntry.ExtendedProperties[nameof(foo)].Should().BeSameAs(foo);
        logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
    }

    [Fact]
    public static void SetExtendedPropertiesMapsStringIntDictionaryItemsToExtendedProperties()
    {
        var logEntry = new LogEntry("Hello, world!", LogLevel.Error);

        var foo = 123;
        var bar = 456;

        logEntry.SetExtendedProperties(new Dictionary<string, int> { { nameof(foo), foo }, { nameof(bar), bar } });

        logEntry.ExtendedProperties[nameof(foo)].Should().Be(foo);
        logEntry.ExtendedProperties[nameof(bar)].Should().Be(bar);
    }

    [Fact]
    public static void SetExtendedPropertiesMapsStringOtherDictionaryItemsToExtendedProperties()
    {
        var logEntry = new LogEntry("Hello, world!", LogLevel.Error);

        var foo = new Qux(123);
        var bar = new Qux(456);

        logEntry.SetExtendedProperties(new Dictionary<string, Qux> { { nameof(foo), foo }, { nameof(bar), bar } });

        logEntry.ExtendedProperties[nameof(foo)].Should().BeSameAs(foo);
        logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
    }

    [Fact]
    public static void SetExtendedPropertiesMapsHashtableItemsWithStringKeysToExtendedProperties()
    {
        var logEntry = new LogEntry("Hello, world!", LogLevel.Error);

        var foo = 123;
        var bar = "abc";

        logEntry.SetExtendedProperties(new Hashtable { { nameof(foo), foo }, { nameof(bar), bar }, { 123, "this item does not have a string key" } });

        logEntry.ExtendedProperties[nameof(foo)].Should().Be(foo);
        logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
        logEntry.ExtendedProperties.Count.Should().Be(2);
    }

    [Fact]
    public static void SetExtendedPropertiesWithTraceData()
    {
#if NET5_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

#pragma warning disable CA2000 // Dispose objects before losing scope
        var a = new Activity("foo");
#pragma warning restore CA2000 // Dispose objects before losing scope
        a.Start();

        logEntry.SetExtendedProperties(null);

        logEntry.ExtendedProperties["TraceId"].Should().Be(Activity.Current?.TraceId.ToString());
        logEntry.ExtendedProperties["SpanId"].Should().Be(Activity.Current?.SpanId.ToString());
#endif
    }

    [Fact]
    public static void SetExtendedPropertiesWithoutTraceData()
    {
        var logEntry = new LogEntry("Hello, world!", LogLevel.Info);

        logEntry.SetExtendedProperties(null);

        logEntry.ExtendedProperties.Should().NotContainKey("SpanId");
        logEntry.ExtendedProperties.Should().NotContainKey("TraceId");
    }

    [Fact]
    public static void GetExceptionDataReturnsNullWhenExceptionIsNull()
    {
        var logEntry = new LogEntry("Hello, world!", LogLevel.Error);

        logEntry.GetExceptionData().Should().BeNull();
    }

    [Fact]
    public static void GetExceptionDataReturnsAValueWhenExceptionIsNotNull()
    {
        var exception = new NotSupportedException();

        var logEntry = new LogEntry("Hello, world!", exception, LogLevel.Error);

        logEntry.GetExceptionData().Should().NotBeNull();
    }

    private class Qux
    {
        public Qux(int garply) => Garply = garply;
        public int Garply { get; }
    }
}
