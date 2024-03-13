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
                        null, 
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

            var expectedContent = "This DocString can contain values jUsTstring, 123, \"quoted string with spaces\", and some+math*for (/fun) ;=) in different forms: \"jUsTstring\", \"123\", \"\"quoted string with spaces\"\", \"some+math*for (/fun) ;=)\", also jUsTstring+123+\"quoted string with spaces\"+some+math*for (/fun) ;=)+jUsTstring or (jUsTstring)(123)(\"quoted string with spaces\")(some+math*for (/fun) ;=))...";
            Assert.Equal(expectedContent, docString.Content);
        }

        [Theory]
        [InlineData(
            "I declare: \"<a> > <b>\"",                       // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5 > 6\"",                           // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5 > -6\"",                         // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a>><b>\"",                         // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5>6\"",                             // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5>-6\"",                           // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a> ><b>\"",                        // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5 >6\"",                            // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5 >-6\"",                          // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a>> <b>\"",                        // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5> 6\"",                            // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5> -6\"",                          // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a> >= <b>\"",                      // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5 >= 6\"",                          // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5 >= -6\"",                        // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a>>=<b>\"",                        // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5>=6\"",                            // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5>=-6\"",                          // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a> >=<b>\"",                       // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5 >=6\"",                           // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5 >=-6\"",                         // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a>>= <b>\"",                       // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5>= 6\"",                           // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5>= -6\"",                         // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a> < <b>\"",                       // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5 < 6\"",                           // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5 < -6\"",                         // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a><<b>\"",                         // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5<6\"",                             // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5<-6\"",                           // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a> <<b>\"",                        // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5 <6\"",                            // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5 <-6\"",                          // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a>< <b>\"",                        // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5< 6\"",                            // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5< -6\"",                          // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a> <= <b>\"",                      // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5 <= 6\"",                          // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5 <= -6\"",                        // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a><=<b>\"",                        // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5<=6\"",                            // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5<=-6\"",                          // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a> <=<b>\"",                       // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5 <=6\"",                           // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5 <=-6\"",                         // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        [InlineData(
            "I declare: \"<a><= <b>\"",                       // givenExpression
            "the result for <a> and <b> should be <result>",  // thenExpression
            "I declare: \"5<= 6\"",                           // givenResult1
            "the result for 5 and 6 should be True",          // thenResult1
            "I declare: \"-5<= -6\"",                         // givenResult2
            "the result for -5 and -6 should be False")]      // thenResult2
        public void ApplyExampleRow_Digests_Row_Values_Into_Scenario_With_Operators_And_Spaces(
            string givenExpression,
            string thenExpression,
            string givenResult1,
            string thenResult1,
            string givenResult2,
            string thenResult2)
        {
            void ValidateStep(Gherkin.Ast.Step step, string keyword, string text, Gherkin.Ast.Step other, bool shouldSubstitute)
            {
                Assert.NotSame(other, step);
                Assert.Equal(shouldSubstitute, other.Text != step.Text);

                Assert.Equal(keyword, step.Keyword);
                Assert.Equal(text, step.Text);
            }

            void ValidateScenario(Gherkin.Ast.Scenario scenario, Gherkin.Ast.ScenarioOutline outline, string[] stepTexts)
            {
                Assert.NotNull(scenario);
                Assert.Equal(outline.Name, scenario.Name);
                Assert.Equal(outline.Steps.Count(), scenario.Steps.Count());
                Assert.Equal(3, scenario.Steps.Count());

                var sutSteps = outline.Steps.ToList();
                var scenarioSteps = scenario.Steps.ToList();

                ValidateStep(scenarioSteps[0], "Given", stepTexts[0], sutSteps[0], true);
                ValidateStep(scenarioSteps[1], "When", stepTexts[1], sutSteps[1], false);
                ValidateStep(scenarioSteps[2], "Then", stepTexts[2], sutSteps[2], true);
            }

            //arrange.
            var sut = new Gherkin.Ast.ScenarioOutline(
                null,
                null,
                null,
                "outline123",
                null,
                new Gherkin.Ast.Step[]
                {
                    new Gherkin.Ast.Step(null, "Given", givenExpression, null),
                    new Gherkin.Ast.Step(null, "When", "I press check", null),
                    new Gherkin.Ast.Step(null, "Then", thenExpression, null),
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
                            new Gherkin.Ast.TableCell(null, "result"),
                        }),
                        new Gherkin.Ast.TableRow[]
                        {
                            new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                            {
                                new Gherkin.Ast.TableCell(null, "5"),
                                new Gherkin.Ast.TableCell(null, "6"),
                                new Gherkin.Ast.TableCell(null, "True"),
                            }),
                            new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                            {
                                new Gherkin.Ast.TableCell(null, "-5"),
                                new Gherkin.Ast.TableCell(null, "-6"),
                                new Gherkin.Ast.TableCell(null, "False"),
                            }),
                        })
                });

            //act.
            var examplesScenario0 = sut.ApplyExampleRow("existing example", 0);
            var examplesScenario1 = sut.ApplyExampleRow("existing example", 1);

            //assert.
            ValidateScenario(
                examplesScenario0,
                sut,
                new string[]
                {
                    givenResult1,
                    "I press check",
                    thenResult1,
                });

            ValidateScenario(
                examplesScenario1,
                sut,
                new string[]
                {
                    givenResult2,
                    "I press check",
                    thenResult2,
                });
    }

        [Fact]
        public void ApplyExampleRow_Digests_Row_Values_Into_Scenario_With_Spaces_InTheMiddle_And_CaseSensitive_And_DoubleAngleBrackets_AndQuotes()
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
                    new Gherkin.Ast.Step(null, "Given", "<person's nAme> compares <a> with <b with spaces> via operator \"<\": \"<a> < <b with spaces>\"", null),
                    new Gherkin.Ast.Step(null, "When", "<a> is being compared (not using > operator!) to <b with spaces>>", null),
                    new Gherkin.Ast.Step(null, "Then", "result is <A>, <person's nAme> is <quoted |person\"s^ impression> and both <<a>> and <<b with spaces>> are correctly parsed", null),
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
                            new Gherkin.Ast.TableCell(null, "person's nAme"),
                            new Gherkin.Ast.TableCell(null, "a"),
                            new Gherkin.Ast.TableCell(null, "b with spaces"),
                            new Gherkin.Ast.TableCell(null, "A"),
                            new Gherkin.Ast.TableCell(null, "quoted |person\"s^ impression"),
                        }),
                        new Gherkin.Ast.TableRow[]
                        {
                            new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                            {
                                new Gherkin.Ast.TableCell(null, "Johnny"),
                                new Gherkin.Ast.TableCell(null, "84.444512"),
                                new Gherkin.Ast.TableCell(null, "100 000 000"),
                                new Gherkin.Ast.TableCell(null, "T R U E!"),
                                new Gherkin.Ast.TableCell(null, "\"happy^| =)\""),
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

            ValidateStep(scenarioSteps[0], "Given", "Johnny compares 84.444512 with 100 000 000 via operator \"<\": \"84.444512 < 100 000 000\"", sutSteps[0]);
            ValidateStep(scenarioSteps[1], "When", "84.444512 is being compared (not using > operator!) to 100 000 000>", sutSteps[1]);
            ValidateStep(scenarioSteps[2], "Then", "result is T R U E!, Johnny is \"happy^| =)\" and both <84.444512> and <100 000 000> are correctly parsed", sutSteps[2]);

            void ValidateStep(Gherkin.Ast.Step step, string keyword, string text, Gherkin.Ast.Step other)
            {
                Assert.NotSame(other, step);
                Assert.NotSame(other.Text, step.Text);

                Assert.Equal(keyword, step.Keyword);
                Assert.Equal(text, step.Text);
            }
        }

        [Theory]
        [InlineData("space inbetween", "value is <space inbetween>", "value is 1")]
        [InlineData("spaces in between", "<spaces in between>value is <spaces in between>", "1value is 1")]
        [InlineData("special^cHaRaCtErS*BetWeeN^OR|and&plus+minus-mult*div/escape\\quote'end", "value is <special^cHaRaCtErS*BetWeeN^OR|and&plus+minus-mult*div/escape\\quote'end>", "value is 1")]
        [InlineData("a^a", "value is <a^a>", "value is 1")]
        [InlineData("a", "value is <a<a>", "value is <a1")]
        [InlineData("a", "value is <a>a>", "value is 1a>")]
        [InlineData("a|^|a", "value is <a|^|a>", "value is 1")]
        [InlineData("a", "value is <a> <---> <a> <-> <a> < <a> > <a>", "value is 1 <---> 1 <-> 1 < 1 > 1")]
        public void ApplyExampleRow_Digests_Row_Values_Into_Scenario_Correctly_Steps_DataTables_DocStrings(
            string example, string inputSentence, string expectedResultSentence)
        {
            //arrange.
            var column0 = "Column1" + example;
            var column1 = "Column2" + example;
            var column2 = "Column3<" + example + ">";
            var cell0 = "<" + example + ">";
            var expectedResultCell0 = "1";
            var cell1 = "data with <" + example + "> in the middle";
            var expectedResultCell1 = "data with 1 in the middle";
            var cell2 = "data with " + example + " in the middle";
            var expectedResultCell2 = cell2;
            
            var docStringText = "DocString with " + inputSentence + " based on " + example;
            var expectedDocStringContent = "DocString with " + expectedResultSentence + " based on " + example;

            var sut = new Gherkin.Ast.ScenarioOutline(
                null,
                null,
                null,
                "outline123",
                null,
                new Gherkin.Ast.Step[] 
                {
                    new Gherkin.Ast.Step(null, "Given", inputSentence, null),
                    new Gherkin.Ast.Step(null, "Given", inputSentence, new DataTable(new []
                    {
                        new TableRow(null, new[] { new TableCell(null, column0), new TableCell(null, column1), new TableCell(null, column2) }),
                        new TableRow(null, new[] { new TableCell(null, cell0), new TableCell(null, cell1), new TableCell(null, cell2) }),
                    })),
                    new Gherkin.Ast.Step(null, "Given", inputSentence, new DocString(null, "type", docStringText)),
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
                            new Gherkin.Ast.TableCell(null, example)
                        }),
                        new Gherkin.Ast.TableRow[]
                        {
                            new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                            {
                                new Gherkin.Ast.TableCell(null, "1")
                            })
                        })
                });

            //act.
            var scenario = sut.ApplyExampleRow("existing example", 0);

            //assert.
            Assert.NotNull(scenario);
            Assert.Equal(sut.Name, scenario.Name);

            var scenarioSteps = scenario.Steps.ToList();
            var sutSteps = sut.Steps.ToList();
            
            Assert.Equal(sutSteps.Count, scenarioSteps.Count);
            Assert.Equal(3, scenarioSteps.Count);

            // Check regular text
            Assert.NotEqual(sutSteps[0].Text, scenarioSteps[0].Text);
            Assert.Equal(scenarioSteps[0].Text, expectedResultSentence);

            // Check DataTable
            Assert.IsType<DataTable>(scenarioSteps[1].Argument);
            var dataTable = (DataTable)scenarioSteps[1].Argument;
            var rows = dataTable.Rows.ToList();
            Assert.Equal(2, rows.Count());

            var row0 = rows[0].Cells.ToArray();
            Assert.Equal(3, row0.Count());
            var row1 = rows[1].Cells.ToArray();
            Assert.Equal(3, row1.Count());

            Assert.Equal(row0[0].Value, column0);
            Assert.Equal(row0[1].Value, column1);
            Assert.Equal(row0[2].Value, column2);
            Assert.Equal(row1[0].Value, expectedResultCell0);
            Assert.Equal(row1[1].Value, expectedResultCell1);
            Assert.Equal(row1[2].Value, expectedResultCell2);

            // Check DocString
            Assert.IsType<DocString>(scenarioSteps[2].Argument);
            var docString = (DocString)scenarioSteps[2].Argument;

            Assert.NotEqual(docStringText, docString.Content);
            Assert.Equal(expectedDocStringContent, docString.Content);
        }

        [Theory]
        [InlineData("a<a", "value is <a<a>", true, false, false)]
        [InlineData("a<a", "value is <a<a>", false, true, false)]
        [InlineData("a<a", "value is <a<a>", false, false, true)]
        [InlineData("a<a", "value is <a<a>", true, true, false)]
        [InlineData("a<a", "value is <a<a>", false, true, true)]
        [InlineData("a<a", "value is <a<a>", true, false, true)]
        [InlineData("a<a", "value is <a<a>", true, true, true)]
        [InlineData("a>a", "value is <a>a>", true, false, false)]
        [InlineData("a>a", "value is <a>a>", false, true, false)]
        [InlineData("a>a", "value is <a>a>", false, false, true)]
        [InlineData("a>a", "value is <a>a>", true, true, false)]
        [InlineData("a>a", "value is <a>a>", false, true, true)]
        [InlineData("a>a", "value is <a>a>", true, false, true)]
        [InlineData("a>a", "value is <a>a>", true, true, true)]
        public void DontApplyExampleRow_And_Throw_Cant_Substite_Wrong_Example_Parameter(
            string example, string inputSentence, bool parametrizeStep, bool parametrizeDataTable, bool parametrizeDocString)
        {
            //arrange.
            string normalText = "normaltext";

            var sut = new Gherkin.Ast.ScenarioOutline(
                null,
                null,
                null,
                "outline123",
                null,
                new Gherkin.Ast.Step[] 
                {
                    new Gherkin.Ast.Step(null, "Given", parametrizeStep ? inputSentence : normalText, null),
                    new Gherkin.Ast.Step(null, "Given", normalText, new DataTable(new []
                    {
                        new TableRow(null, new[] { new TableCell(null, "Column") }),
                        new TableRow(null, new[] { new TableCell(null, parametrizeDataTable ? inputSentence : normalText)}),
                    })),
                    new Gherkin.Ast.Step(null, "Given", normalText, new DocString(
                        null, "type", parametrizeDocString ? inputSentence : normalText)),
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
                            new Gherkin.Ast.TableCell(null, example)
                        }),
                        new Gherkin.Ast.TableRow[]
                        {
                            new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                            {
                                new Gherkin.Ast.TableCell(null, "1")
                            })
                        })
                });

            //act / assert.
            var exception = Assert.Throws<InvalidOperationException>(() => sut.ApplyExampleRow("existing example", 0));
            Assert.Contains("Examples table did not provide value for `a`", exception.Message);
        }

        [Theory]
        [InlineData(" initiallyspaced", "value is < initiallyspaced>")]
        [InlineData("spacedinthened ", "value is <spacedinthened >")]
        [InlineData(" spaced everywhere ", "value is < spaced everywhere >")]
        [InlineData("^initiallywrong", "value is <^initiallywrong>")]
        [InlineData(">initiallywrong", "value is <>initiallywrong>")]
        [InlineData("!initiallywrong", "value is <!initiallywrong>")]
        [InlineData("wronginthened^", "value is <wronginthened^>")]
        [InlineData("wronginthened<", "value is <wronginthened<>")]
        [InlineData("wronginthened!", "value is <wronginthened!>")]
        [InlineData("!wrong everywhere!", "value is <!wrong everywhere!>")]
        [InlineData(">wrong everywhere<", "value is <>!wrong everywhere!<>")]
        [InlineData("", "value is <>")]
        [InlineData(" ", "value is < >")]
        [InlineData("  ", "value is <  >")]
        [InlineData("   ", "value is <   >")]
        [InlineData("<", "value is <<>")]
        [InlineData(">", "value is <>>")]
        [InlineData("^", "value is <^>")]
        [InlineData("a<", "value is <a<>")]
        [InlineData("a^", "value is <a^>")]
        [InlineData("a<>a", "value is <a<>a>")]
        [InlineData("a<|^|>a", "value is <a<|^|>a>")]
        public void DontApplyExampleRow_Starts_With_Invalid_Character(string example, string inputSentence)
        {
            //arrange.
            var column0 = "Column1" + example;
            var column1 = "Column2" + example;
            var column2 = "Column3<" + example + ">";
            var cell0 = "<" + example + ">";
            var cell1 = "data with <" + example + "> in the middle";
            var cell2 = "data with " + example + " in the middle";

            var docStringText = "DocString with " + inputSentence + " based on " + example;

            var sut = new Gherkin.Ast.ScenarioOutline(
                null,
                null,
                null,
                "outline123",
                null,
                new Gherkin.Ast.Step[] 
                {
                    new Gherkin.Ast.Step(null, "Given", inputSentence, null),
                    new Gherkin.Ast.Step(null, "Given", inputSentence, new DataTable(new []
                    {
                        new TableRow(null, new[] { new TableCell(null, column0), new TableCell(null, column1), new TableCell(null, column2) }),
                        new TableRow(null, new[] { new TableCell(null, cell0), new TableCell(null, cell1), new TableCell(null, cell2) }),
                    })),
                    new Gherkin.Ast.Step(null, "Given", inputSentence, new DocString(null, "type", docStringText)),
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
                            new Gherkin.Ast.TableCell(null, example)
                        }),
                        new Gherkin.Ast.TableRow[]
                        {
                            new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                            {
                                new Gherkin.Ast.TableCell(null, "1")
                            })
                        })
                });

            //act.
            var scenario = sut.ApplyExampleRow("existing example", 0);

            //assert.
            Assert.NotNull(scenario);
            Assert.Equal(sut.Name, scenario.Name);

            var scenarioSteps = scenario.Steps.ToList();
            var sutSteps = sut.Steps.ToList();
            
            Assert.Equal(sutSteps.Count, scenarioSteps.Count);
            Assert.Equal(3, scenarioSteps.Count);

            // Check regular text: no substitutions!
            Assert.Same(sutSteps[0].Text, scenarioSteps[0].Text);

            // Check DataTable: no substitutions!
            Assert.IsType<DataTable>(scenarioSteps[1].Argument);
            var dataTable = (DataTable)scenarioSteps[1].Argument;
            var rows = dataTable.Rows.ToList();
            Assert.Equal(2, rows.Count());

            var row0 = rows[0].Cells.ToArray();
            Assert.Equal(3, row0.Count());
            var row1 = rows[1].Cells.ToArray();
            Assert.Equal(3, row1.Count());

            Assert.Equal(row0[0].Value, column0);
            Assert.Equal(row0[1].Value, column1);
            Assert.Equal(row0[2].Value, column2);
            Assert.Equal(row1[0].Value, cell0);
            Assert.Equal(row1[1].Value, cell1);
            Assert.Equal(row1[2].Value, cell2);

            // Check DocString: no substitutions!
            Assert.IsType<DocString>(scenarioSteps[2].Argument);
            var docString = (DocString)scenarioSteps[2].Argument;

            Assert.Equal(docStringText, docString.Content);
        }
    }
}
