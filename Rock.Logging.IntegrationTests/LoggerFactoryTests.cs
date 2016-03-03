using NUnit.Framework;
using Rock.Conversion;

namespace Rock.Logging.IntegrationTests
{
    [TestFixture]
    public class LoggerFactoryTests
    {
        [Test]
        public void CanSetAppId()
        {
            var appId = "foo";
            Assume.That(ApplicationId.Current != appId, "You've named an application \'foo\', apparently.");
            var log = LoggerFactory.GetInstance(applicationId: appId);
            var implCast = (Logger) log;
            Assert.That(string.Equals(implCast.ApplicationId, appId));
        }
    }
}