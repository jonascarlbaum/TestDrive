using Holf.AllForOne;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using TechTalk.SpecFlow;
using TestDrive.SmokeTests.Helpers;

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
        private Dictionary<string, HttpStatusCode> responses = new Dictionary<string, HttpStatusCode>();
        private static Process process;

        [Given(@"I visit /sitemap\.xml")]
        public void GivenIVisitSitemap_Xml()
        {
            var baseurl = "http://localhost:52764";

            try
            {
                reply = wc.DownloadString($"{baseurl}/sitemap.xml");
            }
            catch (WebException e)
            {
                webException = e;              
            }
        }
        
        [Given(@"I visit each URL in the response")]
        public void GivenIVisitEachURLInTheResponse()
        {
            var xdoc = XDocument.Parse(reply);
            var ns = xdoc.Root.GetDefaultNamespace();
            var urls = xdoc.Root.Elements().Elements(ns + "loc").Select(l => (string)l);
            
            foreach (var url in urls)
            {
                try
                {
                    wc.DownloadString(url);
                    responses[url] = HttpStatusCode.OK;
                }
                catch (WebException e)
                {
                    var response = (HttpWebResponse)e?.Response ?? null;
                    responses[url] = response?.StatusCode ?? HttpStatusCode.RequestUriTooLong;
                }
            }
        }
        
        [Then(@"I should see a valid sitemap")]
        public void ThenIShouldSeeAValidSitemap()
        {
            var xdoc = XDocument.Parse(reply);
            var ns = xdoc.Root.GetDefaultNamespace();
            var elements = xdoc.Root.Elements(ns + "url");

            /*            
            <?xml version="1.0" encoding="utf-8"?>
                <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                  <url>
                    <loc>http://localhost:52764/en/about-us/news-events/events/reporting-made-simple/</loc>
                    <lastmod>2018-07-25T23:27:04+02:00</lastmod>
                    <changefreq>weekly</changefreq>
                    <priority>0.5</priority>
                  </url>
                </urlset>
             */

            Assert.IsTrue(elements.Any());
        }
        
        [Then(@"I should get no (.*) return codes")]
        public void ThenIShouldGetNoReturnCodes(int p0)
        {
            foreach(var response in responses)
            {
                Assert.AreNotEqual(p0, (int)response.Value);
            }
        }
        
        [Then(@"I should get only (.*) return codes")]
        public void ThenIShouldGetOnlyReturnCodes(int p0)
        {
            foreach (var response in responses)
            {
                Assert.AreEqual(p0, (int)response.Value);
            }
        }

        [BeforeFeature]
        public static void Before()
        {
            var assembly = Assembly.GetAssembly(typeof(TestDrive.Web.Global));
            var index = assembly.Location.LastIndexOf("\\TestDrive.SmokeTests\\");
            var projectDir = $"{assembly.Location.Remove(index)}\\TestDrive.Web";

            Console.WriteLine($"Project dir should be [{projectDir}]");

            try
            {
                process = IISExpress.StartIISExpressFromPath(projectDir, 52764);
            } catch (Exception e)
            {
                Console.Error.WriteLine($"Seems [{projectDir}] isn't a valid directory...", e);
                throw e;
            }

            if (process != null)
                process.TieLifecycleToParentProcess();
        }

        [AfterFeature]
        public static void After()
        {
            if (process == null)
                return;

            if (!process.HasExited)
                process.Kill();

            process.Stop();
            process.Dispose();
        }
    }
}
