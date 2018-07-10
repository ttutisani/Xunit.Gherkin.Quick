using System;
using System.Linq;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class GherkinScenarioOutlineTests
    {
        [Theory]
        [InlineData("non-existing example", 0)]
        [InlineData("existing example", 1)]
        public void ApplyExampleRow_Throws_Error_When_Example_Not_Found(
            string exampleName,
            int exampleRowIndex
            )
        {
            //arrange.
            var sut = new Gherkin.Ast.ScenarioOutline(
                null,
                null,
                null,
                "outline123",
                null,
                new Gherkin.Ast.Step[] { },
                new Gherkin.Ast.Examples[] 
                {
                    new Gherkin.Ast.Examples(
                        null,
                        null,
                        null,
                        "existing example",
                        null,
                        new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]{ }),
                        new Gherkin.Ast.TableRow[]
                        {
                            new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]{ })
                        })
                });

            //act / assert.
            Assert.Throws<InvalidOperationException>(() => sut.ApplyExampleRow(exampleName, exampleRowIndex));
        }

        [Fact]
        public void ApplyExampleRow_Digests_Row_Values_Into_Scenario()
        {
            //arrange.
            var sut = new Gherkin.Ast.ScenarioOutline(
                null,
                null,
                null,
                "outline123",
                null,
                new Gherkin.Ast.Step[] 
                {
                    new Gherkin.Ast.Step(null, "Given", "I chose <a> as first number", null),
                    new Gherkin.Ast.Step(null, "And", "I chose <b> as second number", null),
                    new Gherkin.Ast.Step(null, "When", "I press add", null),
                    new Gherkin.Ast.Step(null, "Then", "the result should be <sum> on the screen", null),
                },
                new Gherkin.Ast.Examples[]
                {
                    new Gherkin.Ast.Examples(
                        null,
                        null,
                        null,
                        "existing example",
                        null,
                        new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                        {
                            new Gherkin.Ast.TableCell(null, "a"),
                            new Gherkin.Ast.TableCell(null, "b"),
                            new Gherkin.Ast.TableCell(null, "sum")
                        }),
                        new Gherkin.Ast.TableRow[]
                        {
                            new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                            {
                                new Gherkin.Ast.TableCell(null, "1"),
                                new Gherkin.Ast.TableCell(null, "2"),
                                new Gherkin.Ast.TableCell(null, "3")
                            })
                        })
                });

            //act.
            var scenario = sut.ApplyExampleRow("existing example", 0);

            //assert.
            Assert.NotNull(scenario);
            Assert.Equal(sut.Name, scenario.Name);
            Assert.Equal(sut.Steps.Count(), scenario.Steps.Count());
            Assert.Equal(4, scenario.Steps.Count());

            var sutSteps = sut.Steps.ToList();
            var scenarioSteps = scenario.Steps.ToList();

            ValidateStep(scenarioSteps[0], "Given", "I chose 1 as first number", sutSteps[0]);
            ValidateStep(scenarioSteps[1], "And", "I chose 2 as second number", sutSteps[1]);
            ValidateStep(scenarioSteps[2], "When", "I press add", sutSteps[2]);
            ValidateStep(scenarioSteps[3], "Then", "the result should be 3 on the screen", sutSteps[3]);

            void ValidateStep(Gherkin.Ast.Step step, string keyword, string text,
                Gherkin.Ast.Step other)
            {
                Assert.NotSame(other, step);
                Assert.Equal(keyword, step.Keyword);
                Assert.Equal(text, step.Text);
            }
        }
    }
}
