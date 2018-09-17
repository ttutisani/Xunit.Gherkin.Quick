namespace Xunit.Gherkin.Quick
{

    public sealed class WhenAttribute : BaseStepDefinitionAttribute
    {
        public WhenAttribute(string pattern)
            : base("When", pattern)
        {

        }
    }
}
