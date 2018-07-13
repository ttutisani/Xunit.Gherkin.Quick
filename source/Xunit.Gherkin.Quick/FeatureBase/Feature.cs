using System;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick
{
    public abstract class Feature
    {
        internal ITestOutputHelper InternalOutput { get; set; }

        [Scenario]
        internal async Task Scenario(string scenarioName)
        {
            var scenarioExecutor = new ScenarioExecutor(new FeatureFileRepository());
            await scenarioExecutor.ExecuteScenarioAsync(this, scenarioName);
        }

        [ScenarioOutline]
        internal async Task ScenarioOutline(
            string scenarioOutlineName, 
            string exampleName, 
            int exampleIndex)
        {
            var scenarioOutlineExecutor = new ScenarioOutlineExecutor(new FeatureFileRepository());
            await scenarioOutlineExecutor.ExecuteScenarioOutlineAsync(this, scenarioOutlineName, exampleName, exampleIndex);
        }
    }
}
