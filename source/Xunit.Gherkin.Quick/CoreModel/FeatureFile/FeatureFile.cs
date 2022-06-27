using System;
using System.Linq;
using System.Collections.Generic;
using Gherkin.Ast;
using System.Linq;

using Scenario = global::Gherkin.Ast.Scenario;
using Background = global::Gherkin.Ast.Background;
using StepsContainer = Gherkin.Ast.StepsContainer;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureFile
    {
        
        public GherkinDocument GherkinDocument { get; }

        public FeatureFile(GherkinDocument gherkinDocument)
        {
            GherkinDocument = gherkinDocument ?? throw new System.ArgumentNullException(nameof(gherkinDocument));

            GherkinDialect.Register(
                GherkinDocument.Feature?.Language,
                GherkinDocument.Feature?.Location
            );
        }

        public global::Gherkin.Ast.Scenario GetScenario(string scenarioName)
            => GherkinDocument.Feature.Scenarios().Where(s => s.Name == scenarioName).FirstOrDefault();

		public global::Gherkin.Ast.Background GetBackground()
		{
			return GherkinDocument.Feature.Children.OfType<global::Gherkin.Ast.Background>().SingleOrDefault();
		}

        internal global::Gherkin.Ast.Scenario GetScenarioOutline(string scenarioOutlineName)
            => GherkinDocument.Feature.Outlines().Where(s => s.Name == scenarioOutlineName).FirstOrDefault();

    }
}
