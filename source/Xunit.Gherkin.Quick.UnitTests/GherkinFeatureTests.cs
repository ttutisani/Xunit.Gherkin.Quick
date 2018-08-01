using System;
using System.Linq;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class GherkinFeatureTests
    {
        public static object[][] DataFor_Required_Arguments
        {
            get
            {
                return new object[][] 
                {
                    new object[]{ new Gherkin.Ast.Feature(null, null, null, null, null, null, null), null },
                    new object[]{ null, "scenario name" }
                };
            }
        }

        [Theory]
        [MemberData(nameof(DataFor_Required_Arguments))]
        public void GetCombinedTags_Requires_Arguments(
            Gherkin.Ast.Feature feature,
            string scenarioName
            )
        {
            //act / assert.
            Assert.Throws<ArgumentNullException>(() => feature.GetScenarioTags(scenarioName));
        }

        [Fact]
        public void GetCombinedTags_Retrieves_Tags_Of_Feature_And_Scenario_Combined()
        {
            //arrange.
            var featureTags = new Gherkin.Ast.Tag[] 
            {
                new Gherkin.Ast.Tag(null, "featuretag-1"),
                new Gherkin.Ast.Tag(null, "featuretag-2")
            };

            var scenarioTags = new Gherkin.Ast.Tag[] 
            {
                new Gherkin.Ast.Tag(null, "scenarioTag-1"),
                new Gherkin.Ast.Tag(null, "scenarioTag-2"),
                new Gherkin.Ast.Tag(null, "scenarioTag-3")
            };

            var scenarioName = "scenario name 123";

            var feature = new Gherkin.Ast.Feature(
                featureTags, null, null, null, null, null,
                new Gherkin.Ast.ScenarioDefinition[] 
                {
                    new Gherkin.Ast.Scenario(scenarioTags, null, null, scenarioName, null, null)
                });

            //act.
            var scenarioTagNames = feature.GetScenarioTags(scenarioName);

            //assert.
            Assert.NotNull(scenarioTagNames);

            var expectedTagNames = featureTags.Union(scenarioTags).Select(t => t.Name);
            Assert.Equal(expectedTagNames, scenarioTagNames);
        }
    }
}
