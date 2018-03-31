namespace Xunit.Gherkin.Quick
{

    public sealed class WhenAttribute : StepDefinitionAttributeBase
    {
        public WhenAttribute(string pattern)
            : base("When", pattern)
        {

        }
    }
}
