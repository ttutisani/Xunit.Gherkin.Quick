using Gherkin.Ast;

namespace Xunit.Gherkin.Quick
{
    internal sealed class DocStringArgument : StepMethodArgument
    {
        public override StepMethodArgument Clone()
        {
            throw new System.NotImplementedException();
        }

        public override void DigestScenarioStepValues(string[] argumentValues, StepArgument gherkinStepArgument)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsSameAs(StepMethodArgument other)
        {
            throw new System.NotImplementedException();
        }
    }
}
