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
        public void GetScenarioTags_Requires_Arguments(
            Gherkin.Ast.Feature feature,
            string scenarioName
            )
        {
            //act / assert.
            Assert.Throws<ArgumentNullException>(() => feature.GetScenarioTags(scenarioName));
        }

        [Fact]
        public void GetScenarioTags_Retrieves_Tags_Of_Feature_And_Scenario_Combined()
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
        
        [Theory]
        [MemberData(nameof(DataFor_Required_Arguments))]
        public void IsScenarioIgnored_Requires_Arguments(
            Gherkin.Ast.Feature feature,
            string scenarioName
            )
        {
            //act / assert.
            Assert.Throws<ArgumentNullException>(() => feature.IsScenarioIgnored(scenarioName));
        }


        [Fact]
        public void IsScenarioIgnored_Does_Not_Treat_Ignored_If_No_Ignore_Tag()
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
            var isIgnored = feature.IsScenarioIgnored(scenarioName);

            //assert.
            Assert.False(isIgnored);
        }

        [Fact]
        public void IsScenarioIgnored_Treats_Ignored_If_Feature_Is_Ignored()
        {
            //arrange.
            var featureTags = new Gherkin.Ast.Tag[]
            {
                new Gherkin.Ast.Tag(null, "featuretag-1"),
                new Gherkin.Ast.Tag(null, GherkinFeatureExtensions.IgnoreTag)
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
            var isIgnored = feature.IsScenarioIgnored(scenarioName);

            //assert.
            Assert.True(isIgnored);
        }

        [Fact]
        public void IsScenarioIgnored_Treats_Ignored_If_Scenario_Is_Ignored()
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
                new Gherkin.Ast.Tag(null, GherkinFeatureExtensions.IgnoreTag),
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
            var isIgnored = feature.IsScenarioIgnored(scenarioName);

            //assert.
            Assert.True(isIgnored);
        }

        [Fact(Skip = "something")]
        public void Should_Skip()
        {

        }
    }
}
