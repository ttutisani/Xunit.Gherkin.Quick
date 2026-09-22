using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureClass
    {
        private readonly ReadOnlyCollection<StepMethodInfo> _stepMethods;

        private FeatureClass(IEnumerable<StepMethodInfo> stepMethods)
        {
            _stepMethods = stepMethods != null
                ? stepMethods.ToList().AsReadOnly()
                : throw new ArgumentNullException(nameof(stepMethods));
        }

        public static FeatureClass FromFeatureInstance(Feature featureInstance)
        {
            if (featureInstance == null)
                throw new ArgumentNullException(nameof(featureInstance));

            Type featureType = featureInstance.GetType();

            var stepMethods = featureType.GetTypeInfo().GetMethods()
                .Where(m => m.IsDefined(typeof(BaseStepDefinitionAttribute)))
                .Select(m => StepMethodInfo.FromMethodInfo(m, featureInstance))
                .ToList();

            return new FeatureClass(stepMethods);
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

        private global::Gherkin.Ast.Step _TranslateKeyword(global::Gherkin.Ast.Step gherkingScenarioStep, global::Gherkin.GherkinDialect gherkinDialect)
        {
            string translatedKeyword = null;
            if (gherkingScenarioStep.Keyword.Trim() != "*")
                if (gherkinDialect.GivenStepKeywords.Contains(gherkingScenarioStep.Keyword))
                    translatedKeyword = "Given ";
                else if (gherkinDialect.WhenStepKeywords.Contains(gherkingScenarioStep.Keyword))
                    translatedKeyword = "When ";
                else if (gherkinDialect.ThenStepKeywords.Contains(gherkingScenarioStep.Keyword))
                    translatedKeyword = "Then ";
                else if (gherkinDialect.AndStepKeywords.Contains(gherkingScenarioStep.Keyword))
                    translatedKeyword = "And ";
                else if (gherkinDialect.ButStepKeywords.Contains(gherkingScenarioStep.Keyword))
                    translatedKeyword = "But ";

            if (translatedKeyword != null)
                return new global::Gherkin.Ast.Step(gherkingScenarioStep.Location, translatedKeyword, gherkingScenarioStep.Text, gherkingScenarioStep.Argument);
            else
                return gherkingScenarioStep;
        }
    }
}
