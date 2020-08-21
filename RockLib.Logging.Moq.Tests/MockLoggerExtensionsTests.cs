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

            Action act = () => mockLogger.VerifyDebug(@"(?i)hello,\s+world[.!?]", Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyDebug method 2 throws MockException when criteria are not met - wrong Times")]
        public void VerifyDebugMethod2SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!");

            Action act = () => mockLogger.VerifyDebug(@"(?i)hello,\s+world[.!?]", Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 2 throws MockException when criteria are not met - wrong message")]
        public void VerifyDebugMethod2SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Good-bye, cruel world!");

            Action act = () => mockLogger.VerifyDebug(@"(?i)hello,\s+world[.!?]", Times.Once(), "My fail message");

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

            Action act = () => mockLogger.VerifyDebug(new { Foo = "^foo$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 4 verifies successfully when criteria are met")]
        public void VerifyDebugMethod4HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyDebug(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyDebug method 4 throws MockException when criteria are not met - wrong Times")]
        public void VerifyDebugMethod4SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyDebug(@"(?i)hello,\s+world[.!?]", new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 4 throws MockException when criteria are not met - wrong message")]
        public void VerifyDebugMethod4SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Good-bye, cruel world!", new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyDebug(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 4 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyDebugMethod4SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!", new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyDebug(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar ="^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyDebug method 4 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyDebugMethod4SadPath4()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Debug("Hello, world!", new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyDebug(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

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

            Action act = () => mockLogger.VerifyInfo(@"(?i)hello,\s+world[.!?]", Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyInfo method 2 throws MockException when criteria are not met - wrong Times")]
        public void VerifyInfoMethod2SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!");

            Action act = () => mockLogger.VerifyInfo(@"(?i)hello,\s+world[.!?]", Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 2 throws MockException when criteria are not met - wrong message")]
        public void VerifyInfoMethod2SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Good-bye, cruel world!");

            Action act = () => mockLogger.VerifyInfo(@"(?i)hello,\s+world[.!?]", Times.Once(), "My fail message");

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

            Action act = () => mockLogger.VerifyInfo(new { Foo = "^foo$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 4 verifies successfully when criteria are met")]
        public void VerifyInfoMethod4HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyInfo(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyInfo method 4 throws MockException when criteria are not met - wrong Times")]
        public void VerifyInfoMethod4SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyInfo(@"(?i)hello,\s+world[.!?]", new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 4 throws MockException when criteria are not met - wrong message")]
        public void VerifyInfoMethod4SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Good-bye, cruel world!", new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyInfo(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 4 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyInfoMethod4SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyInfo(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyInfo method 4 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyInfoMethod4SadPath4()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyInfo(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

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

            Action act = () => mockLogger.VerifyWarn(@"(?i)hello,\s+world[.!?]", Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyWarn method 2 throws MockException when criteria are not met - wrong Times")]
        public void VerifyWarnMethod2SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!");

            Action act = () => mockLogger.VerifyWarn(@"(?i)hello,\s+world[.!?]", Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 2 throws MockException when criteria are not met - wrong message")]
        public void VerifyWarnMethod2SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Good-bye, cruel world!");

            Action act = () => mockLogger.VerifyWarn(@"(?i)hello,\s+world[.!?]", Times.Once(), "My fail message");

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

            Action act = () => mockLogger.VerifyWarn(new { Foo = "^foo$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 4 verifies successfully when criteria are met")]
        public void VerifyWarnMethod4HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyWarn(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyWarn method 4 throws MockException when criteria are not met - wrong Times")]
        public void VerifyWarnMethod4SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyWarn(@"(?i)hello,\s+world[.!?]", new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 4 throws MockException when criteria are not met - wrong message")]
        public void VerifyWarnMethod4SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Good-bye, cruel world!", new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyWarn(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 4 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyWarnMethod4SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!", new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyWarn(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyWarn method 4 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyWarnMethod4SadPath4()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Warn("Hello, world!", new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyWarn(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

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

            Action act = () => mockLogger.VerifyError(@"(?i)hello,\s+world[.!?]", Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyError method 2 throws MockException when criteria are not met - wrong Times")]
        public void VerifyErrorMethod2SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!");

            Action act = () => mockLogger.VerifyError(@"(?i)hello,\s+world[.!?]", Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyError method 2 throws MockException when criteria are not met - wrong message")]
        public void VerifyErrorMethod2SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Good-bye, cruel world!");

            Action act = () => mockLogger.VerifyError(@"(?i)hello,\s+world[.!?]", Times.Once(), "My fail message");

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

            Action act = () => mockLogger.VerifyError(new { Foo = "^foo$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 4 verifies successfully when criteria are met")]
        public void VerifyErrorMethod4HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyError(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyError method 4 throws MockException when criteria are not met - wrong Times")]
        public void VerifyErrorMethod4SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyError(@"(?i)hello,\s+world[.!?]", new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyError method 4 throws MockException when criteria are not met - wrong message")]
        public void VerifyErrorMethod4SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Good-bye, cruel world!", new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyError(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 4 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyErrorMethod4SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!", new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyError(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyError method 4 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyErrorMethod4SadPath4()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Error("Hello, world!", new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyError(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

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

            Action act = () => mockLogger.VerifyFatal(@"(?i)hello,\s+world[.!?]", Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyFatal method 2 throws MockException when criteria are not met - wrong Times")]
        public void VerifyFatalMethod2SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!");

            Action act = () => mockLogger.VerifyFatal(@"(?i)hello,\s+world[.!?]", Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 2 throws MockException when criteria are not met - wrong message")]
        public void VerifyFatalMethod2SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Good-bye, cruel world!");

            Action act = () => mockLogger.VerifyFatal(@"(?i)hello,\s+world[.!?]", Times.Once(), "My fail message");

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

            Action act = () => mockLogger.VerifyFatal(new { Foo = "^foo$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 4 verifies successfully when criteria are met")]
        public void VerifyFatalMethod4HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyFatal(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyFatal method 4 throws MockException when criteria are not met - wrong Times")]
        public void VerifyFatalMethod4SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyFatal(@"(?i)hello,\s+world[.!?]", new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 4 throws MockException when criteria are not met - wrong message")]
        public void VerifyFatalMethod4SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Good-bye, cruel world!", new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyFatal(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 4 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyFatalMethod4SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!", new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyFatal(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyFatal method 4 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyFatalMethod4SadPath4()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Fatal("Hello, world!", new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyFatal(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

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

            Action act = () => mockLogger.VerifyAudit(@"(?i)hello,\s+world[.!?]", Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyAudit method 2 throws MockException when criteria are not met - wrong Times")]
        public void VerifyAuditMethod2SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!");

            Action act = () => mockLogger.VerifyAudit(@"(?i)hello,\s+world[.!?]", Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 2 throws MockException when criteria are not met - wrong message")]
        public void VerifyAuditMethod2SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Good-bye, cruel world!");

            Action act = () => mockLogger.VerifyAudit(@"(?i)hello,\s+world[.!?]", Times.Once(), "My fail message");

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

            Action act = () => mockLogger.VerifyAudit(new { Foo = "^foo$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 4 verifies successfully when criteria are met")]
        public void VerifyAuditMethod4HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!", new { Foo = 123, Bar = new object[] { 456 } });

            Action act = () => mockLogger.VerifyAudit(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = new object[] { 456 } }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyAudit method 4 throws MockException when criteria are not met - wrong Times")]
        public void VerifyAuditMethod4SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyAudit(@"(?i)hello,\s+world[.!?]", new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 4 throws MockException when criteria are not met - wrong message")]
        public void VerifyAuditMethod4SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Good-bye, cruel world!", new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyAudit(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 4 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyAuditMethod4SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!", new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyAudit(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyAudit method 4 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyAuditMethod4SadPath4()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Audit("Hello, world!", new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyAudit(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

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

            Action act = () => mockLogger.VerifyLog(@"(?i)hello,\s+world[.!?]", Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyLog method 2 throws MockException when criteria are not met - wrong Times")]
        public void VerifyLogMethod2SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!");

            Action act = () => mockLogger.VerifyLog(@"(?i)hello,\s+world[.!?]", Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyLog method 2 throws MockException when criteria are not met - wrong message")]
        public void VerifyLogMethod2SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Good-bye, cruel world!");

            Action act = () => mockLogger.VerifyLog(@"(?i)hello,\s+world[.!?]", Times.Once(), "My fail message");

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

            Action act = () => mockLogger.VerifyLog(new { Foo = "^foo$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 4 verifies successfully when criteria are met")]
        public void VerifyLogMethod4HappyPath()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog(@"(?i)hello,\s+world[.!?]", new { Foo = 123 }, Times.Once(), "My fail message");

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "VerifyLog method 4 throws MockException when criteria are not met - wrong Times")]
        public void VerifyLogMethod4SadPath1()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123 });

            Action act = () => mockLogger.VerifyLog(@"(?i)hello,\s+world[.!?]", new { Foo = 123 }, Times.Exactly(2), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock exactly 2 times, but was 1 times*");
        }

        [Fact(DisplayName = "VerifyLog method 4 throws MockException when criteria are not met - wrong message")]
        public void VerifyLogMethod4SadPath2()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Good-bye, cruel world!", new { Foo = 123, Bar = "abc" });

            Action act = () => mockLogger.VerifyLog(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 4 throws MockException when criteria are not met - wrong extended property")]
        public void VerifyLogMethod4SadPath3()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 456, Bar = "abc" });

            Action act = () => mockLogger.VerifyLog(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }

        [Fact(DisplayName = "VerifyLog method 4 throws MockException when criteria are not met - wrong string extended property")]
        public void VerifyLogMethod4SadPath4()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.Info("Hello, world!", new { Foo = 123, Bar = "abcdefg" });

            Action act = () => mockLogger.VerifyLog(@"(?i)hello,\s+world[.!?]", new { Foo = 123, Bar = "^abc$" }, Times.Once(), "My fail message");

            act.Should().ThrowExactly<MockException>().WithMessage("*My fail message*Expected invocation on the mock once, but was 0 times*");
        }
    }
}
