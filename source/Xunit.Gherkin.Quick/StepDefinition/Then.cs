namespace Xunit.Gherkin.Quick
{

    public sealed class Then : StepDefinitionAttributeBase
    {
        public Then(string pattern)
            : base("Then", pattern)
        {

        }
    }
}
