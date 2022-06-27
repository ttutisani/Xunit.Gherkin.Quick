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

            if (StepMethodInfo.Dialects.Any(d => d.Language == (GherkinDocument.Feature.Language ?? StepMethodInfo.GherkingDialectProvider.DefaultDialect.Language)) is false)
                StepMethodInfo.Dialects.Add(StepMethodInfo.GherkingDialectProvider.GetDialect(GherkinDocument.Feature.Language, GherkinDocument.Feature.Location));
        }

        public global::Gherkin.Ast.Scenario GetScenario(string scenarioName)
        {

            return GherkinDocument.Feature.Children.FirstOrDefault(c => (c as global::Gherkin.Ast.Scenario)?.Name == scenarioName) as global::Gherkin.Ast.Scenario;

            //return GherkinDocument.Feature.Children.FirstOrDefault(s => s.Name == scenarioName) as global::Gherkin.Ast.Scenario;
        }

		public global::Gherkin.Ast.Background GetBackground()
		{
			return GherkinDocument.Feature.Children.OfType<global::Gherkin.Ast.Background>().SingleOrDefault();
		}

        internal global::Gherkin.Ast.Scenario GetScenarioOutline(string scenarioOutlineName)
        {

            return GherkinDocument.Feature.Children.FirstOrDefault(c => (c as global::Gherkin.Ast.Scenario)?.Name == scenarioOutlineName) as global::Gherkin.Ast.Scenario;
            //return GherkinDocument.Feature.Children.FirstOrDefault(s => (s as StepsContainer)?.Name == scenarioOutlineName) as global::Gherkin.Ast.StepsContainer;
        }
    }
}
