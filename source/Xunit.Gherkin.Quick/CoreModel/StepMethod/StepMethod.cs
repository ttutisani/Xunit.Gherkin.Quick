using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gherkin.Ast;

namespace Xunit.Gherkin.Quick
{
    internal sealed class StepMethod
    {
        private readonly StepMethodInfo _stepMethodInfo;

        public string StepText { get; }

        public GherkinDialect.KeywordFor Kind { get; }

        public string Pattern { get; }

        private StepMethod(StepMethodInfo stepMethodInfo, GherkinDialect.KeywordFor kind, string pattern, string stepText)
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
            var matchingPattern = stepMethodInfo.GetMatchingPattern(gherkinScenarioStep);

            if (matchingPattern == null)
                throw new InvalidOperationException($"This step method info (`{stepMethodInfo.GetMethodName()}`) cannot handle given scenario step: `{gherkinScenarioStep.Keyword.Trim()} {gherkinScenarioStep.Text.Trim()}`.");

            var stepMethodInfoClone = stepMethodInfo.Clone();
            stepMethodInfoClone.DigestScenarioStepValues(gherkinScenarioStep);
            return new StepMethod(stepMethodInfoClone, matchingPattern.Kind, matchingPattern.OriginalPattern, gherkinScenarioStep.Text);
        }

        public async Task ExecuteAsync()
        {
            await _stepMethodInfo.ExecuteAsync();
        }
    }
}
