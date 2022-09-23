Feature: AddTwoNumbersExtra
	In order to learn Math
	As a regular human
	I want to add two numbers using Calculator

Scenario: Sumar sarasa
	Given I chose 1 as first number
	And I chose 2 as second number
	And I pressed add
	And I saw 3 on the screen
	When I choose 4 as first number
	And I choose 5 as second number
	And I press add
	Then the result should be 9 on the screen