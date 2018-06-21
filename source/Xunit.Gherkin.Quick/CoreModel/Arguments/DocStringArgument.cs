using Gherkin.Ast;
using System;

namespace Xunit.Gherkin.Quick
{
    internal sealed class DocStringArgument : StepMethodArgument
    {
        public override StepMethodArgument Clone()
        {
            return new DocStringArgument();
        }

        public override void DigestScenarioStepValues(string[] argumentValues, StepArgument gherkinStepArgument)
        {
            if (!(gherkinStepArgument is DocString docString))
                throw new InvalidOperationException("DocString cannot be extracted from Gherkin.");

            Value = docString;
        }

        public override bool IsSameAs(StepMethodArgument other)
        {
            return other is DocStringArgument;
        }
    }
}
