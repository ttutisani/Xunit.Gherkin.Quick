# Multi Line Text (DocString) Argument Support

Scenario step can have a multi-line text as an argument in Gherkin. This value can be mapped to the argument of type `DocString` in your step method.

This is useful when you need to describe an input or an output as a large text consisting of several lines.

For example, consider this Gherkin feature file, where we have a multi line text present for both Given and Then steps:
```Gherkin
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
```

This is how you can handle these multi-line text arguments in your step methods in the feature class:

```C#
    [FeatureFile("./Texts/TextBuilder.feature")]
    public sealed class TextBuilder : Feature
    {
        private StringBuilder _textBuilder;

        [Given("I have a text like this:")]
        public void I_have_a_text_like_this(DocString text)
        {
            _textBuilder = new StringBuilder(text.Content);
        }

        [When(@"I replace '(\w+)' with '(\w+)'")]
        public void I_replace_x_with_y(string oldValue, string newValue)
        {
            _textBuilder.Replace(oldValue, newValue);
        }

        [Then("I should have text like this:")]
        public void I_should_have_text_like_this(DocString text)
        {
            Assert.Equal(text.Content, _textBuilder.ToString());
        }
    }
```

Note that both Given and Then methods have an argument of type `DocString`. This argument will receive the multi-line text value from the scenario steps defined above.

`DocString` type is defined inside Gherkin framework, which is a dependency for the `Xunit.Gherkin.Quick` framework itself.
