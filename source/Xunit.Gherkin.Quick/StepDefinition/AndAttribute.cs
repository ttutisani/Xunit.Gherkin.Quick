namespace Xunit.Gherkin.Quick
{

    public sealed class AndAttribute : BaseStepDefinitionAttribute
    {
        public AndAttribute(string pattern)
            : base("And", pattern)
        {
        }
    }
}
