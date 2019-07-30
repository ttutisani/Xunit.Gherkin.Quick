using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gherkin.Ast;

namespace Xunit.Gherkin.Quick
{
    internal sealed class StepMethodInfo
    {
        public ReadOnlyCollection<ScenarioStepPattern> ScenarioStepPatterns { get; }

        private readonly ReadOnlyCollection<StepMethodArgument> _arguments;

        private readonly MethodInfoWrapper _methodInfoWrapper;
        public string GetMethodName()
        {
            return _methodInfoWrapper.GetMethodName();
        }

        private string _lastDigestedStepText;

        private StepMethodInfo(
            IEnumerable<ScenarioStepPattern> scenarioStepPatterns,
            IEnumerable<StepMethodArgument> arguments,
            MethodInfoWrapper methodInfoWrapper)
        {
            ScenarioStepPatterns = scenarioStepPatterns != null
                ? scenarioStepPatterns.ToList().AsReadOnly()
                : throw new ArgumentNullException(nameof(scenarioStepPatterns));

            _arguments = arguments != null
                ? arguments.ToList().AsReadOnly()
                : throw new ArgumentNullException(nameof(arguments));

            _methodInfoWrapper = methodInfoWrapper;
        }

        public string GetDigestedStepText()
        {
            if (_lastDigestedStepText == null)
                throw new InvalidOperationException($"Not yet digested. Call `{nameof(DigestScenarioStepValues)}` first.");

            return _lastDigestedStepText;
        }

        public static StepMethodInfo FromMethodInfo(MethodInfo methodInfo, Feature featureInstance)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));

            var stepDefinitionAttribute = methodInfo.GetCustomAttributes<BaseStepDefinitionAttribute>();

            return new StepMethodInfo(
                ScenarioStepPattern.ListFromStepAttributes(stepDefinitionAttribute),
                StepMethodArgument.ListFromMethodInfo(methodInfo),
                new MethodInfoWrapper(methodInfo, featureInstance));
        }

        public bool IsSameAs(StepMethodInfo other)
        {
            if (other == this)
                return true;

            return other != null
                && other._methodInfoWrapper.IsSameAs(_methodInfoWrapper);
        }
        
        public async Task ExecuteAsync()
        {
            await _methodInfoWrapper.InvokeMethodAsync(_arguments.Select(arg => arg.Value).ToArray());
        }

        public StepMethodInfo Clone()
        {
            var argumentsClone = _arguments.Select(arg => arg.Clone());

            return new StepMethodInfo(ScenarioStepPatterns, argumentsClone, _methodInfoWrapper);
        }
        
        public void DigestScenarioStepValues(Step gherkinScenarioStep)
        {
            if (_arguments.Count == 0)
                return;

            var matchingPattern = FindMatchingPattern(gherkinScenarioStep);
            var gherkinStepText = gherkinScenarioStep.Text.Trim();

            if (matchingPattern == null)
                throw new InvalidOperationException($"This step (`{_methodInfoWrapper.GetMethodName()}`) cannot handle scenario step `{gherkinScenarioStep.Keyword.Trim()} {gherkinStepText}`.");

            var argumentValuesFromStep = Regex.Match(gherkinStepText, matchingPattern.Pattern).Groups.Cast<Group>()
                .Skip(1)
                .Select(g => g.Value)
                .ToArray();
            
            foreach (var arg in _arguments)
            {
                arg.DigestScenarioStepValues(argumentValuesFromStep, gherkinScenarioStep.Argument);
            }

            _lastDigestedStepText = gherkinStepText;

            ScenarioStepPattern FindMatchingPattern(Step gStep)
            {
                var gStepText = gStep.Text.Trim();

                foreach (var pattern in ScenarioStepPatterns)
                {
                    if (!pattern.Kind.ToString().Equals(gStep.Keyword.Trim(), StringComparison.OrdinalIgnoreCase))
                        continue;

                    var match = Regex.Match(gStepText, pattern.Pattern);
                    if (!match.Success || !match.Value.Equals(gStepText))
                        continue;

                    return pattern;
                }

                return null;
            }
        }
        
        public bool Matches(Step gherkinScenarioStep)
        {
            var gherkinStepText = gherkinScenarioStep.Text.Trim();

            foreach (var pattern in ScenarioStepPatterns)
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

    internal enum PatternKind
    {
        Given,
        When,
        Then,
        And,
        But
    }

    internal static class PatternKindExtensions
    {
        public static PatternKind ToPatternKind(BaseStepDefinitionAttribute @this)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this));

            switch (@this)
            {
                case GivenAttribute _:
                    return PatternKind.Given;

                case WhenAttribute _:
                    return PatternKind.When;

                case ThenAttribute _:
                    return PatternKind.Then;

                case AndAttribute _:
                    return PatternKind.And;

                case ButAttribute _:
                    return PatternKind.But;

                default:
                    throw new NotSupportedException($"Cannot convert into step method kind: Attribute type {@this.GetType()} is not supported.");
            }
        }
    }
}
