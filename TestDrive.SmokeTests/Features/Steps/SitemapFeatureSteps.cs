using NUnit.Framework;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using TechTalk.SpecFlow;
using TestDrive.SmokeTests.Helpers;
using TestDrive.Web.Controllers;

namespace TestDrive.SmokeTests.Features.Steps
{
    [Binding]
    public class SitemapFeatureSteps
    {
        private WebClient wc = new WebClient
        {
            Encoding = System.Text.Encoding.UTF8
        };

        private string reply = null;
        private WebException webException;
        private HttpWebResponse webResponse = null;
        private static Process process;

        [Given(@"I visit /sitemap\.xml")]
        public void GivenIVisitSitemap_Xml()
        {
            var baseurl = "http://localhost:52764";

            try
            {
                reply = wc.DownloadString(baseurl);
            }
            catch (WebException e)
            {
                webException = e;              
            }
            
            //var urldoc = XDocument.Parse(reply);
            //var elements = urldoc.Elements("url");
            //foreach (var node in elements)
            //{
            //    Console.WriteLine("url " + node.Attribute("loc").Value);
            //    Console.WriteLine("priority " + node.Attribute("priority").Value);
            //    Console.WriteLine("last modified " + node.Attribute("lastmod").Value);
            //    Console.WriteLine("change frequency " + node.Attribute("changefreq").Value);
            //}
        }
        
        [Given(@"I visit each URL in the response")]
        public void GivenIVisitEachURLInTheResponse()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"I should see a valid sitemap")]
        public void ThenIShouldSeeAValidSitemap()
        {
            Assert.AreNotEqual(WebExceptionStatus.ConnectFailure, webException.Status);
            webResponse = webException.Response as HttpWebResponse;
            
            var urldoc = XDocument.Parse(reply);
            var elements = urldoc.Elements("url");

            Assert.IsTrue(elements.Any());
        }
        
        [Then(@"I should get no (.*) return codes")]
        public void ThenIShouldGetNoReturnCodes(int p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"I should get only (.*) return codes")]
        public void ThenIShouldGetOnlyReturnCodes(int p0)
        {
            ScenarioContext.Current.Pending();
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            var assembly = typeof(StartPageController).Assembly;
            process = IISExpress.StartIISExpressFromPath(assembly.Location, 52764);
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            process.Stop();
        }
    }
}
