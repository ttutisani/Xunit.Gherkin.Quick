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
| Given my name is "Tengiz" | [Given(@"my name is (.+)")]] | string | "\\"Tengiz\\"" (string starting and ending with quote character) |
| Given my name is "Tengiz" | [Given(@"my name is my name is ""(\w+)""")]] | string | "Tengiz" |
| Given my name is "Tengiz" | [Given(@"my name is (\w+)")]] | any type | error: will not match string with quotes |
| Given Coffee costs $5.00 today | Given(@"Coffee costs \\$([\d\\.]+) today") | decimal | 5.00 |
| Given Coffee costs $5.00 today | Given(@"Coffee costs $([\d\\.]+) today") | any type | error: will not match dollar sign |
| Given Coffee costs $5.00 today | Given(@"Coffee costs ([\d\\.]+) today") | any type | error: will not match dollar sign |

## BDD Notation

Some texts will use BDD notation in the form:

```
Scenario: Adding two tuples
  Given a1 <- tuple(3, -2, 5, 1)
    And a2 <- tuple(-2, 3, 1, 0)
   Then a1 + a2 = tuple(1, 1, 6, 1)
```

For the most part the *feature* files are plain text and this syntax will be acceptable. But as mentioned here, the C# attributes use regex to provide the parameterization. So the best advice is to avoid using characters considered special in regex notation. Some character conflicts will only reveal themselves fully when the Scenario is combined with the use of Examples. 

Consider keeping the notation simple and avoid all special characters e.g.:

```
Scenario: Adding two tuples
  Given a1 equals tuple 3 -2 5 1
    And a2 equals tuple -2 3 1 0
   Then a1 plus a2 equals tuple 1 1 6 1
```

## Feature File character encoding

An [issue](https://github.com/ttutisani/Xunit.Gherkin.Quick/issues/82) was logged claiming that feature files and corresponding tests would not be *discovered* if the feature file contained hidden unicode characters.

This proved to be a time consuming problem for the user. The root of the problem was a consequence of copy-pasting from a PDF directly into a new feature file. All other feature files and tests would run, but the new tests were non-discoverable. Even when the erroneous feature file had been stripped right back to a simple *Hello World* test, the problem persisted.

The advice is to avoid copy-pasting from PDFs, Word docs etc. And, if having similar problems, copy a known good feature file, get it running, then apply new Scenarios one at a time.
