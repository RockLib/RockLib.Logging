# How to instantiate and configure a logger

The `Logger` class can be directly instantiated and has two public constructors. The only difference between the constructors is how its log processor is initialized (whether by enum or directly). It also has one read/write property.

#### Constructor 1:

Name | Type | Description | Required | Default Value
---- | ---- | ----------- | -------- | -------------
name | `string` | The name of the logger. | No | `"default"`
level | `enum LogLevel`: `NotSet`, `Debug`, `Info`, `Warn`, `Error`, `Fatal`, `Audit` | The logging level of the logger. Logs with a level lower than this are not processed. | No | `NotSet`
logProviders | `IReadOnlyCollection<ILogProvider>` | A collection of `ILogProvider` objects used by this logger. | No | Empty list
isDisabled | `bool` | Whether the logger should be disabled. | No | `false`
processingMode | `enum ProcessingMode`: `Background`, `Synchronous`, `FireAndForget` | A value that indicates how the logger will process logs. | No | `Background`
contextProviders | `IReadOnlyCollection<IContextProvider>` | A collection of `IContextProvider` objects that customize outgoing log entries. | No | Empty list

#### Constructor 2:

Name | Type | Description | Required | Default Value
---- | ---- | ----------- | -------- | -------------
logProcessor | `ILogProcessor` | The object responsible for processing logs. | Yes | N/A
name | `string` | The name of the logger. | No | `"default"`
level | `enum LogLevel`: `NotSet`, `Debug`, `Info`, `Warn`, `Error`, `Fatal`, `Audit` | The logging level of the logger. Logs with a level lower than this are not processed. | No | `NotSet`
logProviders | `IReadOnlyCollection<ILogProvider>` | A collection of `ILogProvider` objects used by this logger. | No | Empty list
isDisabled | `bool` | Whether the logger should be disabled. | No | `false`
contextProviders | `IReadOnlyCollection<IContextProvider>` | A collection of `IContextProvider` objects that customize outgoing log entries. | No | Empty list

#### Read / write properties

Name | Type | Description | Default Value
---- | ---- | ----------- | -------------
ErrorHandler | `IErrorHandler` | An object that handles errors that occur during log processing. | `null`
