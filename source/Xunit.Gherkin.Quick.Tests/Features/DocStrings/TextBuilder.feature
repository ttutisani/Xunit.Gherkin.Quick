Feature: Text Builder
	In order to manipulate text
	As a text nerd
	I need to have a Text Builder mechanism

Scenario: Replacing words in text
	Given I have a text like this:
	"""
	Hello word

	The word is beautiful.
	Isn't it?

	I'm coming word
	"""
	When I replace 'word' with 'world'
	Then I should have text like this:
	"""
	Hello world

	The world is beautiful.
	Isn't it?

	I'm coming world
	"""