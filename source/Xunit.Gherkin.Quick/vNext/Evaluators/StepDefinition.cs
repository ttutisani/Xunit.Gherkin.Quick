using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit.Gherkin.Quick.vNext.TestScenarios;

namespace Xunit.Gherkin.Quick.vNext.Evaluators
{
    internal class StepDefinition
    {
        public StepDefinition(TestStepType type, Regex pattern, MethodInfo method)
        {
            Type = type;
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            Method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public TestStepType Type { get; }

        public Regex Pattern { get; }

        public MethodInfo Method { get; }
    }
}
