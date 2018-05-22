using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xunit.Gherkin.Quick
{
    internal sealed class Scenario
    {
        public ReadOnlyCollection<StepMethod> Steps { get; }

        public Scenario(IEnumerable<StepMethod> stepMethods)
        {
            Steps = stepMethods != null
                ? new ReadOnlyCollection<StepMethod>(stepMethods.ToList())
                : throw new ArgumentNullException(nameof(stepMethods));
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
