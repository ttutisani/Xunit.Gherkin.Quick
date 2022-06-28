namespace Xunit.Gherkin.Quick.ProjectConsumer.StarNotation
{
    [FeatureFile("./StarNotation/StarNotation.feature")]
    public class StarNotation : Feature
    {
        [FeatureFile("./StarNotation/StarNotation.em.feature")]
        public class Emoji : StarNotation { }

        [Given("I have some cukes")]
        public void I_Have_Some_Cukes()
        {
            //implement method.
        }
    }
}
