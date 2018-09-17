using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gherkin.Ast;

namespace Xunit.Gherkin.Quick
{
    internal sealed class StepMethod
    {
        private readonly StepMethodInfo _stepMethodInfo;

        public string StepText { get; }

        public PatternKind Kind { get; }

        public string Pattern { get; }

        private StepMethod(StepMethodInfo stepMethodInfo, PatternKind kind, string pattern, string stepText)
        {
            _stepMethodInfo = stepMethodInfo ?? throw new ArgumentNullException(nameof(stepMethodInfo));
            Kind = kind;
            Pattern = !string.IsNullOrWhiteSpace(pattern) ? pattern : throw new ArgumentNullException(nameof(pattern));

            StepText = !string.IsNullOrWhiteSpace(stepText)
                ? stepText
                : throw new ArgumentNullException(nameof(stepText));

        }

        public static StepMethod FromStepMethodInfo(StepMethodInfo stepMethodInfo, Step gherkinScenarioStep)
        {
            var matchingPattern = GetMatchingPattern(stepMethodInfo, gherkinScenarioStep);

            if (matchingPattern == null)
                throw new InvalidOperationException($"This step method info (`{stepMethodInfo.GetMethodName()}`) cannot handle given scenario step: `{gherkinScenarioStep.Keyword.Trim()} {gherkinScenarioStep.Text.Trim()}`.");

            var stepMethodInfoClone = stepMethodInfo.Clone();
            stepMethodInfoClone.DigestScenarioStepValues(gherkinScenarioStep);
            return new StepMethod(stepMethodInfoClone, matchingPattern.Kind, matchingPattern.Pattern, gherkinScenarioStep.Text);
        }

        private static ScenarioStepPattern GetMatchingPattern(StepMethodInfo stepMethod, Step gherkinScenarioStep)
        {
            var gherkinStepText = gherkinScenarioStep.Text.Trim();

            foreach (var pattern in stepMethod.ScenarioStepPatterns)
            {
                if (!pattern.Kind.ToString().Equals(gherkinScenarioStep.Keyword.Trim(), StringComparison.OrdinalIgnoreCase))
                    continue;

                var match = Regex.Match(gherkinStepText, pattern.Pattern);
                if (!match.Success || !match.Value.Equals(gherkinStepText))
                    continue;

                return pattern;
            }

            return null;
        }

        public async Task ExecuteAsync(Dictionary<string, object> scenarioContext)
        {
            await _stepMethodInfo.ExecuteAsync(scenarioContext);
        }
    }
}
