namespace Xunit.Gherkin.Quick
{

    public sealed class ButAttribute : BaseStepDefinitionAttribute
    {
        public ButAttribute(string pattern)
            : base("But", pattern)
        {
        }
    }
}
