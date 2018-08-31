using TechTalk.SpecFlow;
using TestDrive.SmokeTests.IISExpress;

namespace TestDrive.SmokeTests.Steps
{
    [Binding]
    public class InitAndTeardownSteps
    {
        private static IISExpressHost _webServer = IISExpressHostFactory.CreateDefaultInstance();

        [BeforeTestRun]
        public static void LaunchIISExpress()
        {
            _webServer.Start();
        }

        [AfterTestRun]
        public static void CloseIISExpress()
        {
            _webServer.Stop();
        }
    }
}
