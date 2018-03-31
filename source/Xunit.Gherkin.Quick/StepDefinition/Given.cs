namespace Xunit.Gherkin.Quick
{
    public sealed class Given : StepDefinitionAttributeBase
    {
        public Given(string pattern)
            : base("Given", pattern)
        {

        }
    }
}
