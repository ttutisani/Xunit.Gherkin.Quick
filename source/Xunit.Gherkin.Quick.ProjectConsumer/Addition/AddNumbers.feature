Feature: Add Numbers
	In order to learn Math
	As a regular human
	I want to add numbers using a Calculator

Scenario: Add two numbers
	Given I chose 12 as first number
	And I chose 15 as second number
	When I press add
	Then the result should be 27 on the screen

Scenario: Add three numbers using a table of data
	Given I have chosen the following table of numbers
	| Num1 | Num2 | Num3 |
	| 1    | 2    | 3    |
	When I press add
	Then the result should be 6 on the screen

Scenario: Add five numbers using a multiline string
	Given I have chosen the following list of numbers
	"""
	10
	20
	30
	40
	50
	"""
	When I press add
	Then the result should be 150 on the screen