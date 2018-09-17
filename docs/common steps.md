# New features

Global / common steps

This feature allows you to define steps in a separate feature file. This is handy is a step is defined in multiple separate feature files.
These common steps can be defined in a separate class (and marked with the appropriate gherkin attribute). Features that want to make use of these common steps can use the IncludeSteps attribute to define which classes they want to include steps from.


To save some coding, we map similar steps to the same method. Here is how:
```C#
[FeatureFile("MyFirstFeature.feature")]
[IncludeSteps(typeof(SharedSteps)]
public class MyFirstFeature : Feature
{    
    [When(@"I do something")]
    public void DoSomething()
    {
        ...
    }    

    [Then(@"the result is as expected")]    
    public void CheckResult()
    {
        ...
    }
}

[FeatureFile("MyOtherFeature.feature")]
[IncludeSteps(typeof(SharedSteps)]
public class MyOtherFeature : Feature
{    
    [When(@"I do something else")]
    public void DoSomethingElse()
    {
        ...
    }    

    [Then(@"the result is as expected for this thing")]    
    public void CheckResult()
    {
        ...
    }
}

public class SharedSteps
{
	[Given("A common setup step")]
	public void CommonSetupStep()
	{
		...
	}
}

```

Scenario context

Having the ability to share steps usually comes with the requirement to be able to pass state around. This can now be done with the ScenarioContext dictionary. This is available as a property on the Feature class. It can also be used with common steps, in order to do so the class implementing the common steps should derive from StepsContainer.

```C#
[FeatureFile("MyFirstFeature.feature")]
[IncludeSteps(typeof(SharedSteps)]
public class MyFirstFeature : Feature
{    
    [When(@"I do something")]
    public void DoSomething()
    {
        var importantObject = ScenarioContext["ImportantObject"];
		importantObject.DoSomething();
    }    

    [Then(@"the result is as expected")]    
    public void CheckResult()
    {
        ...
    }
}

public class SharedSteps : StepsContainer
{
	[Given("A common setup step")]
	public void CommonSetupStep()
	{
		var importantObject = new ImportantObject();
		ScenarioContext["ImportantObject"] = importantObject;
	}
}

```

Background 

Allows background steps to be defined in the gherkin file.

```Gherkin
Feature: Adding from 1
	In order to learn Math
	As a regular human
	I want to add two numbers using Calculator
	
Background:
	Given I chose 1 as the first number

Scenario: Add 4
	When I choose 4 as second number	
	Then the result should be 5
	
Scenario: Add 5
	When I choose 5 as second number	
	Then the result should be 6
	
```

The steps from the background are prepended to the list of steps for a scenario when it runs. These steps should be defined in the usual way.