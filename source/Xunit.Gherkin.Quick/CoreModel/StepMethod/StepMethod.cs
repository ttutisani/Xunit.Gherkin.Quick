using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Xunit.Gherkin.Quick
{
    internal sealed class StepMethod
    {
        private StepMethod(
            StepMethodKind kind, 
            string text,
            IEnumerable<StepMethodArgument> arguments,
            MethodInfoWrapper methodInfoWrapper)
        {
            Kind = kind;
            Text = text ?? throw new ArgumentNullException(nameof(text));
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

        public void Execute()
        {
            _methodInfoWrapper.InvokeMethod(Arguments.Select(arg => arg.Value).ToArray());
        }

        public string Text { get; }

        public ReadOnlyCollection<StepMethodArgument> Arguments { get; }
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
