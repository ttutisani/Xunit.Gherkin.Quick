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
            var sut = new FeatureFile(CreateGherkinDocument(scenarioName, new string[0]));

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
            var sut = new FeatureFile(CreateGherkinDocument("existing", new string[0]));

            //act.
            var scenario = sut.GetScenario("non-existing");

            //assert.
            Assert.Null(scenario);
        }

        private static Gherkin.Ast.GherkinDocument CreateGherkinDocument(
            string scenario,
            string[] steps,
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
                        steps.Select(s =>
                        {
                            var spaceIndex = s.IndexOf(' ');
                            return new Gherkin.Ast.Step(
                                null,
                                s.Substring(0, spaceIndex).Trim(),
                                s.Substring(spaceIndex).Trim(),
                                stepArgument);
                        }).ToArray())
                }),
                new Gherkin.Ast.Comment[0]);
        }
    }
}
