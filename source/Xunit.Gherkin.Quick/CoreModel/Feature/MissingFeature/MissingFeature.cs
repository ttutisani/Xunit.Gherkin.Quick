using System;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick
{
    /// <summary>
    /// Base class for feature classes.
    /// Derived classes should define scenario step methods by using
    /// <see cref="GivenAttribute"/>, <see cref="WhenAttribute"/>, 
    /// <see cref="ThenAttribute"/>, <see cref="AndAttribute"/>, 
    /// and <see cref="ButAttribute"/>.
    /// Derived classes should also specify the feature text file by using
    /// <see cref="FeatureFileAttribute"/>.
    /// </summary>
    public abstract class MissingFeature : FeatureBase
    {
        [MissingScenario]
        internal Task Scenario(string scenarioName)
        {
            throw new NotImplementedException($"Scenario `{scenarioName}` is not implemented.");
        }

        [MissingScenarioOutline]
        internal Task ScenarioOutline(
            string scenarioOutlineName,
            string exampleName,
            int exampleIndex)
        {
            throw new NotImplementedException($"Scenario outline `{scenarioOutlineName}`, example `{exampleName}` `#{exampleIndex + 1}` is not implemented.");
        }
    }
}
