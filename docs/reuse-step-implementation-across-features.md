# Reuse Step Implementation Across Features

Let us assume that we need to implement two Gherkin features that sound almost identical. You might want to avoid code duplication and reuse the implementation across feature classes in such a situation.

Below are the two features with almost identical Gherkin text, except for the "When" step.

Feature one:

```Gherkin
Feature: Concatenation
	As a text fan
	I want to experiment with concatenation
	So that my curiosity is fulfilled

Scenario: Name with First Last
	Given I type "John"
	And I type "Doe"
	When I ask to concatenate
	Then I receive "John Doe"
```

Feature two:

```Gherkin
Feature: Concatenation
	As a text fan
	I want to experiment with concatenation
	So that my curiosity is fulfilled

Scenario: Name with Last First
	Given I type "John"
	And I type "Doe"
	When I ask to inverse concatenate
	Then I receive "Doe, John"
```

As you can see, the two scenarios that we need to implement have identical steps, except the "When" part. Since all other steps sound similar, we could share their implementation code too.

In Xunit.Gherkin.Quick, there are two options for achieving this objective:

- Inheriting from a common base class, or 
- Injecting a class with common code in it.

## Option 1: Inheriting from Common Base Class

Since all feature classes must inherit from `Feature` class, our base class should also follow this design. We can also make the base class abstract since we will not need to execute it on its own. The base class holds the shared step implementations for all derived features.

```C#
public abstract class ConcatenationBase : Feature
{
    protected string FirstName { get; private set; }

    [Given(@"I type ""([\w]+)""")]
    public void Given_I_type(string firstName)
    {
        FirstName = firstName;
    }

    protected string LastName { get; private set; }

    [And(@"I type ""([\w]+)""")]
    public void And_I_type(string lastName)
    {
        LastName = lastName;
    }

    private string _concatenationRsult;

    //HACK: this will not need to exist in real tests.
    protected void SetConcatenationResult(string result)
    {
        _concatenationRsult = result;
    }

    [Then(@"I receive ""([\w\s,]+)""")]
    public void Then_I_receive(string fullName)
    {
        Assert.Equal(fullName, _concatenationRsult);
    }
}
```

We can now start implementing the originally shown two features via their designated feature classes. Since we inherit from the above base class, most of the work is already done thanks to the inheritance (child classes inherit all members of a base class).

The derived classes need to implement only the "when" step that is different between the features. Below is a sample implementation of both feature classes.

Feature one:

```C#
[FeatureFile("./ReuseStepsAcrossFeatures/Concatenation.feature")]
public sealed class Concatenation : ConcatenationBase
{
    [When(@"I ask to concatenate")]
    public void When_I_ask_to_concatenate()
    {
        //HACK: must call an application to calculate result.
        base.SetConcatenationResult($"{base.FirstName} {base.LastName}");
    }
}
```

Feature two:

```C#
[FeatureFile("./ReuseStepsAcrossFeatures/InverseConcatenation.feature")]
public sealed class InverseConcatenation : ConcatenationBase
{
    [When(@"I ask to inverse concatenate")]
    public void When_I_ask_to_inverse_concatenate()
    {
        //HACK: must call an application to calculate result.
        base.SetConcatenationResult($"{base.LastName}, {base.FirstName}");
    }
}
```

## Option 2: Injecting Class with Common Code

Since the Xunit.Gherkin.Quick relies on Xunit for execution, it can also take advantage of the dependency injection features that the Xunit provides. These circumstances are handy if you want to implement a common code in one class and then inject it into your features.

Here is an example of the common code for implementing the presented two Gherkin features:

```C#
public sealed class ConcatenationCommonSteps
{
    public string FirstName { get; private set; }

    public void Given_I_type(string firstName)
    {
        FirstName = firstName;
    }

    public string LastName { get; private set; }

    public void And_I_type(string lastName)
    {
        LastName = lastName;
    }

    public string ConcatenationRsult { get; private set; }

    //HACK: this will not need to exist in real tests.
    public void SetConcatenationResult(string result)
    {
        ConcatenationRsult = result;
    }

    public void Then_I_receive(string fullName)
    {
        Assert.Equal(fullName, ConcatenationRsult);
    }
}
```

Now, we can implement the feature classes and inject this common code into both of them. Xunit's dependency injection is achieved by specifying `IClassFixture<T>` interface for the feature class, which tells Xunit what needs to be injected into the class's constructor.

Also, notice that the feature classes' scenario step methods do nothing but redirect calls to the common code. The only difference between the two implementations is the "When" step that was also different in the originally shown Gherkin feature texts.

Feature one:

```C#
[FeatureFile("./ReuseStepsAcrossFeatures/Concatenation.feature")]
public sealed class Concatenation : Feature, IClassFixture<ConcatenationCommonSteps>
{
    private readonly ConcatenationCommonSteps _steps;

    public Concatenation(ConcatenationCommonSteps steps)
    {
        _steps = steps;
    }

    [Given(@"I type ""([\w]+)""")]
    public void Given_I_type(string firstName) => _steps.Given_I_type(firstName);

    [And(@"I type ""([\w]+)""")]
    public void And_I_type(string lastName) => _steps.And_I_type(lastName);

    [When(@"I ask to concatenate")]
    public void When_I_ask_to_concatenate()
    {
        //HACK: must call an application to calculate result.
        _steps.SetConcatenationResult($"{_steps.FirstName} {_steps.LastName}");
    }

    [Then(@"I receive ""([\w\s,]+)""")]
    public void Then_I_receive(string fullName) => _steps.Then_I_receive(fullName);
}
```

Feature two:

```C#
[FeatureFile("./ReuseStepsAcrossFeatures/InverseConcatenation.feature")]
public sealed class InverseConcatenation : Feature, IClassFixture<ConcatenationCommonSteps>
{
    private readonly ConcatenationCommonSteps _steps;

    public InverseConcatenation(ConcatenationCommonSteps steps)
    {
        _steps = steps;
    }

    [Given(@"I type ""([\w]+)""")]
    public void Given_I_type(string firstName) => _steps.Given_I_type(firstName);

    [And(@"I type ""([\w]+)""")]
    public void And_I_type(string lastName) => _steps.And_I_type(lastName);

    [When(@"I ask to inverse concatenate")]
    public void When_I_ask_to_inverse_concatenate()
    {
        //HACK: must call an application to calculate result.
        _steps.SetConcatenationResult($"{_steps.LastName}, {_steps.FirstName}");
    }

    [Then(@"I receive ""([\w\s,]+)""")]
    public void Then_I_receive(string fullName) => _steps.Then_I_receive(fullName);
}

```
