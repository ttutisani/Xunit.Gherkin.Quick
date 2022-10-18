# Handling Not-Implemented Feature Files

There are numerous ways of practicing BDD: 

In some cases, BDD is completely driven by engineers, when they write both the feature files and their test code at the same time. In such a case, perhaps you don't need to worry about this topic.

In other cases, feature files are written by non-engineers (e.g., domain experts), and the engineers need to implement the test code for those features. In this scenario, you might want to discover and run every scenario that is dropped under the test project's sub-folder, irrespective of whether the test code is already written for these features. For example, you might want to fail the newly discovered scenarios so that the test output clearly states that they are not yet implemented. If necessary, you could also decide to skip the new feature until it's implemented.

This topic focuses on this use case and explains how to achieve the described outcome.

Add a class to the project which derives from `MissingFeature` base class. The purpose of this class is to opt-in for the discovery of the not-implemented feature files:

```C#
[FeatureFileSearchPattern("*.feature")]
public sealed class HandleNotImplemented : MissingFeature
{
}
```

**Note**: `FeatureFileSearchPattern` attribute is optional. If all your feature files have an extension of `.feature`, then they will be discovered automatically. If your files are named differently, then you will need to specify their name pattern via this attribute. For example, if your feature files are named as `xxx.gherkin`, then your attribute should look like `[FeatureFileSearchPattern("*.gherkin")]`. If you fail to specify the correct feature file name pattern, then those files will not be discovered. The same statement is true if you don't have a class in your project that derives from `MissingFeature` base class.

If you want to specify multiple feature file extensions to be discovered, list them separated by pipe. For example, here is how you can specify three extensions ('.feature', '.txt', or '.dat') for your feature file discovery:

```C#
[FeatureFileSearchPattern("*.feature|*.txt|*.dat")]
public sealed class HandleNotImplemented : MissingFeature
{
}
```

Now, as an example, add a new feature file to the solution, name it as 'MathWithInfinity.feature', set its "Copy to Output Directory" property to "Copy if newer", and put the following content inside it:

```Gherkin
Feature: Math with Infinity
	In order to learn Math
	As a regular human
	I want to add two numbers using Calculator

@ignore
Scenario: Add a number to infinity
	Given I chose INFINITY as first number
	And I chose 15 as second number
	When I press add
	Then the result should be INFINITY on the screen

Scenario Outline: Infinity math with examples
	Given I chose <a> as first number
	And I chose <b> as second number
	When I press add
	Then the result should be <sum> on the screen

	@ignore
	Examples:
		| a        | b        | sum      |
		| 0        | INFINITY | INFINITY |
		| INFINITY | INFINITY | INFINITY |
```

Make sure to NOT add the feature class for this new feature file for now, so that it looks like it's not yet implemented.

After you rebuild the test project, you should see a couple of new scenarios from the above feature file in the test explorer. If you run the tests via the test explorer or the command line `dotnet test` command, you will see that those new scenarios are running but skept. That is an expected behavior because in the above example, they are marked with `@ignore` tag. If you prefer to instead see those new scenarios failing after the test run, then remove the `@ignore` tags from them.
