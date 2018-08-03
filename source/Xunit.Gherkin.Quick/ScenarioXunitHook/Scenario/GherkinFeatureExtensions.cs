using System;
using System.Collections.Generic;
using System.Linq;

namespace Xunit.Gherkin.Quick
{
    internal static class GherkinFeatureExtensions
    {
        public static List<string> GetScenarioTags(
            this global::Gherkin.Ast.Feature feature,
            string scenarioName)
        {
            if (feature == null)
                throw new ArgumentNullException(nameof(feature));

            if (string.IsNullOrWhiteSpace(scenarioName))
                throw new ArgumentNullException(nameof(scenarioName));

            var scenario = feature
                .Children
                .OfType<global::Gherkin.Ast.Scenario>()
                .FirstOrDefault(s => s.Name == scenarioName);

            if (scenario == null)
                throw new InvalidOperationException($"Cannot find scenario `{scenarioName}` under feature `{feature.Name}`.");

            var scenarioTags = feature.Tags
                ?.Union(scenario.Tags ?? new List<global::Gherkin.Ast.Tag>())
                ?.Select(t => t.Name)
                ?.ToList();

            return scenarioTags ?? new List<string>();
        }

        public const string IgnoreTag = "@ignore";

        public static bool IsScenarioIgnored(
            this global::Gherkin.Ast.Feature feature,
            string scenarioName)
        {
            var scenarioTags = feature.GetScenarioTags(scenarioName);
            return scenarioTags.Contains(IgnoreTag);
        }

        public static List<string> GetExamplesTags(
            this global::Gherkin.Ast.Feature feature,
            string scenarioOutlineName,
            string examplesName)
        {
            if (feature == null)
                throw new ArgumentNullException(nameof(feature));

            if (string.IsNullOrWhiteSpace(scenarioOutlineName))
                throw new ArgumentNullException(nameof(scenarioOutlineName));

            if (string.IsNullOrWhiteSpace(examplesName))
                throw new ArgumentNullException(nameof(examplesName));

            var scenarioOutline = feature.Children.OfType<global::Gherkin.Ast.ScenarioOutline>()
                .FirstOrDefault(o => o.Name == scenarioOutlineName);

            if (scenarioOutline == null)
                throw new InvalidOperationException($"Cannot find scenario outline `{scenarioOutlineName}` under feature `{feature.Name}`.");

            var examples = scenarioOutline.Examples.FirstOrDefault(e => e.Name == examplesName);

            if (examples == null)
                throw new InvalidOperationException($"Cannot find examples `{examplesName}` under scenario outline `{scenarioOutlineName}` and feature `{feature.Name}`.");

            var examplesTags = feature.Tags
                ?.Union(scenarioOutline.Tags ?? new List<global::Gherkin.Ast.Tag>())
                ?.Union(examples.Tags ?? new List<global::Gherkin.Ast.Tag>())
                ?.Select(t => t.Name)
                ?.ToList();

            return examplesTags ?? new List<string>();
        }
    }
}
