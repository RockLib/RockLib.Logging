---
sidebar_position: 4
sidebar_label: 'Add log extended properties "safely"'
---

# How to add log extended properties "safely" (i.e. automatically remove PII)

In general, it is an extremely bad idea to log sensitive information. Unfortunately, it can be all too easy to inadvertently do this, such as by adding a request object containing PII as an extended property to a log. RockLib.Logging has several methods and extension methods to help protect against this. Specifically, we're protecting against properties of complex types that contain sensitive information. Applications are expected to know not to add a top-level extended property containing sensitive information.

## Marking properties as safe to log

To indicate that a property is safe to log, decorate it with the `[SafeToLog]` attribute:

```csharp
public class Client
{
    [SafeToLog]
    public string FirstName { get; set; }

    [SafeToLog]
    public string LastName { get; set; }

    public string SSN { get; set; }
}
```

Alternatively, decorate the class with `[SafeToLog]` and exclude the unsafe properties by decorating them with `[NotSafeToLog]`:

```csharp
[SafeToLog]
public class Client
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [NotSafeToLog]
    public string SSN { get; set; }
}
```

### Decorating at runtime

If you do not own the type that needs to be marked as safe to log, you can add `SafeToLogAttribute` and `NotSafeToLogAttribute` attributes at runtime.

```csharp
SafeToLogAttribute.Decorate<Client>(client => client.FirstName);
SafeToLogAttribute.Decorate<Client>(client => client.LastName);
```

Alternatively, decorate the class with `SafeToLogAttribute` and exclude the unsafe properties by decorating them with `NotSafeToLogAttribute`:

```csharp
SafeToLogAttribute.Decorate<Client>();
NotSafeToLogAttribute.Decorate<Client>(client => client.SSN);
```

## LogEntry methods

To add a sanitized extended property directly to a `LogEntry` object, use the `SetSanitizedExtendedProperty` method:

```csharp
var client = new Client { FirstName = "Joe", LastName = "Public", SSN = "123-45-6789" };

LogEntry logEntry = new LogEntry("Example log", LogLevel.Info);
logEntry.SetSanitizedExtendedProperty("Client", client);
```

To add multiple sanitized extended properties to a `LogEntry` object, use the `SetSanitizedExtendedProperties` method:

```csharp
var client1 = new Client { FirstName = "Joe", LastName = "Public", SSN = "123-45-6789" };
var client2 = new Client { FirstName = "Joan", LastName = "Public", SSN = "987-65-4321" };

LogEntry logEntry = new LogEntry("Example log", LogLevel.Info);
logEntry.SetSanitizedExtendedProperties("Client", new { Client = client1, Coclient = client2 });
```

## ILogger extension methods

To add sanitized extended properties to a log without directly creating a `LogEntry` object, call the `DebugSanitized`, `InfoSanitized`, etc. extension methods.

```csharp
var client1 = new Client { FirstName = "Joe", LastName = "Public", SSN = "123-45-6789" };
var client2 = new Client { FirstName = "Joan", LastName = "Public", SSN = "987-65-4321" };

var extendedProperties = new Dictionary<string, object>();
extendedProperties["Client"] = client1;
extendedProperties["Coclient"] = client2;

logger.InfoSanitized("ExampleMessage", extendedProperties);
```

## Implementation details

It is important to understand exactly what it means to sanitize an extended property:

- If the object is a "complex" type, then only properties decorated with the `[SafeToLog]` attribute are included.
  - The actual sanitization process involves creating a `Dictionary<string, object>` and adding an item for each of the safe-to-log properties. The key is the property name, and value is the sanitized property value.
- If the object is "value" type, such as a number, enum, or string, it is not modified.
  - We *could* attempt to sanitize a string value with regular expressions (looking for ssn or cc), but false positives are extremely likely.
- If the object is a collection, then we return a new collection, where each item is sanitized.
- If the object is a dictionary, then we return a new dictionary, where the value (but not the key) of each item is sanitized.
- If the object type is "clean" according to the `SanitizeEngine.IsCleanTypeFunction` property ([see below](#sanitizeengine-istypesafetolog)), then it is not modified.

Note that sanitation is a recursive process. Any circular references will result in a stack overflow exception.

## SanitizeEngine.IsTypeSafeToLog

This static property is meant to allow applications to mark a type as safe-to-log, when the application does not own the type in question. For example, an application could be using `System.Numerics.BigInteger` to store a very large value in an object:

```csharp
public class Apple
{
    [SafeToLog]
    public BigInteger NumberOfMolecules { get; set; }
}
```

Even though we're opting in to the `NumberOfMolecules` property, the `BigInteger` struct isn't known to the `SanitizeEngine`, so we would be left with an empty dictionary after sanitizing the number. This is one way to mark the `BigInteger` struct as safe-to-log:

```csharp
SanitizeEngine.IsTypeSafeToLog = runtimeType => runtimeType == typeof(BigInteger);
```
