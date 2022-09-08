using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureClass
    {
        public string FeatureFilePath { get; }
        public string FileNameSearchPattern { get; }

        private readonly ReadOnlyCollection<StepMethodInfo> _stepMethods;

        private FeatureClass(string featureFilePath, IEnumerable<StepMethodInfo> stepMethods)
        {
            FeatureFilePath = !string.IsNullOrWhiteSpace(featureFilePath) 
                ? featureFilePath 
                : throw new ArgumentNullException(nameof(featureFilePath));

            _stepMethods = stepMethods != null
                ? stepMethods.ToList().AsReadOnly()
                : throw new ArgumentNullException(nameof(stepMethods));
        }

        public static FeatureClass FromFeatureInstance(Feature featureInstance)
        {
            if (featureInstance == null)
                throw new ArgumentNullException(nameof(featureInstance));

            Type featureType = featureInstance.GetType();
            var featureFilePath = FeatureClassInfo.FromFeatureClassType(featureType).FeatureFilePath;

            var stepMethods = featureType.GetTypeInfo().GetMethods()
                .Where(m => m.IsDefined(typeof(BaseStepDefinitionAttribute)))
                .Select(m => StepMethodInfo.FromMethodInfo(m, featureInstance))
                .ToList();

            return new FeatureClass(featureFilePath, stepMethods);
        }

		public Scenario ExtractScenario(global::Gherkin.Ast.Scenario scenario)
		{
			if (scenario == null)
				throw new ArgumentNullException(nameof(scenario));

            var steps = ExtractSteps(scenario);
			return new Scenario(steps);
		}

		private List<StepMethod> ExtractSteps(global::Gherkin.Ast.ScenarioDefinition gherkinScenario)
        {
            if (gherkinScenario == null)
                throw new ArgumentNullException(nameof(gherkinScenario));

			return gherkinScenario.Steps
				.Select(gherkingScenarioStep =>
				{
					var matchingStepMethodInfo = _stepMethods.FirstOrDefault(stepMethodInfo => stepMethodInfo.Matches(gherkingScenarioStep));
					if (matchingStepMethodInfo == null)
						throw new InvalidOperationException($"Cannot match any method with step `{gherkingScenarioStep.Keyword.Trim()} {gherkingScenarioStep.Text.Trim()}`. Scenario `{gherkinScenario.Name}`.");

					var stepMethod = StepMethod.FromStepMethodInfo(matchingStepMethodInfo, gherkingScenarioStep);
					return stepMethod;
				})
				.ToList();
        }
    }
}
