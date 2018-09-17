using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Xunit.Gherkin.Quick
{
	internal sealed class Scenario
	{
		private readonly Dictionary<string, object> _context;
		private readonly ReadOnlyCollection<StepMethod> _steps;

		public Scenario(IEnumerable<StepMethod> stepMethods)
		{
			_context = new Dictionary<string, object>();
			_steps = stepMethods != null
				? new ReadOnlyCollection<StepMethod>(stepMethods.ToList())
				: throw new ArgumentNullException(nameof(stepMethods));
		}				

		public Scenario(Scenario background, Scenario main)
		{
			_context = new Dictionary<string, object>();
			_steps = background._steps.ToList().Concat(main._steps).ToList().AsReadOnly();		
		}
		
        public async Task ExecuteAsync(IScenarioOutput scenarioOutput)
        {
            if (scenarioOutput == null)
                throw new ArgumentNullException(nameof(scenarioOutput));

            var step = _steps.GetEnumerator();
            while(step.MoveNext())
            {
                try
                {
                    await step.Current.ExecuteAsync(_context);
                    scenarioOutput.StepPassed($"{step.Current.Kind} {step.Current.StepText}");
                }
                catch
                {
                    scenarioOutput.StepFailed($"{step.Current.Kind} {step.Current.StepText}");

                    while(step.MoveNext())
                    {
                        scenarioOutput.StepSkipped($"{step.Current.Kind} {step.Current.StepText}");
                    }

                    throw;
                }
            }
        }
    }
}
