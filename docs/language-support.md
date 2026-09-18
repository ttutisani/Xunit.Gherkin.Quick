# Language Support

One of the advantages of Behaviour-Driven Development (BDD) is that feature files describe what part of a system should do in non-technical language that is concrete enough to capture the context and limitations of a feature in a human-readable form. Business people, software developers, testers and even end-users can go over a feature file and make sense of it.

As one would expect, software is developed by a wide range of people from many parts of the world meaning that not all features would be described in the default English language. The library relies on the Gherkin Parser to generate tests from feature files and they come with built-in support for handling multiple languages, even extending the available set through embedded resources.

After parsing a feature file, the exact same mechanisms for interpreting the translated keywords are used to map implementation steps to those defined in a feature file ensuring a 1-to-1 mapping throughout the parsing, generating tests and execution process.

The following feature is described in English, if no language specification is made then English is picked by default.

```Gherkin
Feature: Sum Numbers
    In order to learn Math
    As a regular human
    I want to sum multiple numbers

Scenario: Add two numbers with examples
    Given I have 10
    And I also have 20
    But I also have -5
    When I sum the numbers
    Then the result is 25
```

Everything else is as usual, define a feature class implementing each step, e.g.:

```c#
[FeatureFile(@"SumNumbers.feature")]
public sealed class HandleNotImplemented : Feature
{
    [Given("I have {int}")]
    public void GivenIHave(int number)
    {
        // ...
    }
}
```

The same feature can be described using Slovak keywords, add the language spec comment at the top of the feature file and update the keywords. The same implementation will work without any change.

```Gherkin
# language: sk
# https://cucumber.io/docs/gherkin/languages
Funkcia: AddTwoNumbers
    In order to learn Math
    As a regular human
    I want to add two numbers using Calculator


Scenár: Add two numbers with examples
    Pokiaľ I have 10
    A I also have 20
    Ale I also have -5
    Keď I sum the numbers
    Tak the result is 25
```

Keep in mind that the pattern matching on the step text itself remains the same across languages, if you want to describe them fully in a different language then you need to translate the text for each step as well. To support multiple languages at the same time, decorate the implementation method with multiple matching attributes, each for the supported language.