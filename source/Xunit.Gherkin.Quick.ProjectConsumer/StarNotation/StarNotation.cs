namespace Xunit.Gherkin.Quick.ProjectConsumer.Emoji
{
    [FeatureFile("./StarNotation/StarNotation.em.feature")]
    public class StarNotation : ProjectConsumer.StarNotation.StarNotation { }
}

namespace Xunit.Gherkin.Quick.ProjectConsumer.StarNotation
{
    [FeatureFile("./StarNotation/StarNotation.feature")]
    public class StarNotation : Feature
    {
        [Given("I have some cukes")]
        public void I_Have_Some_Cukes()
        {
            //implement method.
        }
    }
}
