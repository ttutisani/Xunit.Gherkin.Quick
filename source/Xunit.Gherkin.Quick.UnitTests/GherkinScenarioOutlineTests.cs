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

            ValidateStep(scenarioSteps[0], sutSteps[0], "Given", "I chose 1 as first number");
            ValidateStep(scenarioSteps[1], sutSteps[1], "And", "I chose 2 as second number");
            ValidateStep(scenarioSteps[2], sutSteps[2], "When", "I press add");
            ValidateStep(scenarioSteps[3], sutSteps[3], "Then", "the result should be 3 on the screen");

            void ValidateStep(
                Gherkin.Ast.Step actualStepWithSubstitutions, 
                Gherkin.Ast.Step initialStep,
                string expectedKeyword,
                string expectedText)
            {
                Assert.NotSame(initialStep, actualStepWithSubstitutions);
                Assert.Equal(expectedKeyword, actualStepWithSubstitutions.Keyword);
                Assert.Equal(expectedText, actualStepWithSubstitutions.Text);
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

            ValidateStep(scenarioSteps[0], sutSteps[0], "Given", "I chose 1 as first number and 2 as second number");
            ValidateStep(scenarioSteps[1], sutSteps[1], "When", "I press add");
            ValidateStep(scenarioSteps[2], sutSteps[2], "Then", "the result should be 3 on the screen");

            void ValidateStep(
                Gherkin.Ast.Step actualStepWithSubstitutions, 
                Gherkin.Ast.Step initialStep,
                string expectedKeyword,
                string expectedText)
            {
                Assert.NotSame(initialStep, actualStepWithSubstitutions);
                Assert.Equal(expectedKeyword, actualStepWithSubstitutions.Keyword);
                Assert.Equal(expectedText, actualStepWithSubstitutions.Text);
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

            void ValidateRow(TableRow actualRow, string expectedColumn0Value, string expectedColumn1Value)
            {
                var actualCells = actualRow.Cells.ToArray();
                Assert.Equal(actualCells[0].Value, expectedColumn0Value);
                Assert.Equal(actualCells[1].Value, expectedColumn1Value);
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
            "I declare: \"<a> > <b>\"",                       // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5 > 6\"",                           // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5 > -6\"",                         // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a>><b>\"",                         // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5>6\"",                             // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5>-6\"",                           // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a> ><b>\"",                        // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5 >6\"",                            // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5 >-6\"",                          // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a>> <b>\"",                        // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5> 6\"",                            // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5> -6\"",                          // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a> >= <b>\"",                      // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5 >= 6\"",                          // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5 >= -6\"",                        // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a>>=<b>\"",                        // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5>=6\"",                            // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5>=-6\"",                          // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a> >=<b>\"",                       // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5 >=6\"",                           // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5 >=-6\"",                         // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a>>= <b>\"",                       // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5>= 6\"",                           // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5>= -6\"",                         // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a> < <b>\"",                       // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5 < 6\"",                           // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5 < -6\"",                         // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a><<b>\"",                         // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5<6\"",                             // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5<-6\"",                           // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a> <<b>\"",                        // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5 <6\"",                            // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5 <-6\"",                          // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a>< <b>\"",                        // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5< 6\"",                            // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5< -6\"",                          // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a> <= <b>\"",                      // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5 <= 6\"",                          // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5 <= -6\"",                        // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a><=<b>\"",                        // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5<=6\"",                            // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5<=-6\"",                          // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a> <=<b>\"",                       // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5 <=6\"",                           // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5 <=-6\"",                         // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        [InlineData(
            "I declare: \"<a><= <b>\"",                       // initialGivenExpression
            "the result for <a> and <b> should be <result>",  // initialThenExpression
            "I declare: \"5<= 6\"",                           // expectedGivenExpressionWithExampleRow0
            "the result for 5 and 6 should be True",          // expectedThenExpressionWithExampleRow0
            "I declare: \"-5<= -6\"",                         // expectedGivenExpressionWithExampleRow1
            "the result for -5 and -6 should be False")]      // expectedThenExpressionWithExampleRow1
        public void ApplyExampleRow_Digests_Row_Values_Into_Scenario_With_Operators_And_Spaces(
            string initialGivenExpression,
            string initialThenExpression,
            string expectedGivenExpressionWithExampleRow0,
            string expectedThenExpressionWithExampleRow0,
            string expectedGivenExpressionWithExampleRow1,
            string expectedThenExpressionWithExampleRow1)
        {
            void ValidateStep(
                Gherkin.Ast.Step actualStep, 
                Gherkin.Ast.Step initialStep,
                string expectedKeyword,
                string expectedText,
                bool shouldSubstituteByExamplesValues)
            {
                Assert.NotSame(initialStep, actualStep);
                Assert.Equal(shouldSubstituteByExamplesValues, initialStep.Text != actualStep.Text);

                Assert.Equal(expectedKeyword, actualStep.Keyword);
                Assert.Equal(expectedText, actualStep.Text);
            }

            void ValidateScenario(
                Gherkin.Ast.Scenario actualScenario, 
                Gherkin.Ast.ScenarioOutline initialScenarioOutline, 
                string[] expectedStepTexts)
            {
                Assert.NotNull(actualScenario);
                Assert.Equal(initialScenarioOutline.Name, actualScenario.Name);
                Assert.Equal(initialScenarioOutline.Steps.Count(), actualScenario.Steps.Count());
                Assert.Equal(3, actualScenario.Steps.Count());

                var initialScenarioOutlineSteps = initialScenarioOutline.Steps.ToList();
                var actualScenarioSteps = actualScenario.Steps.ToList();

                ValidateStep(actualScenarioSteps[0], initialScenarioOutlineSteps[0], "Given", expectedStepTexts[0], true);
                ValidateStep(actualScenarioSteps[1], initialScenarioOutlineSteps[1], "When", expectedStepTexts[1], false);
                ValidateStep(actualScenarioSteps[2], initialScenarioOutlineSteps[2], "Then", expectedStepTexts[2], true);
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
                    new Gherkin.Ast.Step(null, "Given", initialGivenExpression, null),
                    new Gherkin.Ast.Step(null, "When", "I press check", null),
                    new Gherkin.Ast.Step(null, "Then", initialThenExpression, null),
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
                    expectedGivenExpressionWithExampleRow0,
                    "I press check",
                    expectedThenExpressionWithExampleRow0,
                });

            ValidateScenario(
                examplesScenario1,
                sut,
                new string[]
                {
                    expectedGivenExpressionWithExampleRow1,
                    "I press check",
                    expectedThenExpressionWithExampleRow1,
                });
        }

        [Fact]
        public void ApplyExampleRow_Digests_Row_Values_Into_Scenario_With_Special_Chars_In_The_Middle_Multiple_Steps()
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

            ValidateStep(scenarioSteps[0], sutSteps[0], "Given", "Johnny compares 84.444512 with 100 000 000 via operator \"<\": \"84.444512 < 100 000 000\"");
            ValidateStep(scenarioSteps[1], sutSteps[1], "When", "84.444512 is being compared (not using > operator!) to 100 000 000>");
            ValidateStep(scenarioSteps[2], sutSteps[2], "Then", "result is T R U E!, Johnny is \"happy^| =)\" and both <84.444512> and <100 000 000> are correctly parsed");

            void ValidateStep(                
                Gherkin.Ast.Step actualStepWithSubstitutions, 
                Gherkin.Ast.Step initialStep,
                string expectedKeyword,
                string expectedText)
            {
                Assert.NotSame(initialStep, actualStepWithSubstitutions);
                Assert.NotSame(initialStep.Text, actualStepWithSubstitutions.Text);

                Assert.Equal(expectedKeyword, actualStepWithSubstitutions.Keyword);
                Assert.Equal(expectedText, actualStepWithSubstitutions.Text);
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
            string example, string initialSentence, string expectedResultSentence)
        {
            //arrange.
            var initialColumnName0 = "Column1" + example;
            var initialColumnName1 = "Column2" + example;
            var initialColumnName2 = "Column3<" + example + ">";
            var initialCellValue0 = "<" + example + ">";
            var expectedResultCellValue0 = "1";
            var initialCellValue1 = "data with <" + example + "> in the middle";
            var expectedResultCellValue1 = "data with 1 in the middle";
            var initialCellValue2 = "data with " + example + " in the middle";
            var expectedResultCellValue2 = initialCellValue2;
            
            var initialDocStringContent = "DocString with " + initialSentence + " based on " + example;
            var expectedResultDocStringContent = "DocString with " + expectedResultSentence + " based on " + example;

            var sut = new Gherkin.Ast.ScenarioOutline(
                null,
                null,
                null,
                "outline123",
                null,
                new Gherkin.Ast.Step[] 
                {
                    new Gherkin.Ast.Step(null, "Given", initialSentence, null),
                    new Gherkin.Ast.Step(null, "Given", initialSentence, new DataTable(new []
                    {
                        new TableRow(null, new[] { new TableCell(null, initialColumnName0), new TableCell(null, initialColumnName1), new TableCell(null, initialColumnName2) }),
                        new TableRow(null, new[] { new TableCell(null, initialCellValue0), new TableCell(null, initialCellValue1), new TableCell(null, initialCellValue2) }),
                    })),
                    new Gherkin.Ast.Step(null, "Given", initialSentence, new DocString(null, "type", initialDocStringContent)),
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
            var actualDataTable = (DataTable)scenarioSteps[1].Argument;
            var actualRows = actualDataTable.Rows.ToList();
            Assert.Equal(2, actualRows.Count());

            var actualRow0 = actualRows[0].Cells.ToArray();
            Assert.Equal(3, actualRow0.Count());
            var actualRow1 = actualRows[1].Cells.ToArray();
            Assert.Equal(3, actualRow1.Count());

            Assert.Equal(actualRow0[0].Value, initialColumnName0);
            Assert.Equal(actualRow0[1].Value, initialColumnName1);
            Assert.Equal(actualRow0[2].Value, initialColumnName2);
            Assert.Equal(actualRow1[0].Value, expectedResultCellValue0);
            Assert.Equal(actualRow1[1].Value, expectedResultCellValue1);
            Assert.Equal(actualRow1[2].Value, expectedResultCellValue2);

            // Check DocString
            Assert.IsType<DocString>(scenarioSteps[2].Argument);
            var docString = (DocString)scenarioSteps[2].Argument;

            Assert.NotEqual(initialDocStringContent, docString.Content);
            Assert.Equal(expectedResultDocStringContent, docString.Content);
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
            string example, string initialSentence, bool parametrizeStep, bool parametrizeDataTable, bool parametrizeDocString)
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
                    new Gherkin.Ast.Step(null, "Given", parametrizeStep ? initialSentence : normalText, null),
                    new Gherkin.Ast.Step(null, "Given", normalText, new DataTable(new []
                    {
                        new TableRow(null, new[] { new TableCell(null, "Column") }),
                        new TableRow(null, new[] { new TableCell(null, parametrizeDataTable ? initialSentence : normalText)}),
                    })),
                    new Gherkin.Ast.Step(null, "Given", normalText, new DocString(
                        null, "type", parametrizeDocString ? initialSentence : normalText)),
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
            var actualException = Assert.Throws<InvalidOperationException>(() => sut.ApplyExampleRow("existing example", 0));
            Assert.Contains("Examples table did not provide value for `a`", actualException.Message);
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
        public void DontApplyExampleRow_Starts_With_Invalid_Character(string example, string initialSentence)
        {
            //arrange.
            var initialColumnName0 = "Column1" + example;
            var initialColumnName1 = "Column2" + example;
            var initialColumnName2 = "Column3<" + example + ">";
            var initialCellValue0 = "<" + example + ">";
            var initialCellValue1 = "data with <" + example + "> in the middle";
            var initialCellValue2 = "data with " + example + " in the middle";

            var initialDocStringContent = "DocString with " + initialSentence + " based on " + example;

            var sut = new Gherkin.Ast.ScenarioOutline(
                null,
                null,
                null,
                "outline123",
                null,
                new Gherkin.Ast.Step[] 
                {
                    new Gherkin.Ast.Step(null, "Given", initialSentence, null),
                    new Gherkin.Ast.Step(null, "Given", initialSentence, new DataTable(new []
                    {
                        new TableRow(null, new[] { new TableCell(null, initialColumnName0), new TableCell(null, initialColumnName1), new TableCell(null, initialColumnName2) }),
                        new TableRow(null, new[] { new TableCell(null, initialCellValue0), new TableCell(null, initialCellValue1), new TableCell(null, initialCellValue2) }),
                    })),
                    new Gherkin.Ast.Step(null, "Given", initialSentence, new DocString(null, "type", initialDocStringContent)),
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
            var actualDataTable = (DataTable)scenarioSteps[1].Argument;
            var actualRows = actualDataTable.Rows.ToList();
            Assert.Equal(2, actualRows.Count());

            var actualRow0 = actualRows[0].Cells.ToArray();
            Assert.Equal(3, actualRow0.Count());
            var actualRow1 = actualRows[1].Cells.ToArray();
            Assert.Equal(3, actualRow1.Count());

            Assert.Equal(actualRow0[0].Value, initialColumnName0);
            Assert.Equal(actualRow0[1].Value, initialColumnName1);
            Assert.Equal(actualRow0[2].Value, initialColumnName2);
            Assert.Equal(actualRow1[0].Value, initialCellValue0);
            Assert.Equal(actualRow1[1].Value, initialCellValue1);
            Assert.Equal(actualRow1[2].Value, initialCellValue2);

            // Check DocString: no substitutions!
            Assert.IsType<DocString>(scenarioSteps[2].Argument);
            var actualDocString = (DocString)scenarioSteps[2].Argument;

            Assert.Equal(initialDocStringContent, actualDocString.Content);
        }
    }
}
