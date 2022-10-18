namespace Xunit.Gherkin.Quick
{
    /// <summary>
    /// Annotates method for scenario step which starts with "Then".
    /// </summary>
    public sealed class ThenAttribute : BaseStepDefinitionAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AndAttribute"/> class.
        /// </summary>
        /// <param name="pattern">Regex pattern that must completely match 
        /// a scenario step.</param>
        public ThenAttribute(string pattern)
            : base("Then", pattern)
        {

        }
    }
}
