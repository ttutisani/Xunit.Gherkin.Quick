# Scenario Background

Background is a set of scenario steps which run before every scenario (and scenario outline) in the same feature file.

This can help eliminate duplication from several scenarios that have the same first steps. In such case, simply put those similar first steps in the background, and remove duplications from the actual scenarios.

Alternatively, background can be seen as a "setup" that should happen before every scenario and scenario outline.

For example, below Gherkin feature defines a background with a "Given" step that is common among all scenarios and outlines in this file:

```Gherkin
Feature: Adding numbers to 5
    As a fanatic of "5"
    I want to add other numbers to it
    So that I can see the whole power of "5"

Background:
    Given I chose 5 as first number

Scenario: Add 1 to 5
    And I chose 1 as second number
    When I press add
    Then the result should be 6 on the screen

Scenario: Add 5 to 5
    And I chose 5 as second number
    When I press add
    Then the result should be 10 on the screen

Scenario Outline: Add various numbers to 5
    And I chose <b> as second number
    When I press add
    Then the result should be <c> on the screen

    Examples:
        | b | c  |
        | 1 | 6  |
        | 5 | 10 |

    Examples: of negative numbers
        | b  | c |
        | -1 | 4 |
        | -5 | 0 |
```

This feature file can be handled by using a feature class as always. Thought process goes with assumption that the background step is prepended to every scenario, as if it's repeated under every scenario definition.

```C#
[FeatureFile("./Addition/AddNumbersTo5.feature")]
public sealed class AddNumbersTo5 : Feature
{
    private readonly Calculator _calculator = new Calculator();

    [Given(@"I chose (-?\d+) as first number")]
    public void Given_I_chose_x_as_first_number(int firstNumber)
    {
        _calculator.SetFirstNumber(firstNumber);
    }

    [And(@"I chose (-?\d+) as second number")]
    public void And_I_chose_y_as_second_number(int secondNumber)
    {
        _calculator.SetSecondNumber(secondNumber);
    }

    [When(@"I press add")]
    public void When_I_press_add()
    {
        _calculator.AddNumbers();
    }

    [Then(@"the result should be (-?\d+) on the screen")]
    public void Then_the_result_should_be_z_on_the_screen(int expectedResult)
    {
        var actualResult = _calculator.Result;

        Assert.Equal(expectedResult, actualResult);
    }
}
```
