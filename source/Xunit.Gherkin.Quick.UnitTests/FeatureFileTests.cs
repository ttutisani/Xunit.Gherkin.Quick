using System.Linq;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class FeatureFileTests
    {
        [Fact]
        public void Ctor_Initializes_Properties()
        {
            //arrange.
            var gherkinDocument = new Gherkin.Ast.GherkinDocument(null, null);

            //act.
            var sut = new FeatureFile(gherkinDocument);

            //assert.
            Assert.Same(gherkinDocument, sut.GherkinDocument);
        }

        [Fact]
        public void GetScenario_Retrieves_If_Found()
        {
            //arrange.
            var scenarioName = "name exists";
            var sut = new FeatureFile(CreateGherkinDocumentWithScenario(scenarioName));

            //act.
            var scenario = sut.GetScenario(scenarioName);

            //assert.
            Assert.NotNull(scenario);
            Assert.Same(sut.GherkinDocument.Feature.Children.First(), scenario);
        }

        [Fact]
        public void GetScenario_Gives_Null_If_Not_Found()
        {
            //arrange.
            var sut = new FeatureFile(CreateGherkinDocumentWithScenario("existing"));

            //act.
            var scenario = sut.GetScenario("non-existing");

            //assert.
            Assert.Null(scenario);
        }

        [Fact]
        public void GetScenario_Applies_Translaation()
        {
            //arrange.
            var sut = new FeatureFile(CreateGherkinDocumentWithSlovakScenario("existing"));

            //act.
            var scenario = sut.GetScenario("existing");

            //assert.
            Assert.NotNull(scenario);
            Assert.Collection(
                scenario.Steps,
                firstStep => Assert.Multiple(
                    () => Assert.Equal("Given ", firstStep.Keyword),
                    () => Assert.Equal("given", firstStep.Text)
                ),
                secondStep => Assert.Multiple(
                    () => Assert.Equal("And ", secondStep.Keyword),
                    () => Assert.Equal("and", secondStep.Text)
                ),
                thirdStep => Assert.Multiple(
                    () => Assert.Equal("But ", thirdStep.Keyword),
                    () => Assert.Equal("but", thirdStep.Text)
                ),
                fourthStep => Assert.Multiple(
                    () => Assert.Equal("When ", fourthStep.Keyword),
                    () => Assert.Equal("when", fourthStep.Text)
                ),
                fifthStep => Assert.Multiple(
                    () => Assert.Equal("Then ", fifthStep.Keyword),
                    () => Assert.Equal("then", fifthStep.Text)
                )
            );
        }

        [Fact]
        public void GetScenarioOutline_Retrieves_If_Found()
        {
            //arrange.
            var scenarioName = "name exists";
            var sut = new FeatureFile(CreateGherkinDocumentWithScenarioOutline(scenarioName));

            //act.
            var scenario = sut.GetScenarioOutline(scenarioName);

            //assert.
            Assert.NotNull(scenario);
            Assert.Same(sut.GherkinDocument.Feature.Children.First(), scenario);
        }

        [Fact]
        public void GetScenarioOutline_Gives_Null_If_Not_Found()
        {
            //arrange.
            var sut = new FeatureFile(CreateGherkinDocumentWithScenarioOutline("existing"));

            //act.
            var scenario = sut.GetScenarioOutline("non-existing");

            //assert.
            Assert.Null(scenario);
        }

        [Fact]
        public void GetScenarioOutline_Applies_Translaation()
        {
            //arrange.
            var sut = new FeatureFile(CreateGherkinDocumentWithSlovakScenarioOutline("existing"));

            //act.
            var scenarioOutline = sut.GetScenarioOutline("existing");

            //assert.
            Assert.NotNull(scenarioOutline);
            Assert.Collection(
                scenarioOutline.Steps,
                firstStep => Assert.Multiple(
                    () => Assert.Equal("Given ", firstStep.Keyword),
                    () => Assert.Equal("given", firstStep.Text)
                ),
                secondStep => Assert.Multiple(
                    () => Assert.Equal("And ", secondStep.Keyword),
                    () => Assert.Equal("and", secondStep.Text)
                ),
                thirdStep => Assert.Multiple(
                    () => Assert.Equal("But ", thirdStep.Keyword),
                    () => Assert.Equal("but", thirdStep.Text)
                ),
                fourthStep => Assert.Multiple(
                    () => Assert.Equal("When ", fourthStep.Keyword),
                    () => Assert.Equal("when", fourthStep.Text)
                ),
                fifthStep => Assert.Multiple(
                    () => Assert.Equal("Then ", fifthStep.Keyword),
                    () => Assert.Equal("then", fifthStep.Text)
                )
            );
        }

        [Fact]
        public void GetBackground_Retrieves_If_Present()
        {
            var sut = new FeatureFile(CreateGherkinDocumentWithBackground());
            var background = sut.GetBackground();
            Assert.NotNull(background);
        }

        [Fact]
        public void GetBackground_Gives_Null_If_Not_Present()
        {
            var sut = new FeatureFile(CreateGherkinDocumentWithScenario("test"));
            var background = sut.GetBackground();
            Assert.Null(background);
        }

        [Fact]
        public void GetBackground_Applies_Translaation()
        {
            //arrange.
            var sut = new FeatureFile(CreateGherkinDocumentWithSlovakBackground());

            //act.
            var background = sut.GetBackground();

            //assert.
            Assert.NotNull(background);
            Assert.Collection(
                background.Steps,
                firstStep => Assert.Multiple(
                    () => Assert.Equal("Given ", firstStep.Keyword),
                    () => Assert.Equal("given", firstStep.Text)
                ),
                secondStep => Assert.Multiple(
                    () => Assert.Equal("And ", secondStep.Keyword),
                    () => Assert.Equal("and", secondStep.Text)
                ),
                thirdStep => Assert.Multiple(
                    () => Assert.Equal("But ", thirdStep.Keyword),
                    () => Assert.Equal("but", thirdStep.Text)
                ),
                fourthStep => Assert.Multiple(
                    () => Assert.Equal("When ", fourthStep.Keyword),
                    () => Assert.Equal("when", fourthStep.Text)
                ),
                fifthStep => Assert.Multiple(
                    () => Assert.Equal("Then ", fifthStep.Keyword),
                    () => Assert.Equal("then", fifthStep.Text)
                )
            );
        }

        private static Gherkin.Ast.GherkinDocument CreateGherkinDocumentWithScenario(
            string scenario,
            Gherkin.Ast.StepArgument stepArgument = null)
        {
            return new Gherkin.Ast.GherkinDocument(
                new Gherkin.Ast.Feature(new Gherkin.Ast.Tag[0], null, null, null, null, null, new Gherkin.Ast.ScenarioDefinition[]
                {
                    new Gherkin.Ast.Scenario(
                        new Gherkin.Ast.Tag[0],
                        null,
                        null,
                        scenario,
                        null,
                        new Gherkin.Ast.Step[]{ })
                }),
                new Gherkin.Ast.Comment[0]);
        }

        private static Gherkin.Ast.GherkinDocument CreateGherkinDocumentWithBackground()
        {
            return new Gherkin.Ast.GherkinDocument(
                new Gherkin.Ast.Feature(new Gherkin.Ast.Tag[0], null, null, null, null, null, new Gherkin.Ast.ScenarioDefinition[]
                {
                    new Gherkin.Ast.Background(
                        null,
                        null,
                        null,
                        null,
                        new Gherkin.Ast.Step[]{ })
                }),
                new Gherkin.Ast.Comment[0]);
        }

        private static Gherkin.Ast.GherkinDocument CreateGherkinDocumentWithScenarioOutline(
            string scenario,
            Gherkin.Ast.StepArgument stepArgument = null)
        {
            return new Gherkin.Ast.GherkinDocument(
                new Gherkin.Ast.Feature(new Gherkin.Ast.Tag[0], null, null, null, null, null, new Gherkin.Ast.ScenarioDefinition[]
                {
                    new Gherkin.Ast.ScenarioOutline(
                        new Gherkin.Ast.Tag[0],
                        null,
                        null,
                        scenario,
                        null,
                        new Gherkin.Ast.Step[]{ },
                        new Gherkin.Ast.Examples[]{ })
                }),
                new Gherkin.Ast.Comment[0]);
        }

        private static Gherkin.Ast.GherkinDocument CreateGherkinDocumentWithSlovakScenario(string scenario)
        {
            return new Gherkin.Ast.GherkinDocument(
                new Gherkin.Ast.Feature(new Gherkin.Ast.Tag[0], null, "sk", null, null, null, new Gherkin.Ast.ScenarioDefinition[]
                {
                    new Gherkin.Ast.Scenario(
                        new Gherkin.Ast.Tag[0],
                        null,
                        null,
                        scenario,
                        null,
                        CreateSlovakSteps()
                    )
                }),
                new Gherkin.Ast.Comment[0]);
        }

        private static Gherkin.Ast.GherkinDocument CreateGherkinDocumentWithSlovakScenarioOutline(string scenario)
        {
            return new Gherkin.Ast.GherkinDocument(
                new Gherkin.Ast.Feature(new Gherkin.Ast.Tag[0], null, "sk", null, null, null, new Gherkin.Ast.ScenarioDefinition[]
                {
                    new Gherkin.Ast.ScenarioOutline(
                        new Gherkin.Ast.Tag[0],
                        null,
                        null,
                        scenario,
                        null,
                        CreateSlovakSteps(),
                        new Gherkin.Ast.Examples[0]
                    )
                }),
                new Gherkin.Ast.Comment[0]);
        }

        private static Gherkin.Ast.GherkinDocument CreateGherkinDocumentWithSlovakBackground()
        {
            return new Gherkin.Ast.GherkinDocument(
                new Gherkin.Ast.Feature(new Gherkin.Ast.Tag[0], null, "sk", null, null, null, new Gherkin.Ast.ScenarioDefinition[]
                {
                    new Gherkin.Ast.Background(
                        null,
                        null,
                        "background",
                        null,
                        CreateSlovakSteps()
                    )
                }),
                new Gherkin.Ast.Comment[0]);
        }

        private static Gherkin.Ast.Step[] CreateSlovakSteps()
            => new Gherkin.Ast.Step[]
            {
                new Gherkin.Ast.Step(null, "Pokiaľ ", "given", null),
                new Gherkin.Ast.Step(null, "A ", "and", null),
                new Gherkin.Ast.Step(null, "Ale ", "but", null),
                new Gherkin.Ast.Step(null, "Keď ", "when", null),
                new Gherkin.Ast.Step(null, "Tak ", "then", null)    
            };
    }
}
