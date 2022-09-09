using FluentAssertions;
using RockLib.Logging.SafeLogging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace RockLib.Logging.Tests.SafeLogging;

public class SanitizeEngineTests
{
    [Fact(DisplayName = "Sanitize method transforms safe-to-log type (defined at compile-time) into string dictionary")]
    public void SanitizeMethodTest1()
    {
        var value = new ExampleSafeToLogType { Foo = "abc", Bar = "xyz" };

        var sanitizedValue = SanitizeEngine.Sanitize(value);

        sanitizedValue.Should().BeOfType<Dictionary<string, object>>()
            .Which.Should().BeEquivalentTo(new Dictionary<string, object> { { "Foo", "abc" } });
    }

    [Fact(DisplayName = "Sanitize method transforms type with safe-to-log properties (defined at compile-time) into string dictionary")]
    public void SanitizeMethodTest2()
    {
        var value = new ExampleSafeToLogProperty { Foo = "abc", Bar = "xyz" };

        var sanitizedValue = SanitizeEngine.Sanitize(value);

        sanitizedValue.Should().BeOfType<Dictionary<string, object>>()
            .Which.Should().BeEquivalentTo(new Dictionary<string, object> { { "Bar", "xyz" } });
    }

    [Fact(DisplayName = "Sanitize method transforms not-safe-to-log object into \"everything sanitized\" message")]
    public void SanitizeMethodTest3()
    {
        var value = new ExampleNotSafeToLog1 { Foo = "abc", Bar = "xyz" };

        var sanitizedValue = SanitizeEngine.Sanitize(value);

        sanitizedValue.Should().BeOfType<string>()
            .Which.Should().Be($"All properties from the {typeof(ExampleNotSafeToLog1).FullName} type have been excluded from the log entry extended properties because none were decorated with the [SafeToLog] attribute.");
    }

    [Fact(DisplayName = "Sanitize method transforms safe-to-log type (defined at run-time) into string dictionary")]
    public void SanitizeMethodTest4()
    {
        SafeToLogAttribute.Decorate<ExampleNotSafeToLog2>();
        NotSafeToLogAttribute.Decorate<ExampleNotSafeToLog2>(x => x.Bar);

        var value = new ExampleNotSafeToLog2 { Foo = "abc", Bar = "xyz" };

        var sanitizedValue = SanitizeEngine.Sanitize(value);

        sanitizedValue.Should().BeOfType<Dictionary<string, object>>()
            .Which.Should().BeEquivalentTo(new Dictionary<string, object> { { "Foo", "abc" } });
    }

    [Fact(DisplayName = "Sanitize method transforms type with safe-to-log properties (defined at run-time) into string dictionary")]
    public void SanitizeMethodTest5()
    {
        SafeToLogAttribute.Decorate<ExampleNotSafeToLog3>(x => x.Foo);

        var value = new ExampleNotSafeToLog3 { Foo = "abc", Bar = "xyz" };

        var sanitizedValue = SanitizeEngine.Sanitize(value);

        sanitizedValue.Should().BeOfType<Dictionary<string, object>>()
            .Which.Should().BeEquivalentTo(new Dictionary<string, object> { { "Foo", "abc" } });
    }

    [Fact(DisplayName = "Sanitize method sanitizes the values of a string dictionary")]
    public void SanitizeMethodTest6()
    {
        var value = new Dictionary<string, object>
        {
            {"Garply", new ExampleSafeToLogType { Foo = "abc", Bar = "xyz" } },
            {"Grault", new ExampleSafeToLogProperty { Foo = "abc", Bar = "xyz" } }
        };

        var sanitizedValue = SanitizeEngine.Sanitize(value);

        var sanitizedDictionary =
            sanitizedValue.Should().BeOfType<Dictionary<string, object>>()
                .Subject;

        sanitizedDictionary.Should().ContainKey("Garply")
            .WhichValue.Should().BeOfType<Dictionary<string, object>>()
            .Which.Should().BeEquivalentTo(new Dictionary<string, object> { { "Foo", "abc" } });

        sanitizedDictionary.Should().ContainKey("Grault")
            .WhichValue.Should().BeOfType<Dictionary<string, object>>()
            .Which.Should().BeEquivalentTo(new Dictionary<string, object> { { "Bar", "xyz" } });
    }

    [Fact(DisplayName = "Sanitize method sanitizes the values of a non-string generic dictionary")]
    public void SanitizeMethodTest7()
    {
        var value = new Dictionary<string, object>
        {
            {"Garply", new ExampleSafeToLogType { Foo = "abc", Bar = "xyz" } },
            {"Grault", new ExampleSafeToLogProperty { Foo = "abc", Bar = "xyz" } }
        };

        var sanitizedValue = SanitizeEngine.Sanitize(value);

        var sanitizedDictionary =
            sanitizedValue.Should().BeOfType<Dictionary<string, object>>()
                .Subject;

        sanitizedDictionary.Should().ContainKey("Garply")
            .WhichValue.Should().BeOfType<Dictionary<string, object>>()
            .Which.Should().BeEquivalentTo(new Dictionary<string, object> { { "Foo", "abc" } });

        sanitizedDictionary.Should().ContainKey("Grault")
            .WhichValue.Should().BeOfType<Dictionary<string, object>>()
            .Which.Should().BeEquivalentTo(new Dictionary<string, object> { { "Bar", "xyz" } });
    }

    [Fact(DisplayName = "Sanitize method sanitizes the values of a non-generic dictionary")]
    public void SanitizeMethodTest8()
    {
        var value = new Hashtable
        {
            {"Garply", new ExampleSafeToLogType { Foo = "abc", Bar = "xyz" } },
            {"Grault", new ExampleSafeToLogProperty { Foo = "abc", Bar = "xyz" } }
        };

        var sanitizedValue = SanitizeEngine.Sanitize(value);

        var sanitizedCollection =
            sanitizedValue.Should().BeOfType<ArrayList>()
                .Subject;

        sanitizedCollection.Should().HaveCount(2);
        var garply = sanitizedCollection.Cast<DictionaryEntry>().First(x => x.Key is string key && key == "Garply");
        var grault = sanitizedCollection.Cast<DictionaryEntry>().First(x => x.Key is string key && key == "Grault");

        garply.Value.Should().BeEquivalentTo(new Dictionary<string, object> { { "Foo", "abc" } });
        grault.Value.Should().BeEquivalentTo(new Dictionary<string, object> { { "Bar", "xyz" } });
    }

    [Fact(DisplayName = "Sanitize method sanitizes the values of a collection")]
    public void SanitizeMethodTest9()
    {
        var value = new List<object>
        {
            new ExampleSafeToLogType { Foo = "abc", Bar = "xyz" },
            new ExampleSafeToLogProperty { Foo = "abc", Bar = "xyz" }
        };

        var sanitizedValue = SanitizeEngine.Sanitize(value);

        var sanitizedCollection =
            sanitizedValue.Should().BeOfType<ArrayList>()
                .Subject;

        sanitizedCollection.Should().HaveCount(2);
        
        sanitizedCollection[0].Should().BeOfType<Dictionary<string, object>>()
            .Which.Should().BeEquivalentTo(new Dictionary<string, object> { { "Foo", "abc" } });

        sanitizedCollection[1].Should().BeOfType<Dictionary<string, object>>()
            .Which.Should().BeEquivalentTo(new Dictionary<string, object> { { "Bar", "xyz" } });
    }

    [Fact(DisplayName = "Sanitize method does not change types whitelisted by IsTypeSafeToLog")]
    public void SanitizeMethodTest10()
    {
        SanitizeEngine.IsTypeSafeToLog = type => type == typeof(ExampleNotSafeToLog4);

        try
        {
            var value = new ExampleNotSafeToLog4 { Foo = "abc", Bar = "xyz" };

            var sanitizedValue = SanitizeEngine.Sanitize(value);

            sanitizedValue.Should().BeSameAs(value);
        }
        finally
        {
            SanitizeEngine.IsTypeSafeToLog = null;
        }
    }

    [Theory(DisplayName = "Sanitize method does not change \"value\" types")]
    [MemberData(nameof(SanitizeMethodTest1TestData))]
    public void SanitizeMethodTest11(object value)
    {
        var sanitizedValue = SanitizeEngine.Sanitize(value);

        sanitizedValue.Should().Be(value);
    }

    public static IEnumerable<object[]> SanitizeMethodTest1TestData
    {
        get
        {
            yield return new object[] { 123 };
            yield return new object[] { Base64FormattingOptions.InsertLineBreaks };
            yield return new object[] { "Hello, world!" };
            yield return new object[] { 123.45M };
            yield return new object[] { DateTime.Now };
            yield return new object[] { DateTime.Now.TimeOfDay };
            yield return new object[] { DateTimeOffset.Now };
            yield return new object[] { Guid.NewGuid() };
            yield return new object[] { new Uri("https://google.com") };
            yield return new object[] { Encoding.UTF8 };
            yield return new object[] { typeof(SanitizeEngineTests) };
        }
    }

    public class ExampleNotSafeToLog1
    {
        public string Foo { get; set; }

        public string Bar { get; set; }
    }

    public class ExampleNotSafeToLog2
    {
        public string Foo { get; set; }

        public string Bar { get; set; }
    }

    public class ExampleNotSafeToLog3
    {
        public string Foo { get; set; }

        public string Bar { get; set; }
    }

    public class ExampleNotSafeToLog4
    {
        public string Foo { get; set; }

        public string Bar { get; set; }
    }

    [SafeToLog]
    public class ExampleSafeToLogType
    {
        public string Foo { get; set; }

        [NotSafeToLog]
        public string Bar { get; set; }
    }

    public class ExampleSafeToLogProperty
    {
        public string Foo { get; set; }

        [SafeToLog]
        public string Bar { get; set; }
    }
}
