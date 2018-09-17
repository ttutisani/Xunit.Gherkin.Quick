using System.Collections.Generic;

namespace Xunit.Gherkin.Quick
{
    public abstract class StepContainer
    {
		protected StepContainer()
		{
			ScenarioContext = new Dictionary<string, object>();
		}

		public Dictionary<string, object> ScenarioContext { get; internal set; }
    }
}
