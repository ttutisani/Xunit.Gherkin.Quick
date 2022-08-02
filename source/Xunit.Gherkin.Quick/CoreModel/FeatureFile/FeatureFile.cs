using System;
using System.Linq;
using System.Collections.Generic;
using Gherkin.Ast;
using System.Linq;


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
            => GherkinDocument.Feature.Children.OfType<global::Gherkin.Ast.Background>().SingleOrDefault();

        internal global::Gherkin.Ast.Scenario GetScenarioOutline(string scenarioOutlineName)
            => GherkinDocument.Feature.Outlines().FirstOrDefault(s => s.Name == scenarioOutlineName);

    }
}
