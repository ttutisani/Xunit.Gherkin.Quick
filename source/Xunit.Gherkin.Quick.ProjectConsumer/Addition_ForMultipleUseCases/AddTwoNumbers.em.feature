
# language: em

📚: AddTwoNumbers
	In order to learn Math
	As a regular human
	I want to add two numbers using Calculator

@addition
📕: Add two numbers
	😐 I chose 12 as first number
	😂 I chose 15 as second number
	🎬 I press add
	🙏 the result should be 27 on the screen

@addition @bigaddition
📕: Add two bigger numbers
	😐 I chose 120 as first number
	😂 I chose 150 as second number
	🎬 I press add
	🙏 the result should be 270 on the screen


📕: Add various pairs of numbers
	😐 following table of 4 inputs and outputs:
		| Number 1 | Number 2 | Result |
		| 1        | 1        | 2      |
		| 10       | 20       | 30     |
		| 10       | 11       | 21     |
		| 111      | 222      | 333    |


📖: Add two numbers with examples
	😐 I chose <a> as first number
	😂 I chose <b> as second number
	🎬 I press add
	🙏 the result should be <sum> on the screen

	@addition
	📓:
		| a   | b   | sum |
		| 0   | 1   | 1   |
		| 1   | 9   | 10  |

	📓: of bigger numbers
		| a   | b   | sum |
		| 99  | 1   | 100 |
		| 100 | 200 | 300 |

	@bigaddition
	📓: of large numbers
		| a    | b | sum   |
		| 999  | 1 | 1000  |
		| 9999 | 1 | 10000 |

	@ignore
	📓: of floating point numbers
		| a   | b   | sum |
		| 1.1 | 2.2 | 3.3 |

@ignore
📕: Add floating point numbers
	😐 I chose 1.11 as first number
	😂 I chose 2.22 as second number
	🎬 I press add
	🙏 the result should be 3.33 on the screen

📕: Add numbers after seeing result
	😐 I chose 1 as first number
	😂 I chose 2 as second number
	😂 I pressed add
	😂 I saw 3 on the screen
	🎬 I choose 4 as first number
	😂 I choose 5 as second number
	😂 I press add
	🙏 the result should be 9 on the screen