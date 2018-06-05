using Gherkin.Ast;

namespace Xunit.Gherkin.Quick
{
    internal sealed class PrimitiveTypeArgument : StepMethodArgument
    {
        public int Index { get; }

        public PrimitiveTypeArgument(int index)
        {
            Index = index;
        }

        public override StepMethodArgument Clone()
        {
            return new PrimitiveTypeArgument(Index);
        }

        public override void DigestScenarioStepValues(string[] argumentValues, StepArgument gherkinStepArgument)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsSameAs(StepMethodArgument other)
        {
            return other is PrimitiveTypeArgument otherPrimitive
                ? otherPrimitive.Index == Index
                : false;
        }
    }
}
