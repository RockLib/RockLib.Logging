# Basic Configuration

## Steps Overview
1. Need to add the <configSection /> entry to your web.config/app.config
2. Need to add the <rock.logging /> section to your config file
3. Need to add the correct keys to the <appSettings /> section of the config file
4. Need to get an instance of the ILogger via the LoggerFactory

## ConfigSection Setup
### Add configSection to App.config/Web.config file.  It is advised that this is placed at the top of the configuration file.  If you already have a <configSections /> entry you will only need to add the <section /> element.

```
  <configSections>
    <section name="rock.logging" type="Rock.Logging.LoggerSectionHandler, Rock.Logging" />
  </configSections>
```

## Rock.Logging Setup
### Add the <rock.logging /> section to the configuration file.  This can be placed anywhere in the file but needs to be below the <configSections> attribute

```
<rock.logging loggingLevel="Debug" isLoggingEnabled="true">
  <formatters>
  </formatters>
  
  <categories>
  </categories>
</rock.logging>
```

Lets take a look at the above snippet.
 * loggingLevel - This specifies the minimal category of logging which will be recorded.  If Debug is provided ALL logging categories will be written.  If Error is provided, only Error, Fatal and Audit will be written
 * isLoggingEnabled - A bool flag to enable/disable the logging feature as a whole.  This will enable or disable all loggers

 If you fail to provide any formatters we will default to very verbose formatting.

 If you fail to provide a category/appender we will default to console logging.

## AppSettings Setup
 ### Add the following Entries to the <appSettings /> section of your config file.  If you do not provide these values you will get a runtime warning when accessing the LogFactory

 ```
 <add key="Rock.ApplicationId.Current" value="[EnterValueHere]"/>
 <add key="Rock.Environment.Current" value="[EnterValueHere]" />
 ```

 Lets take a look at the 2 config keys above in a bit more detail
 * Rock.ApplicationId.Current - This is used to set the current application ID for the backend logger, this should be unique for each app.  Use 200001 if you do not not have an Id
 * Rock.Environment.Current - This will default to Dev is nothing is provided.  Valid options are Dev, Test, Beta, Prod

## Getting Access to ILogger for Logging
 ### Logging Messages
 To get access to the configurred logger you only need to get an Instance via the LoggerFactory.  Once you have your instance you can call the correct log method and a log entry will be written

 ```
var logger = LoggerFactory.GetInstance();
logger.Debug("Some message goes here");
 ```
