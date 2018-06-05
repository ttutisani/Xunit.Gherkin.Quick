using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Gherkin.Ast;

namespace Xunit.Gherkin.Quick
{
    internal sealed class StepMethod
    {
        private StepMethod(
            StepMethodKind kind, 
            string pattern,
            IEnumerable<StepMethodArgument> arguments,
            MethodInfoWrapper methodInfoWrapper)
        {
            Kind = kind;
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            Arguments = arguments != null
                ? arguments.ToList().AsReadOnly()
                : throw new ArgumentNullException(nameof(arguments));

            _methodInfoWrapper = methodInfoWrapper;
        }

        public static StepMethod FromMethodInfo(MethodInfo methodInfo, Feature featureInstance)
        {
            var stepDefinitionAttribute = methodInfo.GetCustomAttribute<BaseStepDefinitionAttribute>();

            return new StepMethod(
                StepMethodKindExtensions.ToStepMethodKind(stepDefinitionAttribute),
                stepDefinitionAttribute.Pattern,
                StepMethodArgument.ListFromParameters(methodInfo.GetParameters()),
                new MethodInfoWrapper(methodInfo, featureInstance));
        }

        public StepMethodKind Kind { get; }

        private readonly MethodInfoWrapper _methodInfoWrapper;

        public bool IsSameAs(StepMethod other)
        {
            if (other == this)
                return true;

            return other != null
                && other.Kind == Kind
                && other.Pattern == Pattern
                && ArgumentsEqual(other.Arguments, Arguments)
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

        public void Execute()
        {
            _methodInfoWrapper.InvokeMethod(Arguments.Select(arg => arg.Value).ToArray());
        }

        public string Pattern { get; }

        public ReadOnlyCollection<StepMethodArgument> Arguments { get; }

        public StepMethod Clone()
        {
            var argumentsClone = Arguments.Select(arg => arg.Clone());

            return new StepMethod(Kind, Pattern, argumentsClone, _methodInfoWrapper);
        }

        public void DigestScenarioStepValues(Step gherkingScenarioStep)
        {
            if (Arguments.Count == 0)
                return;

            var argumentValuesFromStep = Regex.Match(gherkingScenarioStep.Text.Trim(), Pattern).Groups.Cast<Group>()
                .Skip(1)
                .Select(g => g.Value)
                .ToArray();

            if (argumentValuesFromStep.Length != Arguments.Count)
                throw new InvalidOperationException($"Method `{_methodInfoWrapper.GetMethodName()}` for step `{Kind} {gherkingScenarioStep.Text.Trim()}` is expecting {Arguments.Count} params, but {argumentValuesFromStep.Length} param values were supplied.");

            for (int argIndex = 0; argIndex < Arguments.Count; argIndex++)
            {
                Arguments[argIndex].DigestScenarioStepValues(argumentValuesFromStep, gherkingScenarioStep.Argument);
            }
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
