Feature: Sitemap Feature
	In order to avoid broken URLs
	As a site visitor
	I want to be sure all pages of /sitemap.xml returns valid return codes

@smoketest
Scenario: Can retrieve sitemap
	Given I visit http://localhost:50874/sitemap.xml
	Then I should see a valid sitemap
	
@smoketest
Scenario: No URL returns 404
	Given I visit /sitemap.xml
	And I visit each URL in the response
	Then I should get no 404 return codes

@smoketest
Scenario: No URL returns 500
	Given I visit /sitemap.xml
	And I visit each URL in the response
	Then I should get no 500 return codes
	
@smoketest
Scenario: All URLs returns 200 codes
	Given I visit /sitemap.xml
	And I visit each URL in the response
	Then I should get only 200 return codes