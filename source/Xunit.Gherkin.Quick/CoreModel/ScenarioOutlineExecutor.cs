using System;
using System.Threading.Tasks;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioOutlineExecutor
    {
        private readonly IFeatureFileRepository _featureFileRepository;

        public ScenarioOutlineExecutor(IFeatureFileRepository featureFileRepository)
        {
            _featureFileRepository = featureFileRepository ?? throw new ArgumentNullException(nameof(featureFileRepository));
        }

        public async Task ExecuteScenarioOutlineAsync(
            Feature featureInstance, 
            string scenarioOutlineName, 
            string exampleName, 
            int exampleRowIndex)
        {
            if (featureInstance == null)
                throw new ArgumentNullException(nameof(featureInstance));

            if (string.IsNullOrWhiteSpace(scenarioOutlineName))
                throw new ArgumentNullException(nameof(scenarioOutlineName));

            if (exampleRowIndex < 0)
                throw new ArgumentException($"`{nameof(exampleRowIndex)}` must be positive", nameof(exampleRowIndex));

            var featureClass = FeatureClass.FromFeatureInstance(featureInstance);
            var featureFile = _featureFileRepository.GetByFilePath(featureClass.FeatureFilePath);

            var gherkinScenarioOutline = featureFile.GetScenarioOutline(scenarioOutlineName);
            if (gherkinScenarioOutline == null)
                throw new InvalidOperationException($"Cannot find scenario outline `{scenarioOutlineName}`.");

            var gherkinScenario = gherkinScenarioOutline.ApplyExampleRow(exampleName, exampleRowIndex);

            var scenario = featureClass.ExtractScenario(gherkinScenario);
            await scenario.ExecuteAsync(new ScenarioOutput(featureInstance.InternalOutput));
        }
    }
}
