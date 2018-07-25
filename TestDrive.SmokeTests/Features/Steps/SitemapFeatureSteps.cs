using System;
using TechTalk.SpecFlow;

namespace TestDrive.SmokeTests.Features.Steps
{
    [Binding]
    public class SitemapFeatureSteps
    {
        [Given(@"I visit http://localhost:(.*)/sitemap\.xml")]
        public void GivenIVisitHttpLocalhostSitemap_Xml(int p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"I visit each URL in the response")]
        public void GivenIVisitEachURLInTheResponse()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"I should see a valid sitemap")]
        public void ThenIShouldSeeAValidSitemap()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"I should get no (.*) error codes")]
        public void ThenIShouldGetNoErrorCodes(int p0)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
