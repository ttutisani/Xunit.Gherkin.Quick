Nuget: [![Xunit.Gherkin.Quick on Nuget](https://img.shields.io/nuget/v/Xunit.Gherkin.Quick.svg?style=flat-square)](https://www.nuget.org/packages/Xunit.Gherkin.Quick/) [![Xunit.Gherkin.Quick Downloads on Nuget](https://img.shields.io/nuget/dt/Xunit.Gherkin.Quick.svg)](https://www.nuget.org/packages/Xunit.Gherkin.Quick/)

Build Status: [![Build status](https://ci.appveyor.com/api/projects/status/d8twk1y4k55s2f53/branch/master?svg=true)](https://ci.appveyor.com/project/ttutisani/xunit-gherkin-quick/branch/master)

------

# Xunit.Gherkin.Quick
Xunit.Gherkin.Quick is a lightweight, cross platform BDD test framework (targets .NET Standard, can be used from both .NET and .NET Core test projects). It parses Gherkin language and executes Xunit tests corresponding to scenarios.

Key differences of Xunit.Gherkin.Quick from other BDD frameworks are:

- You write your expectations in a human-readable Gherkin language, not in code. This approach allows separation of concerns and responsibilities between requirement and code writing.
- Xunit.Gherkin.Quick is a lightweight framework. No auto-generated code that you fear to change; no Visual Studio dependency where the auto-generation works. With Xunit.Gherkin.Quick, you don't even need an IDE (althought it works conveniently in Visual Studio) - one could write feature text files in a notepad, and code in any dev plugin; then run tests via .NET Core CLI.
- Supports full spectrum of Gherkin language idioms such as Given/When/Then/And/But/* keywords, Scenario Outlines, Scenario Backgrounds, and native Gherkin argument types (DocString, DataTable).
- All supported features are fully documented. Scroll down to the Documentation and Reference section after going through the Getting Started topic on this page.

Enjoy coding!

**Table of Contents**
- [Xunit.Gherkin.Quick](#xunitgherkinquick)
  - [Project Sponsors](#project-sponsors)
  - [Getting Started](#getting-started)
      - [Xunit Test Project](#xunit-test-project)
      - [Install Nuget Package](#install-nuget-package)
      - [Create Gherkin Feature File](#create-gherkin-feature-file)
      - [Implement Feature Scenario](#implement-feature-scenario)
      - [Run Scenario](#run-scenario)
      - [Add More Scenarios](#add-more-scenarios)
  - [Got Stuck?](#got-stuck)
  - [Special Thanks](#special-thanks)
  - [Documentation and Reference](#documentation-and-reference)
  - [See Also](#see-also)

## Project Sponsors

**Why sponsor?** Some organizations might want to have premium support while using the framework; others might want to prioritize implementing certain new capabilities (if they fit within the framework's vision and roadmap). If you wish to be a sponsor, please find the sponsoring link on this page. Feel free to contact me for more details.

```
This section will showcase the sponsors' logos, if they wish to do so.
```

## Getting Started

*Prefer video format?* Watch how to get started with [BDD and Xunit.Gherkin.Quick on Youtube](https://youtu.be/RBcJYt2g_gE).

We'll quickly setup our project, write our first BDD test, and then run it.

#### Xunit Test Project

Create a new or open existing Xunit test project. `Xunit.Gherkin.Quick` needs to be used with Xunit.

#### Install Nuget Package

Package name to search for through GUI: `Xunit.Gherkin.Quick`

Package Manager:
```powershell
Install-Package Xunit.Gherkin.Quick
```

.NET Core:
```powershell
dotnet add package Xunit.Gherkin.Quick
```

These steps should take care of the installation, but if you need more info about setup or the nuget package, click here: https://www.nuget.org/packages/Xunit.Gherkin.Quick/

#### Create Gherkin Feature File

Create a new text file. Name it as `AddTwoNumbers.feature`.

**Important**: change feature file properties to ensure it gets copied into output directory. Set the value of `Copy to Output Directory` to *Copy Always* or *Copy if Newer*. See [Copying Feature Files](/docs/copying-feature-files.md) for more options.

_NOTE: In practice, you can name your files in any way you want, and .feature extension is not necessary either._

Copy the below code and paste it into your feature file:
```Gherkin
Feature: AddTwoNumbers
	In order to learn Math
	As a regular human
	I want to add two numbers using Calculator

Scenario: Add two numbers
	Given I chose 12 as first number
	And I chose 15 as second number
	When I press add
	Then the result should be 27 on the screen
```

This is a BDD style feature written in [Gherkin language](https://docs.cucumber.io/gherkin/reference/).

Now it's time to implement the code to run scenarios of this feature.

**Note**: at this point, the new feature file's scenarios will not be discovered by the test runner either via Visual Studio or via the command line execution. By default, you need to have a corresponding feature class in your project which refers to this new feature file. If you want to instead see every new feature file's scenarios right after they are added without the necessity to have the corresponding feature class, please see [Handling Not-Implemented Feature Files](/docs/handle-not-implemented-feature-files.md).

#### Implement Feature Scenario

Implementing a scenario simply means writing methods in the `Feature`-derived class. Goal is to ensure that each scenario step above will match a method by using regex syntax. If we miss a step and it does not match a method, we will receive an error when we try to run the scenario test.

Here is how we can implement the scenario of the above feature:

```C#
[FeatureFile("./Addition/AddTwoNumbers.feature")]
public sealed class AddTwoNumbers : Feature
{
    private readonly Calculator _calculator = new Calculator();

    [Given(@"I chose (\d+) as first number")]
    public void I_chose_first_number(int firstNumber)
    {
        _calculator.SetFirstNumber(firstNumber);
    }

    [And(@"I chose (\d+) as second number")]
    public void I_chose_second_number(int secondNumber)
    {
        _calculator.SetSecondNumber(secondNumber);
    }

    [When(@"I press add")]
    public void I_press_add()
    {
        _calculator.AddNumbers();
    }

    [Then(@"the result should be (\d+) on the screen")]
    public void The_result_should_be_z_on_the_screen(int expectedResult)
    {
        var actualResult = _calculator.Result;

        Assert.Equal(expectedResult, actualResult);
    }
}
```

Notice a couple of things:

- `FeatureFile` attribute for the class refers to the feature file location (**relative to the project root, not relative to the class file**). You don't need to apply this attribute if you keep your feature files in the root directory of your project, because that's where it will be located by default. Buf if you keep it under a sub-folder (which I do), then make sure to specify the file location (either relative or absolute) using this attribute. For more information, refer to the [`FeatureFile` attribute docs](/docs/feature-file-attribute.md).

- `Given`, `When` and `Then` attributes specify scenario step text which they need to match. If you want to extract value from the text, you need to use parentheses. Behind the scenes this is done using [.NET Regex syntax](https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference). Regex group values are passed as argument values. You can also use `And` and `But` attributes, which work similarly.

- Scenario step method can be `async`, or can be a regular method, just as shown in the example above.

#### Run Scenario

Build BDD tests project.

If you use command line to run unit tests, simply run them as always (e.g., run `dotnet test` command inside a solution or test project folder). You should see the scenario full name in the results as a newly added unit test name.

If you use Visual Studio to run unit tests, open Test Explorer to see the new test item (**important**: use built-in Test Explorer, not ReSharper or anything else). Right click and run it as you would usually run unit tests through test explorer.

Unit test name in this case will be "AddTwoNumbers :: Add two numbers", which is a combination of feature name "AddTwoNumbers" and scenario name "Add two numbers".

![Screenshot of scenario test run](scenario-test-run-screenshot.png)

#### Add More Scenarios

If the feature has multiple scenarios, add them to the same feature file. They will show up as additional tests in the test explorer. And they will need additional methods in the same feature class for execution.

## Got Stuck?

Look into our [issues](https://github.com/ttutisani/Xunit.Gherkin.Quick/issues) if your problem was already resolved. Also try [searching StackOverflow](https://stackoverflow.com/search?q=xunit.gherkin.quick).

Feel free to post your question/problem either into our [issues repository](https://github.com/ttutisani/Xunit.Gherkin.Quick/issues), or into [StackOverflow](https://stackoverflow.com).

Check out a fully-working sample project with BDD test implementations for every supported feature: [ProjectConsumer](/source/Xunit.Gherkin.Quick.ProjectConsumer).

## Special Thanks

I want to send special Thank You to all the contributors, which you can see here: https://github.com/ttutisani/Xunit.Gherkin.Quick/graphs/contributors

## Documentation and Reference

- [`FeatureFile` attribute](/docs/feature-file-attribute.md)
- [Handling Not-Implemented Feature Files](/docs/handle-not-implemented-feature-files.md)
- [Step Attributes (Given, When, Then, And, But) usage instructions](/docs/step-attributes.md)
- [DataTable Argument usage instructions](/docs/datatable-argument.md)
- [DocString (multi-line text) Argument usage instructions](/docs/docstring-argument.md)
- [Gherking Tags usage instructions](/docs/tags.md)
- [Scenario Outline usage instructions](/docs/scenario-outline.md)
- [Scenario Background usage instructions](/docs/scenario-background.md)
- [Logging Output from Scenario Steps to Standard Test Output](/docs/log-test-output.md)
- [Ignoring Scenario, Scenario Outline, Examples, or Feature](/docs/ignore-scenario.md)
- [Star Notation Support - How to Skip Keywords in Scenario Steps](/docs/star-notation.md)
- [Shared Step Method With Multiple Step Attributes](/docs/shared-step-method.md)
- [Reusing step implementation across features](/docs/reuse-step-implementation-across-features.md)
- [Before and After Scenario Execution Hooks](/docs/before-after-scenario-hooks.md)
- [Syntax highlighting of Gherkin/Cucumber](/docs/gherkin-syntax-highlighting.md)
- [Integrating Test Results With PicklesDoc](/docs/picklesdoc-test-results.md)
- [Copying feature files](/docs/copying-feature-files.md)
- [Feature file character encoding](/docs/encoding-feature-files.md)
- [Domain Model (for contributors)](/contribution/domain-model.md)

## See Also

- [Versions](/versions)
- [Who uses Xunit.Gherkin.Quick and why?](https://github.com/ttutisani/Xunit.Gherkin.Quick/issues/75)
- [Xunit.Gherkin.Quick on StackOverflow](https://stackoverflow.com/search?q=xunit.gherkin.quick)
