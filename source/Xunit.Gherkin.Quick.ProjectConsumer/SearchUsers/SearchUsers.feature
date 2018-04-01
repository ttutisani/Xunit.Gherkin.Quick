Feature: SearchUsers
	In order to provide a user management feature
	As a member of HR staff
	I want to find a user

Scenario: Find user
	Given there are users:
	   | username | email               |
	   | everzet  | everzet@knplabs.com |
	   | fabpot   | fabpot@symfony.com  |
	When I search for 'everzet'
	Then the result should have an email of 'everzet@knplabs.com'
