using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace TestDrive.SmokeTests.Steps
{
    [Binding]
    public class SitemapFeatureSteps
    {
        private static WebClient GetNewWebClient()
        {
            return new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
        }

        private string reply = null;
        private Dictionary<string, HttpStatusCode> responses = new Dictionary<string, HttpStatusCode>();

        [Given(@"I visit /sitemap\.xml")]
        public void GivenIVisitSitemap_Xml()
        {
            var baseurl = $"{ConfigurationManager.AppSettings["Server:Url"]}:{ConfigurationManager.AppSettings["Server:Port"]}";

            try
            {
                using (var wc = GetNewWebClient())
                {
                    reply = wc.DownloadString(new Uri($"{baseurl}/sitemap.xml"));
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("ERROR fetching sitemap!", e);
                throw e;
            }
        }

        [Given(@"I visit each URL in the response")]
        public void GivenIVisitEachURLInTheResponse()
        {
            Assert.IsNotNull(reply);
            Assert.IsNotEmpty(reply);

            var xdoc = XDocument.Parse(reply);
            var ns = xdoc.Root.GetDefaultNamespace();
            var urls = xdoc.Root.Elements().Elements(ns + "loc").Select(l => (string)l);

            foreach (var url in urls)
            {
                try
                {
                    var urlBuilder = new UriBuilder(url)
                    {
                        Port = int.Parse(ConfigurationManager.AppSettings["Server:Port"])
                    };

                    using (var wc = GetNewWebClient())
                    {
                        wc.DownloadString(urlBuilder.ToString());
                    }

                    responses[url] = HttpStatusCode.OK;
                }
                catch (WebException e)
                {
                    Console.Error.WriteLine($"Failure visiting '{url}' => {e.InnerException?.ToString()}", e);
                    responses[url] = ((HttpWebResponse)e?.Response)?.StatusCode ?? HttpStatusCode.BadRequest;
                    throw e;
                }
            }
        }

        [Then(@"I should see a valid sitemap")]
        public void ThenIShouldSeeAValidSitemap()
        {
            Assert.IsNotNull(reply);
            Assert.IsNotEmpty(reply);

            IEnumerable<XElement> elements = null;
            try
            {
                var xdoc = XDocument.Parse(reply);
                var ns = xdoc.Root.GetDefaultNamespace();
                elements = xdoc.Root.Elements(ns + "url");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error parsing reply {reply}", e);
                throw e;
            }

            Assert.IsNotNull(elements);
            Assert.IsTrue(elements.Any());
        }

        [Then(@"I should get no (.*) return codes")]
        public void ThenIShouldGetNoReturnCodes(int p0)
        {
            Assert.True(responses.All((x) => (int)x.Value != p0));
        }

        [Then(@"I should get only (.*) return codes")]
        public void ThenIShouldGetOnlyReturnCodes(int p0)
        {
            Assert.True(responses.All((x) => (int)x.Value == p0));
        }
    }
}
