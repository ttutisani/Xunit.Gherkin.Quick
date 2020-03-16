Feature: Math with Infinity
	In order to learn Math
	As a regular human
	I want to add two numbers using Calculator

@ignore
Scenario: Add a number to infinity
	Given I chose INFINITY as first number
	And I chose 15 as second number
	When I press add
	Then the result should be INFINITY on the screen

Scenario Outline: Infinity math with examples
	Given I chose <a> as first number
	And I chose <b> as second number
	When I press add
	Then the result should be <sum> on the screen

	@ignore
	Examples:
		| a        | b        | sum      |
		| 0        | INFINITY | INFINITY |
		| INFINITY | INFINITY | INFINITY |

