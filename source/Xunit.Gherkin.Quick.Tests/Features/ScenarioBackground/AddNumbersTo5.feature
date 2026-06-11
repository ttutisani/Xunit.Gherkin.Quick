Feature: Adding numbers to 5
	As a fanatic of "5"
	I want to add other numbers to it
	So that I can see the whole power of "5"

Background:
	Given I chose 5 as first number

Scenario: Add 1 to 5
	And I chose 1 as second number
	When I press add
	Then the result should be 6 on the screen

Scenario: Add 5 to 5
	And I chose 5 as second number
	When I press add
	Then the result should be 10 on the screen

Scenario Outline: Add various numbers to 5
	And I chose <b> as second number
	When I press add
	Then the result should be <c> on the screen

	Examples:
		| b | c  |
		| 1 | 6  |
		| 5 | 10 |

	Examples: of negative numbers
		| b  | c |
		| -1 | 4 |
		| -5 | 0 |