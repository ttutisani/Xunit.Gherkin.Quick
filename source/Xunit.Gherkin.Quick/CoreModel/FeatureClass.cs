using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureClass
    {
        public string FeatureFilePath { get; }

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

            var featureFileAttribute = featureType
                .GetTypeInfo()
                .GetCustomAttribute<FeatureFileAttribute>();
            var featureFilePath = featureFileAttribute?.Path ?? $"{featureType.Name}.feature";

            var stepMethods = featureType.GetTypeInfo().GetMethods()
                .Where(m => m.IsDefined(typeof(BaseStepDefinitionAttribute)))
                .Select(m => StepMethodInfo.FromMethodInfo(m, featureInstance))
                .ToList();

            return new FeatureClass(featureFilePath, stepMethods);
        }

		public Scenario ExtractScenario(global::Gherkin.Ast.Scenario scenario, global::Gherkin.Ast.Background background)
		{
			if (scenario == null)
				throw new ArgumentNullException(nameof(scenario));

			var steps = ExtractSteps(scenario);
			if (background != null)
				steps = ExtractSteps(background).Concat(steps).ToList();

			return new Scenario(steps);
		}

		private List<StepMethod> ExtractSteps(global::Gherkin.Ast.ScenarioDefinition gherkinScenario)
        {
            if (gherkinScenario == null)
                throw new ArgumentNullException(nameof(gherkinScenario));

			return gherkinScenario.Steps
				.Select(gherkingScenarioStep =>
				{
					var matchingStepMethodInfo = _stepMethods.FirstOrDefault(stepMethodInfo => IsStepMethodInfoAMatch(gherkingScenarioStep, stepMethodInfo));
					if (matchingStepMethodInfo == null)
						throw new InvalidOperationException($"Cannot match any method with step `{gherkingScenarioStep.Keyword.Trim()} {gherkingScenarioStep.Text.Trim()}`. Scenario `{gherkinScenario.Name}`.");

					var stepMethod = StepMethod.FromStepMethodInfo(matchingStepMethodInfo, gherkingScenarioStep);
					return stepMethod;
				})
				.ToList();            

            bool IsStepMethodInfoAMatch(global::Gherkin.Ast.Step gherkinScenarioStep, StepMethodInfo stepMethod)
            {
                var gherkinStepText = gherkinScenarioStep.Text.Trim();

                foreach (var pattern in stepMethod.ScenarioStepPatterns)
                {
                    if (!pattern.Kind.ToString().Equals(gherkinScenarioStep.Keyword.Trim(), StringComparison.OrdinalIgnoreCase))
                        continue;

                    var match = Regex.Match(gherkinStepText, pattern.Pattern);
                    if (!match.Success || !match.Value.Equals(gherkinStepText))
                        continue;

                    return true;
                }

                return false;
            }
        }
    }
}
