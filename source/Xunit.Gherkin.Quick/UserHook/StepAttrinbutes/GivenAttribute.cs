namespace Xunit.Gherkin.Quick
{
    /// <summary>
    /// Annotates method for scenario step which starts with "Given".
    /// </summary>
    public sealed class GivenAttribute : BaseStepDefinitionAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AndAttribute"/> class.
        /// </summary>
        /// <param name="pattern">Regex pattern that must completely match 
        /// a scenario step.</param>
        public GivenAttribute(string pattern)
            : base("Given", pattern)
        {

        }
    }
}
