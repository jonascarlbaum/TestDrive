using System;
using TechTalk.SpecFlow;

namespace TestDrive.SmokeTests.Features.Steps
{
    [Binding]
    public class SitemapFeatureSteps
    {   
        [Given(@"I visit /sitemap\.xml")]
        public void GivenIVisitSitemap_Xml()
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
    }
}
