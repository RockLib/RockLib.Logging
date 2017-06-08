# Advanced Configuration

## Table of Contents
* [Main Overview](../readme.md)
* [Custom Formatter Configuration](#custom-formatter-configuration)
* Logger Configuration
  * [Console Logger Configuration](#console-logger-configuration)
  * [Email Logger Configuration](#email-logger-configuration)
  * [Rolling File Logger Configuration](#rolling-file-logger-configuration)
* [IoC Configuration](#ioc-configuration)
  
## Custom Formatter Configuration
When configuring Rock.Logging you have the ability to specify the format that your logs are outputted on a per category/appender basis.  You can even provide multiple formatters, each can assigned to a different category

The key thing to keep in mind when creating the formmatter which is intended to be read by a person  you want the log to contain all the information needed to help determine the cause of the log.  This includes {createTime}, {level} and {message}

### Valid Tokens available for Formatting
* {message}
* {applicationId}
* {applicationUserId}
* {environment}
* {machineName}
* {level}
* {exception}
* {exceptionContext}
* {uniqueId}
* {callerInfo}
* {stepReport}
* {createTime}
* {newline}
* {tab}

Below is a very basic formatter, this will format the message and exception on the same line.
```
<formatter name="default" template="[{createTime}] - {level} - {message}" />
``` 

If your configuration has more than 1 formatter it is required that each one has a unique name.  If a name is duplicated than you will get a runtime error.

## Specifying Logging Categories/Appenders
If you do not provide a category the console logger will be used by default.

### Console Logger Configuration
Writes logging events to the application's Console. The events may go to either the standard our stream or the standard error stream.

```
      <category name="Default">
        <providers>
          <provider type="Rock.Logging.ConsoleLogProvider, Rock.Logging"
                    formatter="default"
                    loggingLevel="Debug"/>
        </providers>
      </category>   
```

Formatter: The formatter configuration to use

LogginLevel: specifies the minimal category of logging which will be recorded. If Debug is provided ALL logging categories will be written. If Error is provided, only Error, Fatal and Audit will be written.

### Email Logger Configuration
```
      <category name="Default">
        <providers>
          <provider type="Rock.Logging.EmailLogProvider, Rock.Logging"
                    toEmail="email@domain.com"
                    fromEmail="email@domain.com"
                    subject="{environment} {level} message on {machineName}"
                    formatter="email">
        </providers>
      </category>  
```
Formatter: The formatter configuration to use

LogginLevel: specifies the minimal category of logging which will be recorded. If Debug is provided ALL logging categories will be written. If Error is provided, only Error, Fatal and Audit will be written.

### File Logger Configuration
Writes logging events to a file in the file system.

```
          <provider type="Rock.Logging.FileLogProvider, Rock.Logging"
                    formatter="fileFormatter"
                    loggingLevel="Debug"
                    file="FULL_PATH_TO_FILE" />
```

File: Full path and file name of the log file to be created

Formatter: The formatter configuration to use

LogginLevel: specifies the minimal category of logging which will be recorded. If Debug is provided ALL logging categories will be written. If Error is provided, only Error, Fatal and Audit will be written.

### Rolling File Logger Configuration
Writes logging events to a file in the file system. The RollingFileAppender can be configured to log to multiple files based upon date or file size constraints.

```
          <provider type="Rock.Logging.RollingFileLogProvider, Rock.Logging"
                    maxFileSizeKilobytes="1024"
                    maxArchiveCount="5"
                    formatter="fileFormatter"
                    loggingLevel="Debug"
                    file="C:\Dev\Logs\Rock.Onboard\rock.onboard.rolling.log" />
```
File: Full path and file name of the log file to be created

MaxFileSizeKilobytes: Tells the logging system the max size each log file should be.  Once this size is hit a new file will be created.  

MaxArchiveCount: The number of files to keep as a backup from the roll overs

Formatter: The formatter configuration to use

LogginLevel: specifies the minimal category of logging which will be recorded. If Debug is provided ALL logging categories will be written. If Error is provided, only Error, Fatal and Audit will be written.

### Http Endpoint Log Configuration

## IoC Configuration
Registering the ILogger interface will be different for every IoC container, but the concepts are pretty much the same.  You will want to setup the container to register an instance of logger via the LoggerFactory

### Autofac Configuration
Inside your Autofac IoC setup provide the single line below

```
builder.Register(c => LoggerFactory.GetInstance()).As<ILogger>();
```

The code above will instruct Autofac to pull the current instance out of the LoggerFactor for each request.



