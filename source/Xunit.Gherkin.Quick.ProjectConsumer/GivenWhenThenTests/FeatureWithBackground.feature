Feature: With Background
	In order to use common steps at the start of each scenario
	As a Gherkin enthusiast
	I want to define a background step in my feature

	Background:
		Given a simple background

	Scenario: Ensure background step is run first
		When a sample step is run
		Then the steps are run in order
