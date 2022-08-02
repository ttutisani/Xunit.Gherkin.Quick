using System.Linq;
using Gherkin.Ast;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureFile
    {
        
        public GherkinDocument GherkinDocument { get; }

        public FeatureFile(GherkinDocument gherkinDocument)
            => GherkinDocument = gherkinDocument ?? throw new System.ArgumentNullException(nameof(gherkinDocument));

        public global::Gherkin.Ast.Scenario GetScenario(string scenarioName)
            => GherkinDocument.Feature.Scenarios().FirstOrDefault(s => s.Name == scenarioName);
        
        public Background GetBackground()
            => GherkinDocument.Feature.Children.OfType<Background>().SingleOrDefault();

        internal global::Gherkin.Ast.Scenario GetScenarioOutline(string scenarioOutlineName)
            => GherkinDocument.Feature.Outlines().FirstOrDefault(s => s.Name == scenarioOutlineName);

    }
}
