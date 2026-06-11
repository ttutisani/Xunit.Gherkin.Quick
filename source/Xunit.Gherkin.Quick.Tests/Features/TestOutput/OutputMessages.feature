Feature: Output Messages
	In order to help see what is received by scenario steps
	As a BDD tester
	I want to output messages from steps

Scenario: Log Strings
	Given output is available
	When I log message "hello word"
	Then I should see it in the output