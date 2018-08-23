Feature: AddTwoNumbers
	In order to learn Math
	As a regular human
	I want to add two numbers using Calculator

@addition
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


Scenario: Add various pairs of numbers
	Given following table of 4 inputs and outputs:
		| Number 1 | Number 2 | Result |
		| 1        | 1        | 2      |
		| 10       | 20       | 30     |
		| 10       | 11       | 21     |
		| 111      | 222      | 333    |


Scenario Outline: Add two numbers with examples
	Given I chose <a> as first number
	And I chose <b> as second number
	When I press add
	Then the result should be <sum> on the screen

	@addition
	Examples:
		| a   | b   | sum |
		| 0   | 1   | 1   |
		| 1   | 9   | 10  |

	Examples: of bigger numbers
		| a   | b   | sum |
		| 99  | 1   | 100 |
		| 100 | 200 | 300 |

	@bigaddition
	Examples: of large numbers
		| a    | b | sum   |
		| 999  | 1 | 1000  |
		| 9999 | 1 | 10000 |

	@ignore
	Examples: of floating point numbers
		| a   | b   | sum |
		| 1.1 | 2.2 | 3.3 |

@ignore
Scenario: Add floating point numbers
	Given I chose 1.11 as first number
	And I chose 2.22 as second number
	When I press add
	Then the result should be 3.33 on the screen

Scenario: Add numbers after seeing result
	Given I chose 1 as first number
	And I chose 2 as second number
	And I pressed add
	And I saw 3 on the screen
	When I choose 4 as first number
	And I choose 5 as second number
	And I press add
	Then the result should be 9 on the screen