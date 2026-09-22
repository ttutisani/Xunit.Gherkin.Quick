using Gherkin;
using Gherkin.Ast;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureFile
    {
        public GherkinDocument GherkinDocument { get; }

        public FeatureFile(GherkinDocument gherkinDocument)
            => GherkinDocument = gherkinDocument ?? throw new ArgumentNullException(nameof(gherkinDocument));

        public global::Gherkin.Ast.Scenario GetScenario(string scenarioName)
        {
            var scenario = GherkinDocument.Feature.Children.FirstOrDefault(s => s.Name == scenarioName) as global::Gherkin.Ast.Scenario;
            if (scenario is null || string.IsNullOrWhiteSpace(GherkinDocument.Feature.Language))
                return scenario;

            var dialectProvider = new GherkinDialectProvider();
            var gherkinDialect = dialectProvider.GetDialect(GherkinDocument.Feature.Language, GherkinDocument.Feature.Location);

            return new global::Gherkin.Ast.Scenario(
                (scenario.Tags as Tag[])?.ToArray(),
                scenario.Location,
                scenario.Keyword,
                scenario.Name,
                scenario.Description,
                _Translate(scenario.Steps, gherkinDialect)
            );
        }

        public Background GetBackground()
        {
            var background = GherkinDocument.Feature.Children.OfType<Background>().SingleOrDefault();
            if (background is null || string.IsNullOrWhiteSpace(GherkinDocument.Feature.Language))
                return background;

            var dialectProvider = new GherkinDialectProvider();
            var gherkinDialect = dialectProvider.GetDialect(GherkinDocument.Feature.Language, GherkinDocument.Feature.Location);

            return new Background(
                background.Location,
                background.Keyword,
                background.Name,
                background.Description,
                _Translate(background.Steps, gherkinDialect)
            );
        }

        internal ScenarioOutline GetScenarioOutline(string scenarioOutlineName)
        {
            var scenarioOutline = GherkinDocument.Feature.Children.FirstOrDefault(s => s.Name == scenarioOutlineName) as ScenarioOutline;
            if (scenarioOutline is null || string.IsNullOrWhiteSpace(GherkinDocument.Feature.Language))
                return scenarioOutline;

            var dialectProvider = new GherkinDialectProvider();
            var gherkinDialect = dialectProvider.GetDialect(GherkinDocument.Feature.Language, GherkinDocument.Feature.Location);

            return new ScenarioOutline(
                (scenarioOutline.Tags as Tag[])?.ToArray(),
                scenarioOutline.Location,
                scenarioOutline.Keyword,
                scenarioOutline.Name,
                scenarioOutline.Description,
                _Translate(scenarioOutline.Steps, gherkinDialect),
                (scenarioOutline.Examples as Examples[])?.ToArray()
            );
        }

        private Step[] _Translate(IEnumerable<Step> steps, GherkinDialect gherkinDialect)
            => steps
                .Select(step => _TranslateStep(step, gherkinDialect))
                .ToArray();

        private static Step _TranslateStep(Step step, GherkinDialect gherkinDialect)
        {
            string translatedKeyword = null;
            if (step.Keyword.Trim() != "*")
                if (gherkinDialect.GivenStepKeywords.Contains(step.Keyword))
                    translatedKeyword = "Given ";
                else if (gherkinDialect.WhenStepKeywords.Contains(step.Keyword))
                    translatedKeyword = "When ";
                else if (gherkinDialect.ThenStepKeywords.Contains(step.Keyword))
                    translatedKeyword = "Then ";
                else if (gherkinDialect.AndStepKeywords.Contains(step.Keyword))
                    translatedKeyword = "And ";
                else if (gherkinDialect.ButStepKeywords.Contains(step.Keyword))
                    translatedKeyword = "But ";

            if (translatedKeyword != null)
                return new Step(step.Location, translatedKeyword, step.Text, step.Argument);
            else
                return step;
        }
    }
}
