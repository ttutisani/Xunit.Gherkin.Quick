# FeatureFileAttribute

Apply FeatureFileAttribute attribute to your feature class to link the Gherkin feature located inside the file with the class. You can specify either a single file path or a pattern. let's look at both options below.

## Option 1 - Specify s Single Feature File Path

This approach is straightforward - simply specify where to find the feature file. When the framework discovers the specified file, it will try to match each scenario step inside the file with the step method inside the class.

Example - applying `FeatureFileAttribute` to a feature class.

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

Explanation: the 'AddTwoNumbers.feature' file is located under an 'Addition' subfolder of the output folder.

## Option 2 - Specify a Path Pattern to Feature File(s)

In some cases, you will need to map multiple feature files to a single feature class. This approach will allow handling all features using a single class for code reuse and to avoid duplication. In other cases, you may not know in advance all the feature file names that will need to map to a single feature class, but you may know a pattern their names will follow. In such scenarios, you can specify a path pattern using the `FeatureFileAttribute`.

```C#
[FeatureFile(@".*Pattern/(Nested.*/)?.*\.feature", FeatureFilePathType.Regex)]
public sealed class BaseAndNestedFolders : Feature
{
    [Given("I have a base folder")]
    public void Given_I_have_base_folder()
    {

    }

    [Given("I have a nested folder")]
    public void Given_I_have_nested_folder()
    {

    }
}
```

Explanation: if you follow the Regex rules, you will notice that the mapped feature files are located under a folder that ends with 'Pattern', or under its nested folder that starts with 'Nested'. The file must have a '.feature' extension. All files that match this pattern will be mappted back to the shown feature class.

In this example, the class has two step methods, but not all of them need to be mapped to the found feature files. But the opposite direction works differently - all scenario steps found in all feature files must be mapped to at least one step method in the feature class.

#### Important: For non-'.feature' Extensions

The above path pattern example works out of the box if your feature files have '.feature' extension. If their extension is different, you need to take an extra step. This is so because the framework, by default, only looks for the '.feature' files. You need to change this default behavior to map other extensions as well. To do this, whithin your test assembly, you need to apply the `FeatureFileSearchPatternAttribute`. Note that you need to apply this attribute to an assembly, not to a class.

Here is an example that specifies feature files having extensions of '.feature', '.txt', or '.dat':

```C#
[assembly:Xunit.Gherkin.Quick.FeatureFileSearchPattern("*.feature|*.txt|*.dat")]
```

Side note: you may notice that this attribute has two purposes - one is when you apply it to the assembly as shown above, and another is to indicate where to find the [unhandled (not implemented) feature files](/docs/handle-not-implemented-feature-files.md).
