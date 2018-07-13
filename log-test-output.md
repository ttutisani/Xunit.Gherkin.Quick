# Logging Output from Scenario Steps

Since this framework relies on Xunit for execution, it's natural that you can output messages from your scenario steps (i.e. test methods) through `ITestOutputHelper` interface, which is defined within Xunit framework.

To acquire instance of the output, simply define a constructor argument of type `ITestOutputHelper`. This is optional, and you don't have to define such argument at all times; Only when you need to output string messages from your methods.

For example:

```C#
[FeatureFile(@"./TestOutput/OutputMessages.feature")]
public sealed class OutputMessages : Feature
{
    private readonly ITestOutputHelper _testOutputHelper;

    public OutputMessages(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Given(@"output is available")]
    public void Given_output_is_available()
    {
        Assert.NotNull(_testOutputHelper);
    }

    [When(@"I log message ""(.+)""")]
    public void Given_message(string message)
    {
        _testOutputHelper.WriteLine(message);
    }

    [Then(@"I should see it in the output")]
    public void Then_I_should_see_it_in_the_output()
    {
        //check the output.
    }
}
```

Notice that the constructor has an argument `ITestOutputHelper testOutputHelper`, which can be used later on (as in the `Given_message` method above) to output messages to the standard test output pane.

* * *

Obviously, this example assumes that there is a feature file which has corresponding steps matching these methods. Such as this one:

File: OutputMessages.feature
```Gherkin
Feature: Output Messages
	In order to help see what is received by scenario steps
	As a BDD tester
	I want to output messages from steps

Scenario: Log Strings
	Given output is available
	When I log message "hello word"
	Then I should see it in the output
```
