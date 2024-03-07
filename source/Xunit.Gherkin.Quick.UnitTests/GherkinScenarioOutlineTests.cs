using Gherkin.Ast;
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

        [Fact]
        public void ApplyExampleRow_Digests_Row_Values_Into_Scenario_With_Multiple_Placeholders_Per_Step()
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
                    new Gherkin.Ast.Step(null, "Given", "I chose <a> as first number and <b> as second number", null),
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
            Assert.Equal(3, scenario.Steps.Count());

            var sutSteps = sut.Steps.ToList();
            var scenarioSteps = scenario.Steps.ToList();

            ValidateStep(scenarioSteps[0], "Given", "I chose 1 as first number and 2 as second number", sutSteps[0]);
            ValidateStep(scenarioSteps[1], "When", "I press add", sutSteps[1]);
            ValidateStep(scenarioSteps[2], "Then", "the result should be 3 on the screen", sutSteps[2]);

            void ValidateStep(Gherkin.Ast.Step step, string keyword, string text,
                Gherkin.Ast.Step other)
            {
                Assert.NotSame(other, step);
                Assert.Equal(keyword, step.Keyword);
                Assert.Equal(text, step.Text);
            }
        }

        [Fact]
        public void ApplyExampleRow_Digests_Row_Values_Into_Scenario_With_DataTable_In_Step()
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
                    new Gherkin.Ast.Step(null, "Given", "I pass a datatable with tokens", new DataTable(new []
                    {
                        new TableRow(null, new[] { new TableCell(null, "Column1"), new TableCell(null, "Column2") }),
                        new TableRow(null, new[] { new TableCell(null, "<a>"), new TableCell(null, "data with <b> in the middle") }),
                        new TableRow(null, new[] { new TableCell(null, "<b>"), new TableCell(null, "<a>") })
                    })),
                    new Gherkin.Ast.Step(null, "When", "I apply a sample row", null),
                    new Gherkin.Ast.Step(null, "Then", "the data table should be populated", null),
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
                        }),
                        new Gherkin.Ast.TableRow[]
                        {
                            new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                            {
                                new Gherkin.Ast.TableCell(null, "1"),
                                new Gherkin.Ast.TableCell(null, "2")
                            })
                        })
                });

            //act.
            var scenario = sut.ApplyExampleRow("existing example", 0);

            //assert.
            Assert.NotNull(scenario);
            Assert.Equal(sut.Name, scenario.Name);
            Assert.Equal(sut.Steps.Count(), scenario.Steps.Count());
            Assert.Equal(3, scenario.Steps.Count());

            var scenarioSteps = scenario.Steps.ToList();

            Assert.IsType<DataTable>(scenarioSteps[0].Argument);
            var dataTable = (DataTable)scenarioSteps[0].Argument;
            var rows = dataTable.Rows.ToList();

            ValidateRow(rows[0], "Column1", "Column2");
            ValidateRow(rows[1], "1", "data with 2 in the middle");
            ValidateRow(rows[2], "2", "1");

            void ValidateRow(TableRow row, string expectedColumn0Value, string expectedColumn1Value)
            {
                var cells = row.Cells.ToArray();
                Assert.Equal(cells[0].Value, expectedColumn0Value);
                Assert.Equal(cells[1].Value, expectedColumn1Value);
            }
        }

        [Fact]
        public void ApplyExampleRow_Digests_Row_Values_Into_Scenario_With_DocString_In_Step()
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
                    new Gherkin.Ast.Step(null, "Given", "I pass a docstring with tokens", new DocString(
                        new Location(0, 0), 
                        "type", 
                        "This DocString can contain values <a>, <b>, <c>, and <d> in different forms: \"<a>\", \"<b>\", \"<c>\", \"<d>\", also <a>+<b>+<c>+<d>+<a> or (<a>)(<b>)(<c>)(<d>)...")),
                    new Gherkin.Ast.Step(null, "When", "I apply a sample row", null),
                    new Gherkin.Ast.Step(null, "Then", "the correct docstring should be created", null),
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
                            new Gherkin.Ast.TableCell(null, "c"),
                            new Gherkin.Ast.TableCell(null, "d"),
                        }),
                        new Gherkin.Ast.TableRow[]
                        {
                            new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                            {
                                new Gherkin.Ast.TableCell(null, "jUsTstring"),
                                new Gherkin.Ast.TableCell(null, "123"),
                                new Gherkin.Ast.TableCell(null, "\"quoted string with spaces\""),
                                new Gherkin.Ast.TableCell(null, "some+math*for (/fun) ;=)"),
                            })
                        })
                });

            //act.
            var scenario = sut.ApplyExampleRow("existing example", 0);

            //assert.
            Assert.NotNull(scenario);
            Assert.Equal(sut.Name, scenario.Name);
            Assert.Equal(sut.Steps.Count(), scenario.Steps.Count());
            Assert.Equal(3, scenario.Steps.Count());

            var scenarioSteps = scenario.Steps.ToList();

            Assert.IsType<DocString>(scenarioSteps[0].Argument);
            var docString = (DocString)scenarioSteps[0].Argument;
            var content = docString.Content;

            System.Console.WriteLine(content);

            var expectedContent = "This DocString can contain values jUsTstring, 123, \"quoted string with spaces\", and some+math*for (/fun) ;=) in different forms: \"jUsTstring\", \"123\", \"\"quoted string with spaces\"\", \"some+math*for (/fun) ;=)\", also jUsTstring+123+\"quoted string with spaces\"+some+math*for (/fun) ;=)+jUsTstring or (jUsTstring)(123)(\"quoted string with spaces\")(some+math*for (/fun) ;=))...";
            Assert.Equal(expectedContent, docString.Content);
        }
    }
}
