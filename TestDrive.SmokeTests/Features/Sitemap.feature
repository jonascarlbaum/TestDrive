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
	Given I visit http://localhost:50874/sitemap.xml
	And I visit each URL in the response
	Then I should get no 404 error codes