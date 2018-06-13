Feature: AddTwoNumbers Async
	In order to learn Math
	As a regular human
	I want to add two numbers using Calculator Asynchronously

Scenario: Add two numbers
	Given I chose 12 as first number
	And I chose 15 as second number
	When I press add
	Then the result should be 27 on the screen
