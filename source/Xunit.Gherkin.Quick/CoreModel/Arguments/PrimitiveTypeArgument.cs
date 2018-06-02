namespace Xunit.Gherkin.Quick
{
    internal sealed class PrimitiveTypeArgument : StepMethodArgument
    {
        public override StepMethodArgument Clone()
        {
            return new PrimitiveTypeArgument();
        }
    }
}
