# Step Attributes (Given, When, Then, And, But)

You apply step attribute to a method which should handle the step execution as a code.

Purpose of a step attribute is to provide text that matches the step text. Optionally it can extract values out of the step text and pass those values into the parameters of the method.

All this is done using [.NET Regex syntax](https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference), so make sure that you account for rules governing Regex language if you have complex matching use cases.

For example:
```C#
[Given(@"I chose (\d+) as first number")]
public void I_chose_first_number(int firstNumber)
```

This code will match a step text if it looks like this: `Given I chose 12 as first number`. That's because in Regex, `(\d+)` will match any number of consecutive digits, such as `12`.

Here are commonly seen use cases that need to be handled carefully, accounting Regex syntax rules:

| Step text in Gherkin language | Attribute in feature class | Argument type | Argument value |
| ----------------------------- | -------------------------- | ------------- | -------------- |
| Given I chose 15 as second number | [Given(@"I chose (\d+) as second number")] | int | 15 |
| Given I chose 15 as second number | [Given(@"I chose (\d+) as second number")] | string | "15" |
| Given I chose 15 as second number | [Given(@"I chose (\d) as second number")] | any type | error: will not match 2 digits |
| Given I chose 15.99 as second number | [Given(@"I chose (\d+) as second number")] | any type | error: will not match decimal point |
| Given I chose 15.99 as second number | [Given(@"I chose ([\d\\.]+) as second number")] | decimal | 15.99 |
| Given today is 12/24/2018 | [Given(@"today is (\d{2}/\d{2}/\d{4})")] | DateTime | December 24 2018 |
| Given today is 12/24/2018 | [Given(@"today is (\d{2}/\d{2}/\d{4})")] | string | 12/24/2018 |
| Given my name is "Tengiz" | [Given(@"my name is (.+)")]] | string | `"\"Tengiz\""` (string starting and ending with quote character) |
| Given my name is "Tengiz" | [Given(@"my name is my name is ""(\w+)""")]] | string | "Tengiz" |
| Given my name is "Tengiz" | [Given(@"my name is (\w+)")]] | any type | error: will not match string with quotes |
| Given Coffee costs $5.00 today | Given(@"Coffee costs \\$([\d\\.]+) today") | decimal | 5.00 |
| Given Coffee costs $5.00 today | Given(@"Coffee costs $([\d\\.]+) today") | any type | error: will not match dollar sign |
| Given Coffee costs $5.00 today | Given(@"Coffee costs ([\d\\.]+) today") | any type | error: will not match dollar sign |
