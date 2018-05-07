using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace RockLib.Logging.Tests
{
    public class LogEntryTests
    {
        [Fact]
        public void LevelIsSetFromConstructor1()
        {
            var logEntry = new LogEntry(LogLevel.Error, "Hello, world!");

            logEntry.Level.Should().Be(LogLevel.Error);
        }

        [Fact]
        public void LevelIsSetFromConstructor2()
        {
            var exception = new Exception();

            var logEntry = new LogEntry(LogLevel.Error, "Hello, world!", exception);

            logEntry.Level.Should().Be(LogLevel.Error);
        }

        [Fact]
        public void MessageIsSetFromConstructor1()
        {
            var logEntry = new LogEntry(LogLevel.Error, "Hello, world!");

            logEntry.Message.Should().Be("Hello, world!");
        }

        [Fact]
        public void MessageIsSetFromConstructor2()
        {
            var exception = new Exception();

            var logEntry = new LogEntry(LogLevel.Error, "Hello, world!", exception);

            logEntry.Message.Should().BeSameAs("Hello, world!");
        }

        [Fact]
        public void ExceptionIsNotSetFromConstructor1()
        {
            var logEntry = new LogEntry(LogLevel.Error, "Hello, world!");

            logEntry.Exception.Should().BeNull();
        }

        [Fact]
        public void ExceptionIsSetFromConstructor2()
        {
            var exception = new Exception();

            var logEntry = new LogEntry(LogLevel.Error, "Hello, world!", exception);

            logEntry.Exception.Should().BeSameAs(exception);
        }

        [Fact]
        public void ExtendedPropertiesAreSetFromConstructor1()
        {
            var foo = 123;
            var bar = "abc";

            var logEntry = new LogEntry(LogLevel.Error, "Hello, world!", new { foo, bar });

            logEntry.ExtendedProperties[nameof(foo)].Should().Be(foo);
            logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
        }

        [Fact]
        public void ExtendedPropertiesAreSetFromConstructor2()
        {
            var foo = 123;
            var bar = "abc";
            var exception = new Exception();

            var logEntry = new LogEntry(LogLevel.Error, "Hello, world!", exception, new { foo, bar });

            logEntry.ExtendedProperties[nameof(foo)].Should().Be(foo);
            logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
        }

        [Fact]
        public void ExtendedPropertiesOfTypeDictionaryAreSetFromConstructor1()
        {
            var foo = 123;
            var bar = "abc";

            var logEntry = new LogEntry(LogLevel.Error, "Hello, world!", new Dictionary<string, object> { { "foo", foo }, { "bar", bar } });

            logEntry.ExtendedProperties[nameof(foo)].Should().Be(foo);
            logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
        }

        [Fact]
        public void ExtendedPropertiesOfTypeDictionaryAreSetFromConstructor2()
        {
            var foo = 123;
            var bar = "abc";
            var exception = new Exception();

            var logEntry = new LogEntry(LogLevel.Error, "Hello, world!", exception, new Dictionary<string, object> { { "foo", foo }, { "bar", bar } });

            logEntry.ExtendedProperties[nameof(foo)].Should().Be(foo);
            logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
        }

        [Fact]
        public void SetExtendedPropertiesMapsObjectPropertiesToExtendedProperties()
        {
            var logEntry = new LogEntry(LogLevel.Error, "Hello, world!");

            var foo = 123;
            var bar = "abc";

            logEntry.SetExtendedProperties(new { foo, bar });

            logEntry.ExtendedProperties[nameof(foo)].Should().Be(foo);
            logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
        }

        [Fact]
        public void SetExtendedPropertiesMapsDictionaryItemsToExtendedProperties()
        {
            var logEntry = new LogEntry(LogLevel.Error, "Hello, world!");

            var foo = 123;
            var bar = "abc";

            logEntry.SetExtendedProperties(new Dictionary<string, object> { { "foo", foo }, { "bar", bar } });

            logEntry.ExtendedProperties[nameof(foo)].Should().Be(foo);
            logEntry.ExtendedProperties[nameof(bar)].Should().BeSameAs(bar);
        }

        [Fact]
        public void GetExceptionDataReturnsNullWhenExceptionIsNull()
        {
            var logEntry = new LogEntry(LogLevel.Error, "Hello, world!");

            logEntry.GetExceptionData().Should().BeNull();
        }

        [Fact]
        public void GetExceptionDataReturnsAValueWhenExceptionIsNotNull()
        {
            var exception = new Exception();

            var logEntry = new LogEntry(LogLevel.Error, "Hello, world!", exception);

            logEntry.GetExceptionData().Should().NotBeNull();
        }
    }
}
