namespace Xunit.Gherkin.Quick
{

    public sealed class GivenAttribute : BaseStepDefinitionAttribute
    {
        public GivenAttribute(string pattern)
            : base("Given", pattern)
        {

        }
    }
}
