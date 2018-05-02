Feature: Given When Then
	In order to use Given When Then and other attributes
	As a Gherkin enthusiast
	I want to ensure they map to methods

Scenario: Simple parameter types
	Given Value 1 should be 1
	And Value WORD should be WORD
	But Value 1.23 should be 1.23
	When Value 1/2/2003 should be 1/2/2003
	And Value -12 should be value -12
	But Value -3.21 should be value -3.21
	Then Values 1 and 2 should be 1 and 2
	And Values WORD and 123.4 should be WORD and 123.4
	But Values 3/2/2001 and -3.14 should be values 3/2/2001 and -3.14