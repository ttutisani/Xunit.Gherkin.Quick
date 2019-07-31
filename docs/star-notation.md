# Star Notation

Gherkin language allows you to not specify Given/When/Then/And/But keywords in your scenario stepts. Instead, you can use an asterisk (*) to skip the keyword altogether. Such step will match a step method by the pattern text, without comparing the keyword.

For example, here is how you would implement a very simple feature with star notation.

Feature file:
```Gherkin
Feature: Star-notation feature
  
Scenario: S
	* I have some cukes
```

Feature class:
```C#
public sealed class StarNotation : Feature
{
    [Given("I have some cukes")]
    public void I_Have_Some_Cukes()
    {
        //implement method.
    }
}
``` 
  
----

**A word of warning**: I recommend that you use the star notation as a last resort in your scenario. This is because Given/When/Then/And/But provide more meaningful structure to the text by emphasizing where is precondition, action, and postcondition. Even for the long tests (such as long-running end-to-end tests), I suggest that you attempt to distinguish between these three important phases, and let them repeat throughout the test as necessary (e.g., it's natural that in the long-running tests you want to verify several things so you will have Then several times under different When actions). Besides this conceptual warning, the star notation is normally supported by the Gherkin language and thus by this framework too.
