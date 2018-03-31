namespace Xunit.Gherkin.Quick
{

    public sealed class GivenWhenThenAttribute : StepDefinitionAttributeBase
    {
        public GivenWhenThenAttribute (string pattern)
            : base("But", pattern)
        {
        }
    }

}
