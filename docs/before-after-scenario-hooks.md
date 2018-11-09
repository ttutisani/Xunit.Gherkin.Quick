# Before / After Scenario Execution Hooks

There are cases when you want to execute a code block before and/or after the scenario execution. "Xunit.Gherkin.Quick" framework does not add any specific handlers for these events, but it stays backwards compatible with Xunit's standard way to achieve this effect. In other words, just like you would do in a regular Xunit test class, you can make use of a constructor and "Dispose" method (implement `IDisposable` interface) to run a code block before and after the scenario execution, respectively.

Since the whole scenario (including its all steps) is treated as a single test method, the constructor and "Dispose" will run only before and after _all steps_, not in between. This is a natural expectation, but it's useful to clarify this here to avoid any confusion in use.

Below is an example of a feature implementation that outputs log messages before and after a scenario execution.

Feature file:
```Gherkin
Feature: Before/After Hooks
	As a BDD developer using Xunit.Gherkin.Quick
	I want to ensure that Before and After hooks work for Scenario
	So that I can write code that executes either before or after the Scenario

Scenario: Scenario Before and After
	Given first step executed
	And second step executed
```

Feature class:
```C#
public sealed class BeforeAfter : Feature, IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;

    public BeforeAfter(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        _testOutputHelper.WriteLine("Before executing scenario.");
    }

    [Given("first step executed")]
    public void First_Step_Executed()
    {
        _testOutputHelper.WriteLine("First step.");
    }

    [And("second step executed")]
    public void Second_Step_Executed()
    {
        _testOutputHelper.WriteLine("Second step.");
    }

    public void Dispose()
    {
        _testOutputHelper.WriteLine("After scenario execution.");
    }
}
```

When we execute the scenario named "Scenario Before and After", the output will contain the following log messages in the given order:

```log
Before executing scenario.
First step.
Second step.
After scenario execution.
```
