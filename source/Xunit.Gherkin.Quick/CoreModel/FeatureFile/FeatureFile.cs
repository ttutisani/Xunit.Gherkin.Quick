using Gherkin;
using Gherkin.Ast;
using System.Linq;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureFile
    {
        public GherkinDocument GherkinDocument { get; }
        public GherkinDialect GherkinDialect { get; }

        public FeatureFile(GherkinDocument gherkinDocument)
        {
            GherkinDocument = gherkinDocument ?? throw new System.ArgumentNullException(nameof(gherkinDocument));
            var dialectProvider = new GherkinDialectProvider();
            try
            {
                GherkinDialect = dialectProvider.GetDialect(gherkinDocument.Feature.Language, gherkinDocument.Feature.Location);
            }
            catch
            {
                GherkinDialect = dialectProvider.DefaultDialect;
            }
        }

        public global::Gherkin.Ast.Scenario GetScenario(string scenarioName)
        {
            return GherkinDocument.Feature.Children.FirstOrDefault(s => s.Name == scenarioName) as global::Gherkin.Ast.Scenario;
        }

		public global::Gherkin.Ast.Background GetBackground()
		{
			return GherkinDocument.Feature.Children.OfType<global::Gherkin.Ast.Background>().SingleOrDefault();
		}

        internal ScenarioOutline GetScenarioOutline(string scenarioOutlineName)
        {
            return GherkinDocument.Feature.Children.FirstOrDefault(s => s.Name == scenarioOutlineName) as global::Gherkin.Ast.ScenarioOutline;
        }
    }
}
