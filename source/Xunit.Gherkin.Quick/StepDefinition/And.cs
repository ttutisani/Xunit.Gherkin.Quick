namespace Xunit.Gherkin.Quick
{

    public sealed class And : StepDefinitionAttributeBase
    {
        public And(string pattern)
            : base("And", pattern)
        {
        }
    }
}
