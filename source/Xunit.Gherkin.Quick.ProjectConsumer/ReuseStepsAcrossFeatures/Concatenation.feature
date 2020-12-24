Feature: Concatenation
	As a text fan
	I want to experiment with concatenation
	So that my curiosity is fulfilled

Scenario: Name with First Last
	Given I type "John"
	And I type "Doe"
	When I ask to concatenate
	Then I receive "John Doe"
