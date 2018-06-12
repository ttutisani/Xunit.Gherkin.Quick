using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task ExecuteAsync(IScenarioOutput scenarioOutput)
        {
            if (scenarioOutput == null)
                throw new ArgumentNullException(nameof(scenarioOutput));

            var step = Steps.GetEnumerator();
            while(step.MoveNext())
            {
                try
                {
                    await step.Current.ExecuteAsync();
                    scenarioOutput.StepPassed($"{step.Current.StepMethodInfo.Kind} {step.Current.StepText}");
                }
                catch
                {
                    scenarioOutput.StepFailed($"{step.Current.StepMethodInfo.Kind} {step.Current.StepText}");

                    while(step.MoveNext())
                    {
                        scenarioOutput.StepSkipped($"{step.Current.StepMethodInfo.Kind} {step.Current.StepText}");
                    }

                    throw;
                }
            }
        }
    }
}
