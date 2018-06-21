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
        public string Pattern { get; }

        private readonly ReadOnlyCollection<StepMethodArgument> _arguments;

        public StepMethodKind Kind { get; }

        private readonly MethodInfoWrapper _methodInfoWrapper;

        private string _lastDigestedStepText;

        private StepMethodInfo(
            StepMethodKind kind, 
            string pattern,
            IEnumerable<StepMethodArgument> arguments,
            MethodInfoWrapper methodInfoWrapper)
        {
            Kind = kind;
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
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

            var stepDefinitionAttribute = methodInfo.GetCustomAttribute<BaseStepDefinitionAttribute>();

            return new StepMethodInfo(
                StepMethodKindExtensions.ToStepMethodKind(stepDefinitionAttribute),
                stepDefinitionAttribute.Pattern,
                StepMethodArgument.ListFromMethodInfo(methodInfo),
                new MethodInfoWrapper(methodInfo, featureInstance));
        }

        public bool IsSameAs(StepMethodInfo other)
        {
            if (other == this)
                return true;

            return other != null
                && other.Kind == Kind
                && other.Pattern == Pattern
                && ArgumentsEqual(other._arguments, _arguments)
                && other._methodInfoWrapper.IsSameAs(_methodInfoWrapper);
        }

        private static bool ArgumentsEqual(IList<StepMethodArgument> first, IList<StepMethodArgument> second)
        {
            if (first.Count != second.Count)
                return false;

            for (int index = 0; index < first.Count; index++)
            {
                if (!first[index].IsSameAs(second[index]))
                    return false;
            }

            return true;
        }

        public async Task ExecuteAsync()
        {
            await _methodInfoWrapper.InvokeMethodAsync(_arguments.Select(arg => arg.Value).ToArray());
        }

        

        public StepMethodInfo Clone()
        {
            var argumentsClone = _arguments.Select(arg => arg.Clone());

            return new StepMethodInfo(Kind, Pattern, argumentsClone, _methodInfoWrapper);
        }


        public void DigestScenarioStepValues(Step gherkingScenarioStep)
        {
            if (_arguments.Count == 0)
                return;

            var stepText = gherkingScenarioStep.Text.Trim();

            var argumentValuesFromStep = Regex.Match(stepText, Pattern).Groups.Cast<Group>()
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
