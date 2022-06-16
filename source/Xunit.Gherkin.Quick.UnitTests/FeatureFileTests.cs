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

        private static Gherkin.Ast.GherkinDocument CreateGherkinDocumentWithScenario(
            string scenario,
            Gherkin.Ast.StepArgument stepArgument = null)
        {
            return new Gherkin.Ast.GherkinDocument(
                new Gherkin.Ast.Feature(new Gherkin.Ast.Tag[0], null, null, null, null, null, new Gherkin.Ast.Scenario[]
                {
                    new Gherkin.Ast.Scenario(
                        new Gherkin.Ast.Tag[0],
                        null,
                        null,
                        scenario,
                        null,
                        new Gherkin.Ast.Step[]{ }
                        , System.Array.Empty<global::Gherkin.Ast.Examples>())
                }),
                new Gherkin.Ast.Comment[0]);
        }

        private static Gherkin.Ast.GherkinDocument CreateGherkinDocumentWithBackground()
        {
            return new Gherkin.Ast.GherkinDocument(
                new Gherkin.Ast.Feature(new Gherkin.Ast.Tag[0], null, null, null, null, null, new Gherkin.Ast.Background[]
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
                new Gherkin.Ast.Feature(new Gherkin.Ast.Tag[0], null, null, null, null, null, new Gherkin.Ast.Scenario[]
                {
                    new Gherkin.Ast.Scenario(
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
    }
}
