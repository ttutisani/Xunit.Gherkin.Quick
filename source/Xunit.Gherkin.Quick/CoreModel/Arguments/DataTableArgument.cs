using Gherkin.Ast;
using System;

namespace Xunit.Gherkin.Quick
{
    internal sealed class DataTableArgument : StepMethodArgument
    {
        public override StepMethodArgument Clone()
        {
            return new DataTableArgument();
        }

        public override void DigestScenarioStepValues(string[] argumentValues, StepArgument gherkinStepArgument)
        {
            if (!(gherkinStepArgument is DataTable dataTable))
                throw new InvalidOperationException("DataTable cannot be extracted from Gherkin.");

            Value = dataTable;
        }

        public override bool IsSameAs(StepMethodArgument other)
        {
            return other is DataTableArgument;
        }
    }
}
