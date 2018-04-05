# Using table data in your tests

Table data can be added to any of the steps in a gherkin specification, e.g:

```Scenario: Find user
	Given there are 2 users:
	   | username | email               |
	   | everzet  | everzet@knplabs.com |
	   | fabpot   | fabpot@symfony.com  |
	When I search for 'everzet'
	Then the result should have an email of 'everzet@knplabs.com'```

In order to use this data in your tests, simply add a DataTable parameter to the matching method:

```[Given(@"there are (\d+) users:")]
    public void There_are_users(int count, DataTable users)
    {
	   // Use the data
    }```

_NOTE: You can add your DataTable anywhere in the method definition you like, although for readability we recommend either first or last._
