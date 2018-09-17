using Gherkin.Ast;
using System;
using System.Linq;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureFile
    {
        public GherkinDocument GherkinDocument { get; }

        public FeatureFile(GherkinDocument gherkinDocument)
        {
            GherkinDocument = gherkinDocument ?? throw new System.ArgumentNullException(nameof(gherkinDocument));
        }

        public global::Gherkin.Ast.ScenarioDefinition GetScenario(string scenarioName)
        {
            return GherkinDocument.Feature.Children.FirstOrDefault(s => s.Name == scenarioName);
        }

		public global::Gherkin.Ast.ScenarioDefinition GetBackgroundScenario()
		{
			return GherkinDocument.Feature.Children.SingleOrDefault(s => s.Keyword == "Background");
		}

        internal ScenarioOutline GetScenarioOutline(string scenarioOutlineName)
        {
            return GherkinDocument.Feature.Children.FirstOrDefault(s => s.Name == scenarioOutlineName) as global::Gherkin.Ast.ScenarioOutline;
        }
    }
}
