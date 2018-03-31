namespace Xunit.Gherkin.Quick
{
    public sealed class GivenAttribute : StepDefinitionAttributeBase
    {
        public GivenAttribute(string pattern)
            : base("Given", pattern)
        {

        }
    }
}
