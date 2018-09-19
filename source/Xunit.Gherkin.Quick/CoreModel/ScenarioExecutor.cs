using System;
using System.Linq;
using System.Threading.Tasks;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioExecutor
    {
        private readonly IFeatureFileRepository _featureFileRepository;

        public ScenarioExecutor(IFeatureFileRepository featureFileRepository)
        {
            _featureFileRepository = featureFileRepository ?? throw new ArgumentNullException(nameof(featureFileRepository));
        }

        public async Task ExecuteScenarioAsync(Feature featureInstance, string scenarioName)
        {
            if (featureInstance == null)
                throw new ArgumentNullException(nameof(featureInstance));

            if (string.IsNullOrWhiteSpace(scenarioName))
                throw new ArgumentNullException(nameof(scenarioName));

            var featureClass = FeatureClass.FromFeatureInstance(featureInstance);
            var featureFile = _featureFileRepository.GetByFilePath(featureClass.FeatureFilePath);

			var gherkinScenario = featureFile.GetScenario(scenarioName);
            if (gherkinScenario == null)
                throw new InvalidOperationException($"Cannot find scenario `{scenarioName}`.");

			var gherkinBackground = featureFile.GetBackground();

			var scenario = featureClass.ExtractScenario(gherkinScenario, gherkinBackground);
            await scenario.ExecuteAsync(new ScenarioOutput(featureInstance.InternalOutput));
        }
    }
}
