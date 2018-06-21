using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick
{
    public abstract class Feature
    {
        /// <summary>
        /// Allows you to log extra data to the result of the test.
        /// </summary>
        protected internal ITestOutputHelper Output { get; internal set; }

        [Scenario]
        internal async Task Scenario(string scenarioName)
        {
            var scenarioExecutor = new ScenarioExecutor(new FeatureFileRepository());
            await scenarioExecutor.ExecuteScenarioAsync(this, scenarioName);
        }
    }
}
