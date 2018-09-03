using System;
using System.Net;
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
            
            using (var wc = new WebClient())
            {
                wc.DownloadString(new Uri($"{_webServer.BaseUrl}/database/restorefortests"));
            }
        }

        [AfterTestRun]
        public static void CloseIISExpress()
        {
            using (var wc = new WebClient())
            {
                wc.DownloadString(new Uri($"{_webServer.BaseUrl}/database/restorefordevelopment"));
            }

            _webServer.Stop();
        }
    }
}
