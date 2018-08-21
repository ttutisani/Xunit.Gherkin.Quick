﻿using System;
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

        public Scenario ExtractScenario(global::Gherkin.Ast.Scenario gherkinScenario)
        {
            if (gherkinScenario == null)
                throw new ArgumentNullException(nameof(gherkinScenario));

            var matchingStepMethods = gherkinScenario.Steps
                .Select(gherkingScenarioStep =>
                {
                    var matchingStepMethod = _stepMethods.FirstOrDefault(stepMethod => IsStepMethodAMatch(gherkingScenarioStep, stepMethod));
                    if (matchingStepMethod == null)
                        throw new InvalidOperationException($"Cannot match any method with step `{gherkingScenarioStep.Keyword.Trim()} {gherkingScenarioStep.Text.Trim()}`. Scenario `{gherkinScenario.Name}`.");

                    var stepMethodClone = matchingStepMethod.Clone();
                    stepMethodClone.DigestScenarioStepValues(gherkingScenarioStep);

                    return StepMethod.FromStepMethodInfo(stepMethodClone, gherkingScenarioStep);
                    //return new StepMethod(stepMethodClone, gherkingScenarioStep.Text);
                })
                .ToList();

            return new Scenario(matchingStepMethods);

            bool IsStepMethodAMatch(global::Gherkin.Ast.Step gherkinScenarioStep, StepMethodInfo stepMethod)
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
