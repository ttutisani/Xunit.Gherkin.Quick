# Shared Step Method With Multiple Step Attributes

In some cases, you will want to share the single method among steps. i.e. your method in the `Feature` class needs to handle multiple scenario steps listed in the Gherkin feature file. To achieve this, simply apply multiple step attributes to the same method.

Caution: your method will be executed as many timeas as many step patterns match with the attributes. So ensure that the code within your method is written in the respective fashion when using this tactic.

For example, assume the following feature file:
```Gherkin
Feature: AddTwoNumbers
	In order to learn Math
	As a regular human
	I want to add two numbers using Calculator

Scenario: Add numbers after seeing result
	Given I chose 1 as first number
	And I chose 2 as second number
	And I pressed add
	And I saw 3 on the screen
	When I choose 4 as first number
	And I choose 5 as second number
	And I press add
	Then the result should be 9 on the screen
```

To save some coding, we map similar steps to the same method. Here is how:
```C#
[FeatureFile("./Addition/AddTwoNumbers.feature")]
public sealed partial class AddTwoNumbers : Feature
{
    private readonly Calculator _calculator = new Calculator();

    [Given(@"I chose (\d+) as first number")]
    [When(@"I choose (\d+) as first number")]
    public void I_chose_first_number(int firstNumber)
    {
        _calculator.SetFirstNumber(firstNumber);
    }

    [And(@"I chose (\d+) as second number")]
    [And(@"I choose (\d+) as second number")]
    public void I_chose_second_number(int secondNumber)
    {
        _calculator.SetSecondNumber(secondNumber);
    }

    [And(@"I pressed add")]
    [And(@"I press add")]
    public void I_press_add()
    {
        _calculator.AddNumbers();
    }

    [Then(@"the result should be (\d+) on the screen")]
    [And(@"I saw (\d+) on the screen")]
    public void The_result_should_be_z_on_the_screen(int expectedResult)
    {
        var actualResult = _calculator.Result;

        Assert.Equal(expectedResult, actualResult);
    }
}
```

As you can see, we had to only define 4 methods in our `Feature` class, even though our Gherkin feature file lists 8 steps for the same scenario.

*A word of caution*: this approach should not be used to dictate the design decisions around your tested classes and systems (i.e. don't design multi-call operations just because this framework allows to test them - e.g. I didn't redesign `Calculator` for this test, it just happened to be working that way - I can call same methods on it many times). Instead, this approach should be used when the system is naturally designed so that the same call will need to happen multiple times. In such cases, this approach will help save code size needed to test such systems.
