namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioExecutor
    {
        private readonly IFeatureFileRepository _featureFileRepository;

        public ScenarioExecutor(IFeatureFileRepository featureFileRepository)
        {
            _featureFileRepository = featureFileRepository ?? throw new System.ArgumentNullException(nameof(featureFileRepository));
        }

        public void ExecuteScenario(Feature featureInstance, string scenarioName)
        {
            if (featureInstance == null)
                throw new System.ArgumentNullException(nameof(featureInstance));

            if (string.IsNullOrWhiteSpace(scenarioName))
                throw new System.ArgumentNullException(nameof(scenarioName));
        }
    }
}
