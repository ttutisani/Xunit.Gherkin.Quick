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

        public Scenario ExtractScenario(string scenarioName, FeatureFile featureFile)
        {
            if (string.IsNullOrWhiteSpace(scenarioName))
                throw new ArgumentNullException(nameof(scenarioName));

            if (featureFile == null)
                throw new ArgumentNullException(nameof(featureFile));

            var gherkinScenario = featureFile.GherkinDocument.Feature.Children.FirstOrDefault(s => s.Name == scenarioName);
            if (gherkinScenario == null)
                throw new InvalidOperationException($"Cannot find scenario `{scenarioName}`.");

            var matchingStepMethods = gherkinScenario.Steps
                .Select(gherkingScenarioStep =>
                {
                    var matchingStepMethod = _stepMethods.FirstOrDefault(stepMethod => IsStepMethodAMatch(gherkingScenarioStep, stepMethod));
                    if (matchingStepMethod == null)
                        throw new InvalidOperationException($"Cannot match any method with step `{gherkingScenarioStep.Keyword}{gherkingScenarioStep.Text}`. Scenario `{scenarioName}`.");

                    var stepMethodClone = matchingStepMethod.Clone();
                    stepMethodClone.DigestScenarioStepValues(gherkingScenarioStep);

                    return new StepMethod(stepMethodClone, gherkingScenarioStep.Text);
                })
                .ToList();

            return new Scenario(matchingStepMethods);

            bool IsStepMethodAMatch(global::Gherkin.Ast.Step gherkinScenarioStep, StepMethodInfo stepMethod)
            {
                if (!stepMethod.Kind.ToString().Equals(gherkinScenarioStep.Keyword.Trim(), StringComparison.OrdinalIgnoreCase))
                    return false;

                var gherkinStepText = gherkinScenarioStep.Text.Trim();

                var match = Regex.Match(gherkinStepText, stepMethod.Pattern);
                if (!match.Success || !match.Value.Equals(gherkinStepText))
                    return false;

                return true;
            }
        }
    }
}
