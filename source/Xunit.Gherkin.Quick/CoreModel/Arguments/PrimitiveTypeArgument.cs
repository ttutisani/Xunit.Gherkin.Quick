using Gherkin.Ast;
using System.Reflection;

namespace Xunit.Gherkin.Quick
{
    internal sealed class PrimitiveTypeArgument : StepMethodArgument
    {
        private readonly ParameterInfo _parameterInfo;

        private readonly int _index;

        public PrimitiveTypeArgument(ParameterInfo parameterInfo, int index)
        {
            _parameterInfo = parameterInfo ?? throw new System.ArgumentNullException(nameof(parameterInfo));
            _index = index;
        }

        public override StepMethodArgument Clone()
        {
            return new PrimitiveTypeArgument(_parameterInfo, _index);
        }

        public override void DigestScenarioStepValues(string[] argumentValues, StepArgument gherkinStepArgument)
        {
            Value = System.Convert.ChangeType(argumentValues[_index], _parameterInfo.ParameterType);
        }

        public override bool IsSameAs(StepMethodArgument other)
        {
            return other is PrimitiveTypeArgument otherPrimitive
                ? otherPrimitive._index == _index
                : false;
        }
    }
}
