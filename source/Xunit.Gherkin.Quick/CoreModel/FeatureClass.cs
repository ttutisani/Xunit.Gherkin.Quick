using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureClass
    {
        public FeatureClass(string featureFilePath, IEnumerable<StepMethod> stepMethods)
        {
            FeatureFilePath = !string.IsNullOrWhiteSpace(featureFilePath) 
                ? featureFilePath 
                : throw new ArgumentNullException(nameof(featureFilePath));

            StepMethods = stepMethods != null
                ? stepMethods.ToList().AsReadOnly()
                : throw new ArgumentNullException(nameof(stepMethods));
        }

        public string FeatureFilePath { get; }

        public ReadOnlyCollection<StepMethod> StepMethods { get; }

        //TODO: maybe we only need a feature type and not the whole instance?
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
                .Select(m => new { parameters = m.GetParameters(), stepDefinitionAttribute = m.GetCustomAttribute<BaseStepDefinitionAttribute>() })
                .Select(m => new StepMethod(
                    StepMethodKindExtensions.ToStepMethodKind(m.stepDefinitionAttribute), 
                    m.stepDefinitionAttribute.Pattern, 
                    StepMethodArgument.ListFromParameters(m.parameters)))
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
                .Select(s =>
                {
                    var matchingStepMethod = StepMethods.FirstOrDefault(sm => sm.Kind.ToString().Equals(s.Keyword.Trim(), StringComparison.OrdinalIgnoreCase));
                    if (matchingStepMethod == null)
                        throw new InvalidOperationException($"Cannot find scenario step `{s.Keyword}{s.Text}` for scenario `{scenarioName}`.");

                    return matchingStepMethod;
                })
                .ToList();

            return new Scenario(matchingStepMethods);
        }
    }
}
