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
        private readonly ReadOnlyCollection<ScenarioStepPattern> _scenarioStepPatterns;

        private readonly ReadOnlyCollection<StepMethodArgument> _arguments;

        private readonly MethodInfoWrapper _methodInfoWrapper;

        private string _lastDigestedStepText;

        private StepMethodInfo(
            IEnumerable<ScenarioStepPattern> scenarioStepPatterns,
            IEnumerable<StepMethodArgument> arguments,
            MethodInfoWrapper methodInfoWrapper)
        {
            _scenarioStepPatterns = scenarioStepPatterns != null
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

            return new StepMethodInfo(_scenarioStepPatterns, argumentsClone, _methodInfoWrapper);
        }

        //TODO: move this method onto StepMethod - because that's only when digest makes sense.
        //StepMethodInfo has multiple patterns, so digest is ambiguous here.
        public void DigestScenarioStepValues(Step gherkingScenarioStep)
        {
            if (_arguments.Count == 0)
                return;

            var stepText = gherkingScenarioStep.Text.Trim();

            var argumentValuesFromStep = Regex.Match(stepText, "" /*Pattern*/).Groups.Cast<Group>()
                .Skip(1)
                .Select(g => g.Value)
                .ToArray();
            
            foreach (var arg in _arguments)
            {
                arg.DigestScenarioStepValues(argumentValuesFromStep, gherkingScenarioStep.Argument);
            }

            _lastDigestedStepText = stepText;
        }
    }

    internal enum StepMethodKind
    {
        Given,
        When,
        Then,
        And,
        But
    }

    internal static class StepMethodKindExtensions
    {
        public static StepMethodKind ToStepMethodKind(BaseStepDefinitionAttribute @this)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this));

            switch (@this)
            {
                case GivenAttribute _:
                    return StepMethodKind.Given;

                case WhenAttribute _:
                    return StepMethodKind.When;

                case ThenAttribute _:
                    return StepMethodKind.Then;

                case AndAttribute _:
                    return StepMethodKind.And;

                case ButAttribute _:
                    return StepMethodKind.But;

                default:
                    throw new NotSupportedException($"Cannot convert into step method kind: Attribute type {@this.GetType()} is not supported.");
            }
        }
    }
}
