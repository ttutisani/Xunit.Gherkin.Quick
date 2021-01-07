[![Build status](https://ci.appveyor.com/api/projects/status/d8twk1y4k55s2f53/branch/master?svg=true)](https://ci.appveyor.com/project/ttutisani/xunit-gherkin-quick/branch/master)

# Xunit.Gherkin.Quick
Xunit.Gherkin.Quick is a lightweight, cross platform BDD test framework (targets .NET Standard, can be used from both .NET and .NET Core test projects). It parses Gherkin language and executes Xunit tests corresponding to scenarios.

## Project Sponsors

Showcase your company's logo here by sponsoring this project!

Do you like Xunit.Gherkin.Quick? Many engineers love it, and it can be even better! Support us by becoming a sponsor, and we will pay back with more dedication, exciting features, and improved support (if you need one). So far, this project runs on a bere enthusiasm, and we do our best to continue development.

If you want to become a sponsor, you can find a sponsoring link on this page, and [contact us](https://www.nuget.org/packages/Xunit.Gherkin.Quick/4.0.0/ContactOwners) to provide your logo and details.

Thank you!

## Getting Started
We'll quickly setup our project, write our first BDD test, and then run it.

#### Xunit test project

Create a new or open existing Xunit test project. `Xunit.Gherkin.Quick` needs to be used with Xunit.

#### Install nuget package

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

#### Create Gherkin feature file

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

- `FeatureFile` attribute for the class refers to the feature file location (**relative to the project root, not relative to the class file**). You don't need to apply this attribute if you keep your feature files in the root directory of your project, because that's where it will be located by default. Buf if you keep it under a sub-folder (which I do), then make sure to specify the file location (either relative or absolute) using this attribute.

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

Also, as recommended by a user of this project, the unit test code can be a useful addendum to the documentation. e.g., see test files and matching feature files in [Xunit.Gherkin.Quick.ProjectConsumer](https://github.com/ttutisani/Xunit.Gherkin.Quick/tree/master/source/Xunit.Gherkin.Quick.ProjectConsumer). [AddNumbersTo5.feature](https://github.com/ttutisani/Xunit.Gherkin.Quick/blob/master/source/Xunit.Gherkin.Quick.ProjectConsumer/Addition/AddNumbersTo5.feature) and [AddNumbersTo5.cs](https://github.com/ttutisani/Xunit.Gherkin.Quick/blob/master/source/Xunit.Gherkin.Quick.ProjectConsumer/Addition/AddNumbersTo5.cs) can be the helpful starting points.

## Special Thanks

I want to send special Thank You to all the contributors, which you can see here: https://github.com/ttutisani/Xunit.Gherkin.Quick/graphs/contributors

Specifically:

- [csurfleet](https://github.com/csurfleet)
- [ChristianPejrup](https://github.com/ChristianPejrup)
- [videege](https://github.com/videege)
- [vinla](https://github.com/vinla)
- [salaerts](https://github.com/salaerts)
- [cathalmchale](https://github.com/cathalmchale)
- [ahmadnazir](https://github.com/ahmadnazir)

## See Also

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
- [Versions](/versions)
- [Who uses Xunit.Gherkin.Quick and why?](https://github.com/ttutisani/Xunit.Gherkin.Quick/issues/75)
- [Xunit.Gherkin.Quick on StackOverflow](https://stackoverflow.com/search?q=xunit.gherkin.quick)
