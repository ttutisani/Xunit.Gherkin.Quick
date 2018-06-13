﻿Feature: AddTwoNumbers
	In order to learn Math
	As a regular human
	I want to add two numbers using Calculator

Scenario: Add two numbers
	Given I chose 12 as first number
	And I chose 15 as second number
	When I press add
	Then the result should be 27 on the screen

Scenario: Add various pairs of numbers
	Given following table of inputs and outputs:
		| Number 1 | Number 2 | Result |
		| 1        | 1        | 2      |
		| 10       | 20       | 30     |
		| 10       | 11       | 21     |
		| 111      | 222      | 333    |