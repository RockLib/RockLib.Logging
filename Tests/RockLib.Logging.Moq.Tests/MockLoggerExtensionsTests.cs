using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace RockLib.Logging.Moq.Tests
{
    public class MockLoggerExtensionsTests
    {
        [Fact(DisplayName = "SetupLogger method adds setups for Level and Name properties")]
        public void SetupLoggerMethodHappyPath()
        {
            var mockLogger = new Mock<ILogger>();

            mockLogger.SetupLogger(LogLevel.Warn, "TestLogger");

            mockLogger.Setups.Should().HaveCount(2);
            mockLogger.Object.Level.Should().Be(LogLevel.Warn);
            mockLogger.Object.Name.Should().Be("TestLogger");
        }

        [Fact(DisplayName = "VerifyDebug method 1 verifies successfully when criteria are met")]
        public void VerifyDebugMethod1HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!");

            Action act = () => mockLogger.VerifyDebug(Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyDebug method 1 throws MockException when criteria are not met - wrong Times")]
        public void VerifyDebugMethod1SadPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!");

            Action act = () => mockLogger.VerifyDebug(Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 2 verifies successfully when criteria are met")]
        public void VerifyDebugMethod2HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!");

            Action act = () => mockLogger.VerifyDebug(@"/(?i)hello,\s+world[.!?]/", Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyDebug method 2 throws MockException when criteria are not met - wrong Times")]
        public void VerifyDebugMethod2SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!");

            Action act = () => mockLogger.VerifyDebug("Hello, world!", Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 2 throws MockException when criteria are not met - wrong message")]
        public void VerifyDebugMethod2SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Good-bye, cruel world!");

            Action act = () => mockLogger.VerifyDebug(@"/(?i)hello,\s+world[.!?]/", Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 3 verifies successfully when criteria are met")]
        public void VerifyDebugMethod3HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyDebug(new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyDebug method 3 throws MockException when criteria are not met - wrong Times")]
        public void VerifyDebugMethod3SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyDebug(new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 3 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyDebugMethod3SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyDebug(new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 3 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyDebugMethod3SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!", new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyDebug(new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 4 verifies successfully when criteria are met")]
        public void VerifyDebugMethod4HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyDebug("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyDebug method 4 throws MockException when criteria are not met - wrong Times")]
        public void VerifyDebugMethod4SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyDebug(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 4 throws MockException when criteria are not met - wrong message")]
        public void VerifyDebugMethod4SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Good-bye, cruel world!", new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyDebug("Hello, world!", new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 4 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyDebugMethod4SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!", new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyDebug(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123, Bar ="/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 4 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyDebugMethod4SadPath4()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!", new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyDebug("Hello, world!", new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 5 verifies successfully when criteria are met")]
        public void VerifyDebugMethod5HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception);

            Action act = () => mockLogger.VerifyDebug(exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyDebug method 5 throws MockException when criteria are not met - wrong Times")]
        public void VerifyDebugMethod5SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception);

            Action act = () => mockLogger.VerifyDebug(exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 5 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyDebugMethod5SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception);

            Action act = () => mockLogger.VerifyDebug(wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 6 verifies successfully when criteria are met")]
        public void VerifyDebugMethod6HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception);

            Action act = () => mockLogger.VerifyDebug(@"/(?i)hello,\s+world[.!?]/", exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyDebug method 6 throws MockException when criteria are not met - wrong Times")]
        public void VerifyDebugMethod6SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception);

            Action act = () => mockLogger.VerifyDebug("Hello, world!", exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 6 throws MockException when criteria are not met - wrong message")]
        public void VerifyDebugMethod6SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Good-bye, cruel world!", exception);

            Action act = () => mockLogger.VerifyDebug(@"/(?i)hello,\s+world[.!?]/", exception, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 6 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyDebugMethod6SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Debug("Hello world!", exception);

            Action act = () => mockLogger.VerifyDebug("Hello, world!", wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 7 verifies successfully when criteria are met")]
        public void VerifyDebugMethod7HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyDebug(exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyDebug method 7 throws MockException when criteria are not met - wrong Times")]
        public void VerifyDebugMethod7SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyDebug(exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 7 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyDebugMethod7SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyDebug(exception, new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 7 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyDebugMethod7SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyDebug(exception, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 7 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyDebugMethod7SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyDebug(wrongException, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 8 verifies successfully when criteria are met")]
        public void VerifyDebugMethod8HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyDebug(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyDebug method 8 throws MockException when criteria are not met - wrong Times")]
        public void VerifyDebugMethod8SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyDebug("Hello, world!", exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 8 throws MockException when criteria are not met - wrong message")]
        public void VerifyDebugMethod8SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Good-bye, cruel world!", exception, new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyDebug(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 8 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyDebugMethod8SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyDebug("Hello, world!", exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 8 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyDebugMethod8SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyDebug(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 8 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyDebugMethod8SadPath5()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyDebug("Hello, world!", wrongException, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 9 verifies successfully when criteria are met")]
        public void VerifyDebugMethod9HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception);

            Action act = () => mockLogger.VerifyDebug(ex => ex == exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyDebug method 9 throws MockException when criteria are not met - wrong Times")]
        public void VerifyDebugMethod9SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception);

            Action act = () => mockLogger.VerifyDebug(ex => ex == exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 9 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyDebugMethod9SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception);

            Action act = () => mockLogger.VerifyDebug(ex => ex == wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 10 verifies successfully when criteria are met")]
        public void VerifyDebugMethod10HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception);

            Action act = () => mockLogger.VerifyDebug(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyDebug method 10 throws MockException when criteria are not met - wrong Times")]
        public void VerifyDebugMethod10SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception);

            Action act = () => mockLogger.VerifyDebug("Hello, world!", ex => ex == exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 10 throws MockException when criteria are not met - wrong message")]
        public void VerifyDebugMethod10SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Good-bye, cruel world!", exception);

            Action act = () => mockLogger.VerifyDebug(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 10 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyDebugMethod10SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Debug("Hello world!", exception);

            Action act = () => mockLogger.VerifyDebug("Hello, world!", ex => ex == wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 11 verifies successfully when criteria are met")]
        public void VerifyDebugMethod11HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyDebug(ex => ex == exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyDebug method 11 throws MockException when criteria are not met - wrong Times")]
        public void VerifyDebugMethod11SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyDebug(ex => ex == exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 1 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyDebugMethod11SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyDebug(ex => ex == exception, new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 11 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyDebugMethod11SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyDebug(ex => ex == exception, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 11 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyDebugMethod11SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyDebug(ex => ex == wrongException, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 12 verifies successfully when criteria are met")]
        public void VerifyDebugMethod12HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyDebug(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyDebug method 12 throws MockException when criteria are not met - wrong Times")]
        public void VerifyDebugMethod12SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyDebug("Hello, world!", ex => ex == exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 12 throws MockException when criteria are not met - wrong message")]
        public void VerifyDebugMethod12SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Good-bye, cruel world!", exception, new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyDebug(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 12 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyDebugMethod12SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyDebug("Hello, world!", ex => ex == exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 12 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyDebugMethod12SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyDebug(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 12 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyDebugMethod12SadPath5()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Debug("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyDebug("Hello, world!", ex => ex == wrongException, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 1 verifies successfully when criteria are met")]
        public void VerifyInfoMethod1HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!");

            Action act = () => mockLogger.VerifyInfo(Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyInfo method 1 throws MockException when criteria are not met - wrong Times")]
        public void VerifyInfoMethod1SadPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!");

            Action act = () => mockLogger.VerifyInfo(Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 2 verifies successfully when criteria are met")]
        public void VerifyInfoMethod2HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!");

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyInfo method 2 throws MockException when criteria are not met - wrong Times")]
        public void VerifyInfoMethod2SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!");

            Action act = () => mockLogger.VerifyInfo("Hello, world!", Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 2 throws MockException when criteria are not met - wrong message")]
        public void VerifyInfoMethod2SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Good-bye, cruel world!");

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 3 verifies successfully when criteria are met")]
        public void VerifyInfoMethod3HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyInfo(new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyInfo method 3 throws MockException when criteria are not met - wrong Times")]
        public void VerifyInfoMethod3SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyInfo(new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 3 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyInfoMethod3SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyInfo(new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 3 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyInfoMethod3SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyInfo(new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 4 verifies successfully when criteria are met")]
        public void VerifyInfoMethod4HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyInfo("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyInfo method 4 throws MockException when criteria are not met - wrong Times")]
        public void VerifyInfoMethod4SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 4 throws MockException when criteria are not met - wrong message")]
        public void VerifyInfoMethod4SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Good-bye, cruel world!", new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyInfo("Hello, world!", new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 4 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyInfoMethod4SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 4 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyInfoMethod4SadPath4()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 5 verifies successfully when criteria are met")]
        public void VerifyInfoMethod5HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyInfo(exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyInfo method 5 throws MockException when criteria are not met - wrong Times")]
        public void VerifyInfoMethod5SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyInfo(exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 5 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyInfoMethod5SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyInfo(wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 6 verifies successfully when criteria are met")]
        public void VerifyInfoMethod6HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyInfo("Hello, world!", exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyInfo method 6 throws MockException when criteria are not met - wrong Times")]
        public void VerifyInfoMethod6SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 6 throws MockException when criteria are not met - wrong message")]
        public void VerifyInfoMethod6SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Good-bye, cruel world!", exception);

            Action act = () => mockLogger.VerifyInfo("Hello, world!", exception, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 6 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyInfoMethod6SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Info("Hello world!", exception);

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 7 verifies successfully when criteria are met")]
        public void VerifyInfoMethod7HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyInfo(exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyInfo method 7 throws MockException when criteria are not met - wrong Times")]
        public void VerifyInfoMethod7SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyInfo(exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 7 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyInfoMethod7SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyInfo(exception, new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 7 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyInfoMethod7SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyInfo(exception, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 7 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyInfoMethod7SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyInfo(wrongException, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 8 verifies successfully when criteria are met")]
        public void VerifyInfoMethod8HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyInfo method 8 throws MockException when criteria are not met - wrong Times")]
        public void VerifyInfoMethod8SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyInfo("Hello, world!", exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 8 throws MockException when criteria are not met - wrong message")]
        public void VerifyInfoMethod8SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Good-bye, cruel world!", exception, new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 8 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyInfoMethod8SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyInfo("Hello, world!", exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 8 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyInfoMethod8SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 8 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyInfoMethod8SadPath5()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyInfo("Hello, world!", wrongException, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 9 verifies successfully when criteria are met")]
        public void VerifyInfoMethod9HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyInfo(ex => ex == exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyInfo method 9 throws MockException when criteria are not met - wrong Times")]
        public void VerifyInfoMethod9SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyInfo(ex => ex == exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 9 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyInfoMethod9SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyInfo(ex => ex == wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 10 verifies successfully when criteria are met")]
        public void VerifyInfoMethod10HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyInfo method 10 throws MockException when criteria are not met - wrong Times")]
        public void VerifyInfoMethod10SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyInfo("Hello, world!", ex => ex == exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 10 throws MockException when criteria are not met - wrong message")]
        public void VerifyInfoMethod10SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Good-bye, cruel world!", exception);

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 10 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyInfoMethod10SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Info("Hello world!", exception);

            Action act = () => mockLogger.VerifyInfo("Hello, world!", ex => ex == wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 11 verifies successfully when criteria are met")]
        public void VerifyInfoMethod11HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyInfo(ex => ex == exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyInfo method 11 throws MockException when criteria are not met - wrong Times")]
        public void VerifyInfoMethod11SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyInfo(ex => ex == exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 1 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyInfoMethod11SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyInfo(ex => ex == exception, new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 11 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyInfoMethod11SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyInfo(ex => ex == exception, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 11 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyInfoMethod11SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyInfo(ex => ex == wrongException, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 12 verifies successfully when criteria are met")]
        public void VerifyInfoMethod12HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyInfo method 12 throws MockException when criteria are not met - wrong Times")]
        public void VerifyInfoMethod12SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyInfo("Hello, world!", ex => ex == exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 12 throws MockException when criteria are not met - wrong message")]
        public void VerifyInfoMethod12SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Good-bye, cruel world!", exception, new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 12 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyInfoMethod12SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyInfo("Hello, world!", ex => ex == exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 12 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyInfoMethod12SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyInfo(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 12 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyInfoMethod12SadPath5()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyInfo("Hello, world!", ex => ex == wrongException, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 1 verifies successfully when criteria are met")]
        public void VerifyWarnMethod1HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!");

            Action act = () => mockLogger.VerifyWarn(Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyWarn method 1 throws MockException when criteria are not met - wrong Times")]
        public void VerifyWarnMethod1SadPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!");

            Action act = () => mockLogger.VerifyWarn(Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 2 verifies successfully when criteria are met")]
        public void VerifyWarnMethod2HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!");

            Action act = () => mockLogger.VerifyWarn(@"/(?i)hello,\s+world[.!?]/", Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyWarn method 2 throws MockException when criteria are not met - wrong Times")]
        public void VerifyWarnMethod2SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!");

            Action act = () => mockLogger.VerifyWarn("Hello, world!", Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 2 throws MockException when criteria are not met - wrong message")]
        public void VerifyWarnMethod2SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Good-bye, cruel world!");

            Action act = () => mockLogger.VerifyWarn(@"/(?i)hello,\s+world[.!?]/", Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 3 verifies successfully when criteria are met")]
        public void VerifyWarnMethod3HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyWarn(new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyWarn method 3 throws MockException when criteria are not met - wrong Times")]
        public void VerifyWarnMethod3SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyWarn(new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 3 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyWarnMethod3SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyWarn(new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 3 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyWarnMethod3SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!", new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyWarn(new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 4 verifies successfully when criteria are met")]
        public void VerifyWarnMethod4HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyWarn("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyWarn method 4 throws MockException when criteria are not met - wrong Times")]
        public void VerifyWarnMethod4SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyWarn(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 4 throws MockException when criteria are not met - wrong message")]
        public void VerifyWarnMethod4SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Good-bye, cruel world!", new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyWarn("Hello, world!", new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 4 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyWarnMethod4SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!", new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyWarn(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 4 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyWarnMethod4SadPath4()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!", new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyWarn("Hello, world!", new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 5 verifies successfully when criteria are met")]
        public void VerifyWarnMethod5HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception);

            Action act = () => mockLogger.VerifyWarn(exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyWarn method 5 throws MockException when criteria are not met - wrong Times")]
        public void VerifyWarnMethod5SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception);

            Action act = () => mockLogger.VerifyWarn(exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 5 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyWarnMethod5SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception);

            Action act = () => mockLogger.VerifyWarn(wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 6 verifies successfully when criteria are met")]
        public void VerifyWarnMethod6HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception);

            Action act = () => mockLogger.VerifyWarn(@"/(?i)hello,\s+world[.!?]/", exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyWarn method 6 throws MockException when criteria are not met - wrong Times")]
        public void VerifyWarnMethod6SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception);

            Action act = () => mockLogger.VerifyWarn("Hello, world!", exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 6 throws MockException when criteria are not met - wrong message")]
        public void VerifyWarnMethod6SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Good-bye, cruel world!", exception);

            Action act = () => mockLogger.VerifyWarn(@"/(?i)hello,\s+world[.!?]/", exception, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 6 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyWarnMethod6SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Warn("Hello world!", exception);

            Action act = () => mockLogger.VerifyWarn("Hello, world!", wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 7 verifies successfully when criteria are met")]
        public void VerifyWarnMethod7HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyWarn(exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyWarn method 7 throws MockException when criteria are not met - wrong Times")]
        public void VerifyWarnMethod7SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyWarn(exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 7 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyWarnMethod7SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyWarn(exception, new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 7 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyWarnMethod7SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyWarn(exception, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 7 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyWarnMethod7SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyWarn(wrongException, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 8 verifies successfully when criteria are met")]
        public void VerifyWarnMethod8HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyWarn(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyWarn method 8 throws MockException when criteria are not met - wrong Times")]
        public void VerifyWarnMethod8SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyWarn("Hello, world!", exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 8 throws MockException when criteria are not met - wrong message")]
        public void VerifyWarnMethod8SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Good-bye, cruel world!", exception, new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyWarn(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 8 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyWarnMethod8SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyWarn("Hello, world!", exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 8 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyWarnMethod8SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyWarn(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 8 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyWarnMethod8SadPath5()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyWarn("Hello, world!", wrongException, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 9 verifies successfully when criteria are met")]
        public void VerifyWarnMethod9HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception);

            Action act = () => mockLogger.VerifyWarn(ex => ex == exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyWarn method 9 throws MockException when criteria are not met - wrong Times")]
        public void VerifyWarnMethod9SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception);

            Action act = () => mockLogger.VerifyWarn(ex => ex == exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 9 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyWarnMethod9SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception);

            Action act = () => mockLogger.VerifyWarn(ex => ex == wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 10 verifies successfully when criteria are met")]
        public void VerifyWarnMethod10HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception);

            Action act = () => mockLogger.VerifyWarn(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyWarn method 10 throws MockException when criteria are not met - wrong Times")]
        public void VerifyWarnMethod10SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception);

            Action act = () => mockLogger.VerifyWarn("Hello, world!", ex => ex == exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 10 throws MockException when criteria are not met - wrong message")]
        public void VerifyWarnMethod10SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Good-bye, cruel world!", exception);

            Action act = () => mockLogger.VerifyWarn(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 10 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyWarnMethod10SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Warn("Hello world!", exception);

            Action act = () => mockLogger.VerifyWarn("Hello, world!", ex => ex == wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 11 verifies successfully when criteria are met")]
        public void VerifyWarnMethod11HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyWarn(ex => ex == exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyWarn method 11 throws MockException when criteria are not met - wrong Times")]
        public void VerifyWarnMethod11SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyWarn(ex => ex == exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 1 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyWarnMethod11SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyWarn(ex => ex == exception, new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 11 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyWarnMethod11SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyWarn(ex => ex == exception, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 11 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyWarnMethod11SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyWarn(ex => ex == wrongException, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 12 verifies successfully when criteria are met")]
        public void VerifyWarnMethod12HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyWarn(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyWarn method 12 throws MockException when criteria are not met - wrong Times")]
        public void VerifyWarnMethod12SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyWarn("Hello, world!", ex => ex == exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 12 throws MockException when criteria are not met - wrong message")]
        public void VerifyWarnMethod12SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Good-bye, cruel world!", exception, new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyWarn(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 12 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyWarnMethod12SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyWarn("Hello, world!", ex => ex == exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 12 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyWarnMethod12SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyWarn(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 12 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyWarnMethod12SadPath5()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Warn("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyWarn("Hello, world!", ex => ex == wrongException, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 1 verifies successfully when criteria are met")]
        public void VerifyErrorMethod1HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!");

            Action act = () => mockLogger.VerifyError(Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyError method 1 throws MockException when criteria are not met - wrong Times")]
        public void VerifyErrorMethod1SadPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!");

            Action act = () => mockLogger.VerifyError(Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyError method 2 verifies successfully when criteria are met")]
        public void VerifyErrorMethod2HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!");

            Action act = () => mockLogger.VerifyError(@"/(?i)hello,\s+world[.!?]/", Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyError method 2 throws MockException when criteria are not met - wrong Times")]
        public void VerifyErrorMethod2SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!");

            Action act = () => mockLogger.VerifyError("Hello, world!", Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyError method 2 throws MockException when criteria are not met - wrong message")]
        public void VerifyErrorMethod2SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Good-bye, cruel world!");

            Action act = () => mockLogger.VerifyError(@"/(?i)hello,\s+world[.!?]/", Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 3 verifies successfully when criteria are met")]
        public void VerifyErrorMethod3HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyError(new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyError method 3 throws MockException when criteria are not met - wrong Times")]
        public void VerifyErrorMethod3SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyError(new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyError method 3 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyErrorMethod3SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyError(new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 3 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyErrorMethod3SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!", new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyError(new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 4 verifies successfully when criteria are met")]
        public void VerifyErrorMethod4HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyError("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyError method 4 throws MockException when criteria are not met - wrong Times")]
        public void VerifyErrorMethod4SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyError(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyError method 4 throws MockException when criteria are not met - wrong message")]
        public void VerifyErrorMethod4SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Good-bye, cruel world!", new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyError("Hello, world!", new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 4 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyErrorMethod4SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!", new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyError(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 4 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyErrorMethod4SadPath4()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!", new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyError("Hello, world!", new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 5 verifies successfully when criteria are met")]
        public void VerifyErrorMethod5HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception);

            Action act = () => mockLogger.VerifyError(exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyError method 5 throws MockException when criteria are not met - wrong Times")]
        public void VerifyErrorMethod5SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception);

            Action act = () => mockLogger.VerifyError(exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyError method 5 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyErrorMethod5SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Error("Hello, world!", exception);

            Action act = () => mockLogger.VerifyError(wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 6 verifies successfully when criteria are met")]
        public void VerifyErrorMethod6HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception);

            Action act = () => mockLogger.VerifyError(@"/(?i)hello,\s+world[.!?]/", exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyError method 6 throws MockException when criteria are not met - wrong Times")]
        public void VerifyErrorMethod6SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception);

            Action act = () => mockLogger.VerifyError("Hello, world!", exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyError method 6 throws MockException when criteria are not met - wrong message")]
        public void VerifyErrorMethod6SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Good-bye, cruel world!", exception);

            Action act = () => mockLogger.VerifyError(@"/(?i)hello,\s+world[.!?]/", exception, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 6 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyErrorMethod6SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Error("Hello world!", exception);

            Action act = () => mockLogger.VerifyError("Hello, world!", wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 7 verifies successfully when criteria are met")]
        public void VerifyErrorMethod7HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyError(exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyError method 7 throws MockException when criteria are not met - wrong Times")]
        public void VerifyErrorMethod7SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyError(exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyError method 7 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyErrorMethod7SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyError(exception, new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 7 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyErrorMethod7SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyError(exception, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 7 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyErrorMethod7SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyError(wrongException, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 8 verifies successfully when criteria are met")]
        public void VerifyErrorMethod8HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyError(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyError method 8 throws MockException when criteria are not met - wrong Times")]
        public void VerifyErrorMethod8SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyError("Hello, world!", exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyError method 8 throws MockException when criteria are not met - wrong message")]
        public void VerifyErrorMethod8SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Good-bye, cruel world!", exception, new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyError(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 8 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyErrorMethod8SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyError("Hello, world!", exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 8 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyErrorMethod8SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyError(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 8 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyErrorMethod8SadPath5()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyError("Hello, world!", wrongException, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 9 verifies successfully when criteria are met")]
        public void VerifyErrorMethod9HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception);

            Action act = () => mockLogger.VerifyError(ex => ex == exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyError method 9 throws MockException when criteria are not met - wrong Times")]
        public void VerifyErrorMethod9SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception);

            Action act = () => mockLogger.VerifyError(ex => ex == exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyError method 9 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyErrorMethod9SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Error("Hello, world!", exception);

            Action act = () => mockLogger.VerifyError(ex => ex == wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 10 verifies successfully when criteria are met")]
        public void VerifyErrorMethod10HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception);

            Action act = () => mockLogger.VerifyError(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyError method 10 throws MockException when criteria are not met - wrong Times")]
        public void VerifyErrorMethod10SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception);

            Action act = () => mockLogger.VerifyError("Hello, world!", ex => ex == exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyError method 10 throws MockException when criteria are not met - wrong message")]
        public void VerifyErrorMethod10SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Good-bye, cruel world!", exception);

            Action act = () => mockLogger.VerifyError(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 10 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyErrorMethod10SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Error("Hello world!", exception);

            Action act = () => mockLogger.VerifyError("Hello, world!", ex => ex == wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 11 verifies successfully when criteria are met")]
        public void VerifyErrorMethod11HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyError(ex => ex == exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyError method 11 throws MockException when criteria are not met - wrong Times")]
        public void VerifyErrorMethod11SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyError(ex => ex == exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyError method 1 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyErrorMethod11SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyError(ex => ex == exception, new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 11 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyErrorMethod11SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyError(ex => ex == exception, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 11 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyErrorMethod11SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyError(ex => ex == wrongException, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 12 verifies successfully when criteria are met")]
        public void VerifyErrorMethod12HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyError(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyError method 12 throws MockException when criteria are not met - wrong Times")]
        public void VerifyErrorMethod12SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyError("Hello, world!", ex => ex == exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyError method 12 throws MockException when criteria are not met - wrong message")]
        public void VerifyErrorMethod12SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Good-bye, cruel world!", exception, new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyError(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 12 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyErrorMethod12SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyError("Hello, world!", ex => ex == exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 12 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyErrorMethod12SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyError(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 12 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyErrorMethod12SadPath5()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Error("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyError("Hello, world!", ex => ex == wrongException, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 1 verifies successfully when criteria are met")]
        public void VerifyFatalMethod1HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!");

            Action act = () => mockLogger.VerifyFatal(Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyFatal method 1 throws MockException when criteria are not met - wrong Times")]
        public void VerifyFatalMethod1SadPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!");

            Action act = () => mockLogger.VerifyFatal(Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 2 verifies successfully when criteria are met")]
        public void VerifyFatalMethod2HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!");

            Action act = () => mockLogger.VerifyFatal(@"/(?i)hello,\s+world[.!?]/", Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyFatal method 2 throws MockException when criteria are not met - wrong Times")]
        public void VerifyFatalMethod2SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!");

            Action act = () => mockLogger.VerifyFatal("Hello, world!", Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 2 throws MockException when criteria are not met - wrong message")]
        public void VerifyFatalMethod2SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Good-bye, cruel world!");

            Action act = () => mockLogger.VerifyFatal(@"/(?i)hello,\s+world[.!?]/", Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 3 verifies successfully when criteria are met")]
        public void VerifyFatalMethod3HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyFatal(new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyFatal method 3 throws MockException when criteria are not met - wrong Times")]
        public void VerifyFatalMethod3SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyFatal(new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 3 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyFatalMethod3SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyFatal(new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 3 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyFatalMethod3SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!", new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyFatal(new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 4 verifies successfully when criteria are met")]
        public void VerifyFatalMethod4HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyFatal("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyFatal method 4 throws MockException when criteria are not met - wrong Times")]
        public void VerifyFatalMethod4SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyFatal(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 4 throws MockException when criteria are not met - wrong message")]
        public void VerifyFatalMethod4SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Good-bye, cruel world!", new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyFatal("Hello, world!", new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 4 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyFatalMethod4SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!", new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyFatal(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 4 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyFatalMethod4SadPath4()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!", new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyFatal("Hello, world!", new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 5 verifies successfully when criteria are met")]
        public void VerifyFatalMethod5HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception);

            Action act = () => mockLogger.VerifyFatal(exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyFatal method 5 throws MockException when criteria are not met - wrong Times")]
        public void VerifyFatalMethod5SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception);

            Action act = () => mockLogger.VerifyFatal(exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 5 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyFatalMethod5SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception);

            Action act = () => mockLogger.VerifyFatal(wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 6 verifies successfully when criteria are met")]
        public void VerifyFatalMethod6HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception);

            Action act = () => mockLogger.VerifyFatal(@"/(?i)hello,\s+world[.!?]/", exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyFatal method 6 throws MockException when criteria are not met - wrong Times")]
        public void VerifyFatalMethod6SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception);

            Action act = () => mockLogger.VerifyFatal("Hello, world!", exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 6 throws MockException when criteria are not met - wrong message")]
        public void VerifyFatalMethod6SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Good-bye, cruel world!", exception);

            Action act = () => mockLogger.VerifyFatal(@"/(?i)hello,\s+world[.!?]/", exception, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 6 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyFatalMethod6SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Fatal("Hello world!", exception);

            Action act = () => mockLogger.VerifyFatal("Hello, world!", wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 7 verifies successfully when criteria are met")]
        public void VerifyFatalMethod7HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyFatal(exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyFatal method 7 throws MockException when criteria are not met - wrong Times")]
        public void VerifyFatalMethod7SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyFatal(exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 7 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyFatalMethod7SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyFatal(exception, new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 7 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyFatalMethod7SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyFatal(exception, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 7 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyFatalMethod7SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyFatal(wrongException, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 8 verifies successfully when criteria are met")]
        public void VerifyFatalMethod8HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyFatal(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyFatal method 8 throws MockException when criteria are not met - wrong Times")]
        public void VerifyFatalMethod8SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyFatal("Hello, world!", exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 8 throws MockException when criteria are not met - wrong message")]
        public void VerifyFatalMethod8SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Good-bye, cruel world!", exception, new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyFatal(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 8 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyFatalMethod8SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyFatal("Hello, world!", exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 8 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyFatalMethod8SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyFatal(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 8 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyFatalMethod8SadPath5()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyFatal("Hello, world!", wrongException, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 9 verifies successfully when criteria are met")]
        public void VerifyFatalMethod9HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception);

            Action act = () => mockLogger.VerifyFatal(ex => ex == exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyFatal method 9 throws MockException when criteria are not met - wrong Times")]
        public void VerifyFatalMethod9SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception);

            Action act = () => mockLogger.VerifyFatal(ex => ex == exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 9 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyFatalMethod9SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception);

            Action act = () => mockLogger.VerifyFatal(ex => ex == wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 10 verifies successfully when criteria are met")]
        public void VerifyFatalMethod10HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception);

            Action act = () => mockLogger.VerifyFatal(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyFatal method 10 throws MockException when criteria are not met - wrong Times")]
        public void VerifyFatalMethod10SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception);

            Action act = () => mockLogger.VerifyFatal("Hello, world!", ex => ex == exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 10 throws MockException when criteria are not met - wrong message")]
        public void VerifyFatalMethod10SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Good-bye, cruel world!", exception);

            Action act = () => mockLogger.VerifyFatal(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 10 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyFatalMethod10SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Fatal("Hello world!", exception);

            Action act = () => mockLogger.VerifyFatal("Hello, world!", ex => ex == wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 11 verifies successfully when criteria are met")]
        public void VerifyFatalMethod11HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyFatal(ex => ex == exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyFatal method 11 throws MockException when criteria are not met - wrong Times")]
        public void VerifyFatalMethod11SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyFatal(ex => ex == exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 1 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyFatalMethod11SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyFatal(ex => ex == exception, new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 11 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyFatalMethod11SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyFatal(ex => ex == exception, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 11 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyFatalMethod11SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyFatal(ex => ex == wrongException, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 12 verifies successfully when criteria are met")]
        public void VerifyFatalMethod12HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyFatal(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyFatal method 12 throws MockException when criteria are not met - wrong Times")]
        public void VerifyFatalMethod12SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyFatal("Hello, world!", ex => ex == exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 12 throws MockException when criteria are not met - wrong message")]
        public void VerifyFatalMethod12SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Good-bye, cruel world!", exception, new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyFatal(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 12 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyFatalMethod12SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyFatal("Hello, world!", ex => ex == exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 12 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyFatalMethod12SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyFatal(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 12 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyFatalMethod12SadPath5()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Fatal("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyFatal("Hello, world!", ex => ex == wrongException, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 1 verifies successfully when criteria are met")]
        public void VerifyAuditMethod1HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!");

            Action act = () => mockLogger.VerifyAudit(Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyAudit method 1 throws MockException when criteria are not met - wrong Times")]
        public void VerifyAuditMethod1SadPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!");

            Action act = () => mockLogger.VerifyAudit(Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 2 verifies successfully when criteria are met")]
        public void VerifyAuditMethod2HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!");

            Action act = () => mockLogger.VerifyAudit(@"/(?i)hello,\s+world[.!?]/", Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyAudit method 2 throws MockException when criteria are not met - wrong Times")]
        public void VerifyAuditMethod2SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!");

            Action act = () => mockLogger.VerifyAudit("Hello, world!", Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 2 throws MockException when criteria are not met - wrong message")]
        public void VerifyAuditMethod2SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Good-bye, cruel world!");

            Action act = () => mockLogger.VerifyAudit(@"/(?i)hello,\s+world[.!?]/", Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 3 verifies successfully when criteria are met")]
        public void VerifyAuditMethod3HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyAudit(new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyAudit method 3 throws MockException when criteria are not met - wrong Times")]
        public void VerifyAuditMethod3SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyAudit(new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 3 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyAuditMethod3SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyAudit(new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 3 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyAuditMethod3SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!", new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyAudit(new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 4 verifies successfully when criteria are met")]
        public void VerifyAuditMethod4HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyAudit("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyAudit method 4 throws MockException when criteria are not met - wrong Times")]
        public void VerifyAuditMethod4SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyAudit(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 4 throws MockException when criteria are not met - wrong message")]
        public void VerifyAuditMethod4SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Good-bye, cruel world!", new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyAudit("Hello, world!", new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 4 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyAuditMethod4SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!", new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyAudit(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 4 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyAuditMethod4SadPath4()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!", new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyAudit("Hello, world!", new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 5 verifies successfully when criteria are met")]
        public void VerifyAuditMethod5HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception);

            Action act = () => mockLogger.VerifyAudit(exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyAudit method 5 throws MockException when criteria are not met - wrong Times")]
        public void VerifyAuditMethod5SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception);

            Action act = () => mockLogger.VerifyAudit(exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 5 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyAuditMethod5SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception);

            Action act = () => mockLogger.VerifyAudit(wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 6 verifies successfully when criteria are met")]
        public void VerifyAuditMethod6HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception);

            Action act = () => mockLogger.VerifyAudit(@"/(?i)hello,\s+world[.!?]/", exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyAudit method 6 throws MockException when criteria are not met - wrong Times")]
        public void VerifyAuditMethod6SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception);

            Action act = () => mockLogger.VerifyAudit("Hello, world!", exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 6 throws MockException when criteria are not met - wrong message")]
        public void VerifyAuditMethod6SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Good-bye, cruel world!", exception);

            Action act = () => mockLogger.VerifyAudit(@"/(?i)hello,\s+world[.!?]/", exception, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 6 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyAuditMethod6SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Audit("Hello world!", exception);

            Action act = () => mockLogger.VerifyAudit("Hello, world!", wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 7 verifies successfully when criteria are met")]
        public void VerifyAuditMethod7HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyAudit(exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyAudit method 7 throws MockException when criteria are not met - wrong Times")]
        public void VerifyAuditMethod7SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyAudit(exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 7 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyAuditMethod7SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyAudit(exception, new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 7 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyAuditMethod7SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyAudit(exception, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 7 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyAuditMethod7SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyAudit(wrongException, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 8 verifies successfully when criteria are met")]
        public void VerifyAuditMethod8HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyAudit(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyAudit method 8 throws MockException when criteria are not met - wrong Times")]
        public void VerifyAuditMethod8SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyAudit("Hello, world!", exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 8 throws MockException when criteria are not met - wrong message")]
        public void VerifyAuditMethod8SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Good-bye, cruel world!", exception, new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyAudit(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 8 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyAuditMethod8SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyAudit("Hello, world!", exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 8 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyAuditMethod8SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyAudit(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 8 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyAuditMethod8SadPath5()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyAudit("Hello, world!", wrongException, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 9 verifies successfully when criteria are met")]
        public void VerifyAuditMethod9HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception);

            Action act = () => mockLogger.VerifyAudit(ex => ex == exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyAudit method 9 throws MockException when criteria are not met - wrong Times")]
        public void VerifyAuditMethod9SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception);

            Action act = () => mockLogger.VerifyAudit(ex => ex == exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 9 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyAuditMethod9SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception);

            Action act = () => mockLogger.VerifyAudit(ex => ex == wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 10 verifies successfully when criteria are met")]
        public void VerifyAuditMethod10HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception);

            Action act = () => mockLogger.VerifyAudit(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyAudit method 10 throws MockException when criteria are not met - wrong Times")]
        public void VerifyAuditMethod10SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception);

            Action act = () => mockLogger.VerifyAudit("Hello, world!", ex => ex == exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 10 throws MockException when criteria are not met - wrong message")]
        public void VerifyAuditMethod10SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Good-bye, cruel world!", exception);

            Action act = () => mockLogger.VerifyAudit(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 10 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyAuditMethod10SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Audit("Hello world!", exception);

            Action act = () => mockLogger.VerifyAudit("Hello, world!", ex => ex == wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 11 verifies successfully when criteria are met")]
        public void VerifyAuditMethod11HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyAudit(ex => ex == exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyAudit method 11 throws MockException when criteria are not met - wrong Times")]
        public void VerifyAuditMethod11SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyAudit(ex => ex == exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 1 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyAuditMethod11SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyAudit(ex => ex == exception, new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 11 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyAuditMethod11SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyAudit(ex => ex == exception, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 11 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyAuditMethod11SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyAudit(ex => ex == wrongException, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 12 verifies successfully when criteria are met")]
        public void VerifyAuditMethod12HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyAudit(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyAudit method 12 throws MockException when criteria are not met - wrong Times")]
        public void VerifyAuditMethod12SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyAudit("Hello, world!", ex => ex == exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 12 throws MockException when criteria are not met - wrong message")]
        public void VerifyAuditMethod12SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Good-bye, cruel world!", exception, new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyAudit(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 12 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyAuditMethod12SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyAudit("Hello, world!", ex => ex == exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 12 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyAuditMethod12SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyAudit(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 12 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyAuditMethod12SadPath5()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Audit("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyAudit("Hello, world!", ex => ex == wrongException, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 1 verifies successfully when criteria are met")]
        public void VerifyLogMethod1HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!");

            Action act = () => mockLogger.VerifyLog(Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyLog method 1 throws MockException when criteria are not met - wrong Times")]
        public void VerifyLogMethod1SadPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!");

            Action act = () => mockLogger.VerifyLog(Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyLog method 2 verifies successfully when criteria are met")]
        public void VerifyLogMethod2HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!");

            Action act = () => mockLogger.VerifyLog(@"/(?i)hello,\s+world[.!?]/", Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyLog method 2 throws MockException when criteria are not met - wrong Times")]
        public void VerifyLogMethod2SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!");

            Action act = () => mockLogger.VerifyLog("Hello, world!", Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyLog method 2 throws MockException when criteria are not met - wrong message")]
        public void VerifyLogMethod2SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Good-bye, cruel world!");

            Action act = () => mockLogger.VerifyLog(@"/(?i)hello,\s+world[.!?]/", Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 3 verifies successfully when criteria are met")]
        public void VerifyLogMethod3HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog(new { Foo = 123 }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyLog method 3 throws MockException when criteria are not met - wrong Times")]
        public void VerifyLogMethod3SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog(new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyLog method 3 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyLogMethod3SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog(new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 3 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyLogMethod3SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyLog(new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 4 verifies successfully when criteria are met")]
        public void VerifyLogMethod4HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog("Hello, world!", new { Foo = 123 }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyLog method 4 throws MockException when criteria are not met - wrong Times")]
        public void VerifyLogMethod4SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyLog method 4 throws MockException when criteria are not met - wrong message")]
        public void VerifyLogMethod4SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Good-bye, cruel world!", new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyLog("Hello, world!", new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 4 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyLogMethod4SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyLog(@"/(?i)hello,\s+world[.!?]/", new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 4 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyLogMethod4SadPath4()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyLog("Hello, world!", new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 5 verifies successfully when criteria are met")]
        public void VerifyLogMethod5HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyLog(exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyLog method 5 throws MockException when criteria are not met - wrong Times")]
        public void VerifyLogMethod5SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyLog(exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyLog method 5 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyLogMethod5SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyLog(wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 6 verifies successfully when criteria are met")]
        public void VerifyLogMethod6HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyLog(@"/(?i)hello,\s+world[.!?]/", exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyLog method 6 throws MockException when criteria are not met - wrong Times")]
        public void VerifyLogMethod6SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyLog("Hello, world!", exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyLog method 6 throws MockException when criteria are not met - wrong message")]
        public void VerifyLogMethod6SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Good-bye, cruel world!", exception);

            Action act = () => mockLogger.VerifyLog(@"/(?i)hello,\s+world[.!?]/", exception, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 6 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyLogMethod6SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyLog("Hello, world!", wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 7 verifies successfully when criteria are met")]
        public void VerifyLogMethod7HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog(exception, new { Foo = 123 }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyLog method 7 throws MockException when criteria are not met - wrong Times")]
        public void VerifyLogMethod7SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog(exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyLog method 7 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyLogMethod7SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog(exception, new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 7 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyLogMethod7SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyLog(exception, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 8 verifies successfully when criteria are met")]
        public void VerifyLogMethod8HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123 }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyLog method 8 throws MockException when criteria are not met - wrong Times")]
        public void VerifyLogMethod8SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog("Hello, world!", exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyLog method 8 throws MockException when criteria are not met - wrong message")]
        public void VerifyLogMethod8SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Good-bye, cruel world!", exception, new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyLog(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 8 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyLogMethod8SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyLog("Hello, world!", exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 8 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyLogMethod8SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyLog(@"/(?i)hello,\s+world[.!?]/", exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }





        [Fact(DisplayName = "VerifyLog method 9 verifies successfully when criteria are met")]
        public void VerifyLogMethod9HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyLog(ex => ex == exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyLog method 9 throws MockException when criteria are not met - wrong Times")]
        public void VerifyLogMethod9SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyLog(ex => ex == exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyLog method 9 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyLogMethod9SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyLog(ex => ex == wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 10 verifies successfully when criteria are met")]
        public void VerifyLogMethod10HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyLog("Hello, world!", ex => ex == exception, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyLog method 10 throws MockException when criteria are not met - wrong Times")]
        public void VerifyLogMethod10SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyLog(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyLog method 10 throws MockException when criteria are not met - wrong message")]
        public void VerifyLogMethod10SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Good-bye, cruel world!", exception);

            Action act = () => mockLogger.VerifyLog("Hello, world!", ex => ex == exception, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 10 throws MockException when criteria are not met - wrong Exception")]
        public void VerifyLogMethod10SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();
            var wrongException = new Exception();

            mockLogger.Object.Info("Hello, world!", exception);

            Action act = () => mockLogger.VerifyLog(@"/(?i)hello,\s+world[.!?]/", ex => ex == wrongException, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 11 verifies successfully when criteria are met")]
        public void VerifyLogMethod11HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog(ex => ex == exception, new { Foo = 123 }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyLog method 1 throws MockException when criteria are not met - wrong Times")]
        public void VerifyLogMethod11SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog(ex => ex == exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyLog method 11 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyLogMethod11SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog(ex => ex == exception, new { Foo = 456 }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 11 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyLogMethod11SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = "foobar" });

            Action act = () => mockLogger.VerifyLog(ex => ex == exception, new { Foo = "/^foo$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 12 verifies successfully when criteria are met")]
        public void VerifyLogMethod12HappyPath()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog("Hello, world!", ex => ex == exception, new { Foo = 123 }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyLog method 12 throws MockException when criteria are not met - wrong Times")]
        public void VerifyLogMethod12SadPath1()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyLog method 12 throws MockException when criteria are not met - wrong message")]
        public void VerifyLogMethod12SadPath2()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Good-bye, cruel world!", exception, new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyLog("Hello, world!", ex => ex == exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 12 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyLogMethod12SadPath3()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyLog(@"/(?i)hello,\s+world[.!?]/", ex => ex == exception, new { Foo = 123, Bar = "/^abc$/" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 12 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyLogMethod12SadPath4()
        {
            var mockLogger = new MockLogger();
            var exception = new Exception();

            mockLogger.Object.Info("Hello, world!", exception, new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyLog("Hello, world!", ex => ex == exception, new { Foo = 123, Bar = "abc" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }
    }
}
