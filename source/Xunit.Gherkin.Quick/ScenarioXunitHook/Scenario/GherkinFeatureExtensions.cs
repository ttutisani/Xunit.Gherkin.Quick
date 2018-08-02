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
    }
}
