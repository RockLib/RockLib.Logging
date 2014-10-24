using System;
using NUnit.Framework;
using System.Configuration;

namespace Rock.Logging.IntegrationTests
{
    public class FileConfigurationTests
    {
        [Test]
        public void Foo()
        {
            var factory = (ILoggerFactory)ConfigurationManager.GetSection("rock.logging");
            var logger = factory.Get<Logger>();
            logger.Warn(GetLogEntry());
        }

        private static LogEntry GetLogEntry()
        {
            Exception exception;

            try
            {
                try
                {
                    throw new Exception();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("o noes!", ex);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return new LogEntry("Hello, world!", new { foo = "you <> me", baz = "qux" }, exception, "testing...");
        }
    }
}