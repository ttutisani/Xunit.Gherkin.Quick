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
    /// Derived classes can also specify the feature text file by using
    /// <see cref="FeatureFileAttribute"/>.
    /// </summary>
    public abstract class Feature : FeatureBase
    {
        [Scenario]
        internal async Task Scenario(string scenarioName, string featureFilePath)
        {
            var scenarioExecutor = new ScenarioExecutor(new FeatureFileRepository("*.feature"));
            await scenarioExecutor.ExecuteScenarioAsync(this, scenarioName, featureFilePath);
        }

        [ScenarioOutline]
        internal async Task ScenarioOutline(
            string scenarioOutlineName, 
            string exampleName, 
            int exampleIndex,
            string featureFilePath)
        {
            var scenarioOutlineExecutor = new ScenarioOutlineExecutor(new FeatureFileRepository("*.feature"));
            await scenarioOutlineExecutor.ExecuteScenarioOutlineAsync(this, scenarioOutlineName, exampleName, exampleIndex, featureFilePath);
        }
    }
}
