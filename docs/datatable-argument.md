# DataTable Argument Support

Besides primitive types, your scenario step handing method can have an argument of type `DataTable`. This allows to handle Data Tables defined in the Gherkin feature file.

_`DataTable` is defined under `Gherkin.Ast` namespace, so make sure to add the `using` directive for it. Respective Nuget package is already in your project because `Xunit.Gherkin.Quick` brings it as a dependency._

## Example

Gherkin feature file ("./Addition/AddTwoNumbers.feature"):
```Gherkin
Feature: AddTwoNumbers
	In order to learn Math
	As a regular human
	I want to add two numbers using Calculator

Scenario: Add various pairs of numbers
	Given following table of 4 inputs and outputs:
		| Number 1 | Number 2 | Result |
		| 1        | 1        | 2      |
		| 10       | 20       | 30     |
		| 10       | 11       | 21     |
		| 111      | 222      | 333    |
```

Feature class:
```C#
[FeatureFile("./Addition/AddTwoNumbers.feature")]
public sealed class AddTwoNumbers : Feature
{
    [Given(@"following table of (\d+) inputs and outputs:")]
    public void Following_table_of_inputs_and_outputs(int inputCount, DataTable dataTable)
    {
        Assert.Equal(inputCount, dataTable.Rows.Count() - 1);

        foreach (var row in dataTable.Rows.Skip(1))
        {
            //arrange.
            var calculator = new Calculator();
            calculator.SetFirstNumber(int.Parse(row.Cells.ElementAt(0).Value));
            calculator.SetSecondNumber(int.Parse(row.Cells.ElementAt(1).Value));

            //act.
            calculator.AddNumbers();

            //assert.
            Assert.Equal(int.Parse(row.Cells.ElementAt(2).Value), calculator.Result);
        }
    }
}
```

Notes:
- `DataTable` is a simple class which has rows and cells. Each cell has a value of type `String`. You will need to convert the values into your desired destination types.
- First row of the received `DataTable` instance is a header row (at least in this example). Unless you are interested in the column names (or unless you don't put column names into the first row), you will be skipping this row most of the time (`dataTable.Rows.Skip(1)`).
- `DataTable` argument needs to be placed after those arguments which receive values from regex groups (matching parentherses) in the method attribute. That's why `DataTable` is placed after `int inputCount` argument in the above example - `inputCount` has a matching regex group expression in the `Given` attribute - "(\d+)".

