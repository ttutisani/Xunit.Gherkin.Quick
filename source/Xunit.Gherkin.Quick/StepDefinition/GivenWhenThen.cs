namespace Xunit.Gherkin.Quick
{

    public sealed class GivenWhenThen : StepDefinitionAttributeBase
    {
        public GivenWhenThen(string pattern)
            : base("But", pattern)
        {
        }
    }

}
