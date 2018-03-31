namespace Xunit.Gherkin.Quick
{

    public sealed class When : StepDefinitionAttributeBase
    {
        public When(string pattern)
            : base("When", pattern)
        {

        }
    }
}
