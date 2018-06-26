# Tag Support

Tags are supported as a way to organize features and scenarios.  You can add any number of tags to a Gherkin feature or scenario.

## Example

Gherkin feature file ("./Addition/AddTwoNumbers.feature"):
```Gherkin
@addition
Feature: AddTwoNumbers
	In order to learn Math
	As a regular human
	I want to add two numbers using Calculator

Scenario: Add two numbers
	Given I chose 12 as first number
	And I chose 15 as second number
	When I press add
	Then the result should be 27 on the screen

@addition @bigaddition
Scenario: Add two bigger numbers
	Given I chose 120 as first number
	And I chose 150 as second number
	When I press add
	Then the result should be 270 on the screen
```

When executing tests, you can specify the tests which should be run using the `Category` attribute, for example:

`dotnet test --filter Category=bigaddition`

Notes:
- Tags applied to the feature are inherited by all scenarios in the same file.
