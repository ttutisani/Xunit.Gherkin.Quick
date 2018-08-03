using System;
using System.Linq;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class GherkinFeatureTests
    {
        public static object[][] DataFor_Feature_And_Scenario_Null_Arguments
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
        [MemberData(nameof(DataFor_Feature_And_Scenario_Null_Arguments))]
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
        [MemberData(nameof(DataFor_Feature_And_Scenario_Null_Arguments))]
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

        public static object[][] DataFor_Feature_And_ScenarioOutline_And_Examples_Null_Arguments
        {
            get
            {
                return new object[][]
                {
                    new object[]{ null, "scenario outline name", "examples name" },
                    new object[]{ new Gherkin.Ast.Feature(null, null, null, null, null, null, null), null, "examples name" },
                    new object[]{ new Gherkin.Ast.Feature(null, null, null, null, null, null, null), "scenario outline name", null }
                };
            }
        }

        [Theory]
        [MemberData(nameof(DataFor_Feature_And_ScenarioOutline_And_Examples_Null_Arguments))]
        public void GetExamplesTags_Requires_Arguments(
            Gherkin.Ast.Feature feature,
            string scenarioOutlineName,
            string examplesName
            )
        {
            //act / assert.
            Assert.Throws<ArgumentNullException>(() => feature.GetExamplesTags(
                scenarioOutlineName,
                examplesName));
        }

        [Fact]
        public void GetExamplesTags_Retrieves_Tags_Of_Feature_And_OutLine_And_Examples_Combined()
        {
            //arrange.
            var featureTags = new Gherkin.Ast.Tag[]
            {
                new Gherkin.Ast.Tag(null, "featuretag-1"),
                new Gherkin.Ast.Tag(null, "featuretag-2")
            };

            var outlineTags = new Gherkin.Ast.Tag[]
            {
                new Gherkin.Ast.Tag(null, "outlinetag-1"),
                new Gherkin.Ast.Tag(null, "outlinetag-2"),
                new Gherkin.Ast.Tag(null, "outlinetag-3")
            };

            var examplesTags = new Gherkin.Ast.Tag[]
            {
                new Gherkin.Ast.Tag(null, "examplestag-1"),
                new Gherkin.Ast.Tag(null, "examplestag-2"),
                new Gherkin.Ast.Tag(null, "examplestag-2"),
                new Gherkin.Ast.Tag(null, "examplestag-3")
            };

            var scenarioOutlineName = "scenario name 123";
            var examplesName = "examples name 123";

            var feature = new Gherkin.Ast.Feature(
                featureTags, null, null, null, null, null,
                new Gherkin.Ast.ScenarioDefinition[]
                {
                    new Gherkin.Ast.ScenarioOutline(outlineTags, null, null, scenarioOutlineName, null, null,
                    new Gherkin.Ast.Examples[]
                    {
                        new Gherkin.Ast.Examples(examplesTags, null, null, examplesName, null, null, null)
                    })
                });

            //act.
            var examplesTagNames = feature.GetExamplesTags(scenarioOutlineName, examplesName);

            //assert.
            Assert.NotNull(examplesTagNames);

            var expectedTagNames = featureTags.Union(outlineTags).Union(examplesTags).Select(t => t.Name);
            Assert.Equal(expectedTagNames, examplesTagNames);
        }

        //[Theory]
        //[MemberData(nameof(DataFor_Required_Arguments))]
        //public void IsScenarioIgnored_Requires_Arguments(
        //    Gherkin.Ast.Feature feature,
        //    string scenarioName
        //    )
        //{
        //    //act / assert.
        //    Assert.Throws<ArgumentNullException>(() => feature.IsScenarioIgnored(scenarioName));
        //}


        //[Fact]
        //public void IsScenarioIgnored_Does_Not_Treat_Ignored_If_No_Ignore_Tag()
        //{
        //    //arrange.
        //    var featureTags = new Gherkin.Ast.Tag[]
        //    {
        //        new Gherkin.Ast.Tag(null, "featuretag-1"),
        //        new Gherkin.Ast.Tag(null, "featuretag-2")
        //    };

        //    var scenarioTags = new Gherkin.Ast.Tag[]
        //    {
        //        new Gherkin.Ast.Tag(null, "scenarioTag-1"),
        //        new Gherkin.Ast.Tag(null, "scenarioTag-2"),
        //        new Gherkin.Ast.Tag(null, "scenarioTag-3")
        //    };

        //    var scenarioName = "scenario name 123";

        //    var feature = new Gherkin.Ast.Feature(
        //        featureTags, null, null, null, null, null,
        //        new Gherkin.Ast.ScenarioDefinition[]
        //        {
        //            new Gherkin.Ast.Scenario(scenarioTags, null, null, scenarioName, null, null)
        //        });

        //    //act.
        //    var isIgnored = feature.IsScenarioIgnored(scenarioName);

        //    //assert.
        //    Assert.False(isIgnored);
        //}

        //[Fact]
        //public void IsScenarioIgnored_Treats_Ignored_If_Feature_Is_Ignored()
        //{
        //    //arrange.
        //    var featureTags = new Gherkin.Ast.Tag[]
        //    {
        //        new Gherkin.Ast.Tag(null, "featuretag-1"),
        //        new Gherkin.Ast.Tag(null, GherkinFeatureExtensions.IgnoreTag)
        //    };

        //    var scenarioTags = new Gherkin.Ast.Tag[]
        //    {
        //        new Gherkin.Ast.Tag(null, "scenarioTag-1"),
        //        new Gherkin.Ast.Tag(null, "scenarioTag-2"),
        //        new Gherkin.Ast.Tag(null, "scenarioTag-3")
        //    };

        //    var scenarioName = "scenario name 123";

        //    var feature = new Gherkin.Ast.Feature(
        //        featureTags, null, null, null, null, null,
        //        new Gherkin.Ast.ScenarioDefinition[]
        //        {
        //            new Gherkin.Ast.Scenario(scenarioTags, null, null, scenarioName, null, null)
        //        });

        //    //act.
        //    var isIgnored = feature.IsScenarioIgnored(scenarioName);

        //    //assert.
        //    Assert.True(isIgnored);
        //}

        //[Fact]
        //public void IsScenarioIgnored_Treats_Ignored_If_Scenario_Is_Ignored()
        //{
        //    //arrange.
        //    var featureTags = new Gherkin.Ast.Tag[]
        //    {
        //        new Gherkin.Ast.Tag(null, "featuretag-1"),
        //        new Gherkin.Ast.Tag(null, "featuretag-2")
        //    };

        //    var scenarioTags = new Gherkin.Ast.Tag[]
        //    {
        //        new Gherkin.Ast.Tag(null, "scenarioTag-1"),
        //        new Gherkin.Ast.Tag(null, GherkinFeatureExtensions.IgnoreTag),
        //        new Gherkin.Ast.Tag(null, "scenarioTag-3")
        //    };

        //    var scenarioName = "scenario name 123";

        //    var feature = new Gherkin.Ast.Feature(
        //        featureTags, null, null, null, null, null,
        //        new Gherkin.Ast.ScenarioDefinition[]
        //        {
        //            new Gherkin.Ast.Scenario(scenarioTags, null, null, scenarioName, null, null)
        //        });

        //    //act.
        //    var isIgnored = feature.IsScenarioIgnored(scenarioName);

        //    //assert.
        //    Assert.True(isIgnored);
        //}
    }
}
