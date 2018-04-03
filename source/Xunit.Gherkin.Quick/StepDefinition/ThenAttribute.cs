namespace Xunit.Gherkin.Quick
{

    public sealed class ThenAttribute : BaseStepDefinitionAttribute
    {
        public ThenAttribute(string pattern)
            : base("Then", pattern)
        {

        }
    }
}
