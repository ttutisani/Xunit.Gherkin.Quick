using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Xunit.Gherkin.Quick
{
    internal sealed class Scenario
    {
        private readonly ReadOnlyCollection<StepMethod> _steps;
        private Feature _instance;

        public Scenario(IEnumerable<StepMethod> stepMethods, Feature feature)
        {
            _instance = feature;
            _steps = stepMethods != null
                ? new ReadOnlyCollection<StepMethod>(stepMethods.ToList())
                : throw new ArgumentNullException(nameof(stepMethods));
        }

        public async Task ExecuteAsync(IScenarioOutput scenarioOutput)
        {
            if (scenarioOutput == null)
                throw new ArgumentNullException(nameof(scenarioOutput));

            await _instance.InvokeAsync(BeforeAfterAttributeHelper.InvokeMethodType.BeforeScenario);
            var step = _steps.GetEnumerator();
            try
            {
                while (step.MoveNext())
                {
                    try
                    {
                        await step.Current.ExecuteAsync();
                        scenarioOutput.StepPassed($"{step.Current.Kind} {step.Current.StepText}");
                    }
                    catch
                    {
                        scenarioOutput.StepFailed($"{step.Current.Kind} {step.Current.StepText}");

                        while (step.MoveNext())
                        {
                            scenarioOutput.StepSkipped($"{step.Current.Kind} {step.Current.StepText}");
                        }

                        throw;
                    }
                }
            }
            finally
            {
                await _instance.InvokeAsync(BeforeAfterAttributeHelper.InvokeMethodType.AfterScenario);
            }
        }
    }
}
