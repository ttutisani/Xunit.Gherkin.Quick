namespace Xunit.Gherkin.Quick
{

    public sealed class AndAttribute : StepDefinitionAttributeBase
    {
        public AndAttribute(string pattern)
            : base("And", pattern)
        {
        }
    }
}
