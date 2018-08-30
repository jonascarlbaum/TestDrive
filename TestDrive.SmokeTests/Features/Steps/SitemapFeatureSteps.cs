using Holf.AllForOne;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TechTalk.SpecFlow;
using TestDrive.SmokeTests.Helpers;

namespace TestDrive.SmokeTests.Features.Steps
{
    [Binding]
    public class SitemapFeatureSteps
    {
        private static WebClient GetNewClient()
        {
            return new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
        }

        private string reply = null;
        private WebException webException;
        private Dictionary<string, HttpStatusCode> responses = new Dictionary<string, HttpStatusCode>();
        private static Process process;

        [Given(@"I visit /sitemap\.xml")]
        public void GivenIVisitSitemap_Xml()
        {
            var baseurl = ConfigurationManager.AppSettings["Server:Url"];

            try
            {
                //reply = GetAsync($"{baseurl}/sitemap.xml").Result;
                //Console.Error.WriteLine("SUCCESS fetching sitemap!");
                using (var wc = GetNewClient())
                {
                    reply = wc.DownloadStringTaskAsync($"{baseurl}/sitemap.xml").Result;
                    Console.Error.WriteLine("SUCCESS fetching sitemap!");
                }
            }
            catch (WebException e)
            {
                webException = e;
                Console.Error.WriteLine("ERROR fetching sitemap!", e);
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
                    Console.Error.WriteLine($"Visit '{url}'");

                    using (var wc = GetNewClient())
                    {
                        var urlBuilder = new UriBuilder(url)
                        {
                            Port = int.Parse(ConfigurationManager.AppSettings["Server:Port"])
                        };

                     wc.DownloadStringTaskAsync(new Uri(urlBuilder.ToString()));
                    //Thread.Sleep(10 + (urls.ToList().IndexOf(url) % 5 == 0 ? 200 : 0));

                    //var kaka = GetAsync(urlBuilder.ToString()).Result;
                    }

                    responses[url] = HttpStatusCode.OK;
                    Console.Error.WriteLine($"Success visiting '{url}'");
                }
                catch (WebException e)
                {
                    Console.Error.WriteLine($"Failure visiting '{url}' => {e.InnerException?.ToString()}", e);
                    var response = (HttpWebResponse)e?.Response ?? null;
                    responses[url] = response?.StatusCode ?? HttpStatusCode.BadRequest;
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
            Assert.True(responses.All((x) => (int)x.Value != p0));
            //foreach(var response in responses)
            //{
            //    Assert.AreNotEqual(p0, (int)response.Value);
            //}
        }
        
        [Then(@"I should get only (.*) return codes")]
        public void ThenIShouldGetOnlyReturnCodes(int p0)
        {
            Assert.True(responses.All((x) => (int)x.Value == p0));
            //foreach (var response in responses)
            //{
            //    Assert.AreEqual(p0, (int)response.Value);
            //}
        }


        [BeforeScenario]
        public static void BeforeScenario()
        {
            //System.Net.ServicePointManager.DefaultConnectionLimit = 1000;
        }

        [AfterScenario]
        public static void AfterScenario()
        {
        }

        [BeforeFeature]
        public static void Before()
        {
            var assembly = Assembly.GetAssembly(typeof(TestDrive.Web.Global));
            var index = assembly.Location.LastIndexOf("\\TestDrive.SmokeTests\\");
            var projectDir = $"{assembly.Location.Remove(index)}\\TestDrive.Web";

            //Console.Error.WriteLine($"Project dir should be [{projectDir}]");

            try
            {
                process = IISExpress.StartIISExpressFromPath(projectDir, int.Parse(ConfigurationManager.AppSettings["Server:Port"]));
            } catch (Exception e)
            {
                Console.Error.WriteLine($"Seems [{projectDir}] isn't a valid directory...", e);
                throw e;
            }
            
            if (process != null)
                process.TieLifecycleToParentProcess();
            
            //var baseurl = ConfigurationManager.AppSettings["Server:Url"];

            //try
            //{
            //    using (var wc = GetNewClient())
            //    {
            //        wc.DownloadString($"{baseurl}/sitemap.xml");
            //        Thread.Sleep(15000);
            //    }
            //}
            //catch (WebException e)
            //{
            //    Console.Error.WriteLine("ERROR fetching sitemap!", e);
            //}
        }

        [AfterFeature]
        public static void After()
        {
            try
            {
                if (process == null)
                    return;

                if (!process.HasExited)
                    process.Kill();

                process.Stop();
                process.Dispose();

                process = null;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error in AfterFeature", e);
            }
        }

        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
