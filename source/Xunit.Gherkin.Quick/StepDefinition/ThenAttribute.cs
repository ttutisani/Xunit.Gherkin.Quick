namespace Xunit.Gherkin.Quick
{

    public sealed class ThenAttribute : StepDefinitionAttributeBase
    {
        public ThenAttribute (string pattern)
            : base("Then", pattern)
        {

        }
    }
}
