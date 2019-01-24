namespace Xunit.Gherkin.Quick
{
    /// <summary>
    /// Annotates method for scenario step which starts with "When".
    /// </summary>
    public sealed class WhenAttribute : BaseStepDefinitionAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AndAttribute"/> class.
        /// </summary>
        /// <param name="pattern">Regex pattern that must completely match 
        /// a scenario step.</param>
        public WhenAttribute(string pattern)
            : base("When", pattern)
        {

        }
    }
}
