# Step Attributes (Given, When, Then, And, But)

You apply step attribute to a method which should handle the step execution as a code.

Purpose of a step attribute is to provide text that matches the step text. Optionally it can extract values out of the step text and pass those values into the parameters of the method.

You can use either .NET regex syntax or cucumber expressions (recommended for simpler matching rules such as primitive types). The two sections below will go into more details.

## Using .NET Regex Syntax

Using [.NET Regex syntax](https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference), your attributes should specify a valid regex pattern that will match the step text and extract the values using regex groups.

For example:
```C#
[Given(@"I chose (\d+) as first number")]
public void I_chose_first_number(int firstNumber)
```

This code will match a step text if it looks like this: `Given I chose 12 as first number`. That's because in Regex, `(\d+)` will match any number of consecutive digits, such as `12`. Additionally, since it's a regex group (recognized by the enclosing parentheses), the matching value (in this case, 12), will end up in the method's argument (`int firstNumber`).

Here are commonly seen use cases that need to be handled carefully, accounting Regex syntax rules:

| Step text in Gherkin language | Attribute in feature class | Argument type | Argument value |
| ----------------------------- | -------------------------- | ------------- | -------------- |
| Given I chose 15 as second number | [Given(@"I chose (\d+) as second number")] | int | 15 |
| Given I chose -15 as second number | [Given(@"I chose (-?\d+) as second number")] | int | -15 (? allows -15 or 15 as input to the test) |
| Given I chose 15 as second number | [Given(@"I chose (\d+) as second number")] | string | "15" |
| Given I chose 15 as second number | [Given(@"I chose (\d) as second number")] | any type | error: will not match 2 digits |
| Given I chose 15.99 as second number | [Given(@"I chose (\d+) as second number")] | any type | error: will not match decimal point |
| Given I chose 15.99 as second number | [Given(@"I chose ([\d\\.]+) as second number")] | decimal | 15.99 |
| Given today is 12/24/2018 | [Given(@"today is (\d{2}/\d{2}/\d{4})")] | DateTime | December 24 2018 |
| Given today is 12/24/2018 | [Given(@"today is (\d{2}/\d{2}/\d{4})")] | string | 12/24/2018 |
| Given my name is "Tengiz" | [Given(@"my name is (.+)")]] | string | "\\"Tengiz\\"" (string starting and ending with quote character) |
| Given my name is "Tengiz" | [Given(@"my name is my name is ""(\w+)""")]] | string | "Tengiz" |
| Given my name is "Tengiz" | [Given(@"my name is (\w+)")]] | any type | error: will not match string with quotes |
| Given Coffee costs $5.00 today | Given(@"Coffee costs \\$([\d\\.]+) today") | decimal | 5.00 |
| Given Coffee costs $5.00 today | Given(@"Coffee costs $([\d\\.]+) today") | any type | error: will not match dollar sign |
| Given Coffee costs $5.00 today | Given(@"Coffee costs ([\d\\.]+) today") | any type | error: will not match dollar sign |
| Given My Brothers' names are Kevin, Lucas, Paul | Given(@"My Brothers' names are ((?:\w+,\s*)+\w+)") | string | Kevin, Lucas, Paul|

## Cucumber Expressions

If you only require simple patterns like integers, words or strings, you can use [cucumber expressions](https://github.com/cucumber/cucumber-expressions#readme), which is a simple alternative for the more complex regex expressions. With cucumber expressions, the above example can be rewritten to:
```C#
[Given(@"I chose {int} as first number")]
public void I_chose_first_number(int firstNumber)
```

Besides simplicity, cucumber expressions have the added benefit of being a formal specification other tools can look for. For instance by providing placeholders for code autocompletion in Visual Studio Code, as implemented by the plugin [VSCucumberAutoComplete](https://github.com/alexkrechik/VSCucumberAutoComplete).

Xunit.Gherkin.Quick supports the following cucumber expressions:

| Expression | Purpose | Corresponding regular expression |
| ---------- | ------- | -------------------------------- |
| {int} | Integer numbers | `[+-]?\d+` |
| {float} | Floating point numbers | `[+-]?([0-9]\*[.])?[0-9]+` |
| {word} | Single word without whitespace | `\w+` |
| {string} | Quoted strings | `("[^"]*")\|('[^']*')` |
| {} | Anything | `.*` |

## Special characters within scenario steps

Some texts will use BDD notation in the form:

```Gherkin
Scenario: Adding two tuples
  Given a1 <- tuple(3, -2, 5, 1)
    And a2 <- tuple(-2, 3, 1, 0)
   Then a1 + a2 = tuple(1, 1, 6, 1)
```

For the most part the *feature* files are plain text and this syntax will be acceptable. But as mentioned here, the C# attributes use regex to provide the parameterization. So the best advice is to avoid using characters considered special in regex notation. Some character conflicts will only reveal themselves fully when Scenario Outlines are created and combined with the use of Examples. 

Consider keeping the notation simple and avoid all special characters e.g.:

```Gherkin
Scenario: Adding two tuples
  Given a1 equals tuple 3 -2 5 1
    And a2 equals tuple -2 3 1 0
   Then a1 plus a2 equals tuple 1 1 6 1
```
