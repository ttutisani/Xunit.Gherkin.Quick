Feature: Concatenation
	As a text fan
	I want to experiment with concatenation
	So that my curiosity is fulfilled

Scenario: Name with Last First
	Given I type "John"
	And I type "Doe"
	When I ask to inverse concatenate
	Then I receive "Doe, John"
