using System;

namespace Xunit.Gherkin.Quick.vNext.Evaluators
{
    internal class MatchedStepDefinition : StepDefinition
    {
        public MatchedStepDefinition(StepDefinition stepDefinition, object[] arguments)
            : base(stepDefinition.Type, stepDefinition.Pattern, stepDefinition.Method)
        {
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        public object[] Arguments { get; }
    }
}
