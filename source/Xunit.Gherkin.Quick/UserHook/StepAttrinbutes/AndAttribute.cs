namespace Xunit.Gherkin.Quick
{
    /// <summary>
    /// Annotates method for scenario step which starts with "And".
    /// </summary>
    public sealed class AndAttribute : BaseStepDefinitionAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AndAttribute"/> class.
        /// </summary>
        /// <param name="pattern">Regex pattern that must completely match 
        /// a scenario step.</param>
        public AndAttribute(string pattern)
            : base("And", pattern)
        {
        }
    }
}
