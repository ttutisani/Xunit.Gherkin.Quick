using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class ScenarioOutlineExecutorTests
    {
        private readonly Mock<IFeatureFileRepository> _featureFileRepository = new Mock<IFeatureFileRepository>();
        private readonly ScenarioOutlineExecutor _sut;

        public ScenarioOutlineExecutorTests()
        {
            _sut = new ScenarioOutlineExecutor(_featureFileRepository.Object);
        }

        [Fact]
        public async Task ExecuteScenarioOutlineAsync_Requires_FeatureInstance()
        {
            //act / assert.
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ExecuteScenarioOutlineAsync(null, "scenario name", "example name", 0));
        }

        [Theory]
        [InlineData(null, "example name", 0, typeof(ArgumentNullException))]
        [InlineData("", "example name", 0, typeof(ArgumentNullException))]
        [InlineData("   ", "example name", 0, typeof(ArgumentNullException))]

        [InlineData("scenario name", "example name", -1, typeof(ArgumentException))]
        public async Task ExecuteScenarioOutlineAsync_Requires_Arguments(
            string scenarioOutlineName,
            string exampleName,
            int exampleRowIndex,
            Type expectedExceptionType)
        {
            //arrange.
            var featureInstance = new FeatureForNullArgumentTests();

            //act / assert.
            await Assert.ThrowsAsync(expectedExceptionType, async () => await _sut.ExecuteScenarioOutlineAsync(featureInstance, scenarioOutlineName, exampleName, exampleRowIndex));
        }

        private sealed class FeatureForNullArgumentTests : Feature
        { }

		[Fact]
		public async Task ScenarioOutline_Runs_Background_Steps_First()
		{
			var feature = new GherkinFeatureBuilder()
				.WithBackground(sb => sb
					.Given("background", null))
				.WithScenarioOutline("test outline", sb => sb
					.Given("I chose <a> as first number", null)
					.And("I chose <b> as second number", null)
					.When("I press add", null)
					.Then("the result should be <sum> on the screen", null),
					 eb => eb
						.WithExampleHeadings("a", "b", "sum")
						.WithExamples("", db => db.WithData("1", "2", "3")))
				.Build();
			
			_featureFileRepository.Setup(r => r.GetByFilePath($"{nameof(FeatureWithScenarioSteps)}.feature"))
				.Returns(new FeatureFile(new Gherkin.Ast.GherkinDocument(feature, new Gherkin.Ast.Comment[0])))
				.Verifiable();

			var featureInstance = new FeatureWithScenarioSteps();
			var output = new Mock<ITestOutputHelper>();
			featureInstance.InternalOutput = output.Object;
		
			await _sut.ExecuteScenarioOutlineAsync(featureInstance, "test outline", "", 0);

            _featureFileRepository.Verify();
			Assert.Equal(5, featureInstance.CallStack.Count);
			Assert.Equal(nameof(FeatureWithScenarioSteps.BackgroundStep), featureInstance.CallStack[0].Key);
		}

        [Theory]
        [InlineData("", 0, 0, 1, 1)]
        [InlineData("", 1, 1, 9, 10)]
        [InlineData("of bigger numbers", 0, 99, 1, 100)]
        [InlineData("of bigger numbers", 1, 100, 200, 300)]
        [InlineData("of large numbers", 0, 999, 1, 1000)]
        [InlineData("of large numbers", 1, 9999, 1, 10000)]
        public async Task ExecuteScenarioOutlineAsync_Executes_All_Scenario_Steps(
            string exampleName,
            int exampleRowIndex,

            int a,
            int b,
            int sum
            )
        {
            //arrange.
            var step1Text = "Given " + FeatureWithScenarioSteps.ScenarioStep1Text.Replace(@"(\d+)", $"{a}", StringComparison.InvariantCultureIgnoreCase);
            var step2Text = "And " + FeatureWithScenarioSteps.ScenarioStep2Text.Replace(@"(\d+)", $"{b}", StringComparison.InvariantCultureIgnoreCase);
            var step3Text = "When " + FeatureWithScenarioSteps.ScenarioStep3Text;
            var step4Text = "Then " + FeatureWithScenarioSteps.ScenarioStep4Text.Replace(@"(\d+)", $"{sum}", StringComparison.InvariantCultureIgnoreCase);

            var scenarioOutlineName = "scenario 12345";
            _featureFileRepository.Setup(r => r.GetByFilePath($"{nameof(FeatureWithScenarioSteps)}.feature"))
                .Returns(new FeatureFile(CreateGherkinDocument(scenarioOutlineName)))
                .Verifiable();

            var featureInstance = new FeatureWithScenarioSteps();
            var output = new Mock<ITestOutputHelper>();
            featureInstance.InternalOutput = output.Object;

            //act.
            await _sut.ExecuteScenarioOutlineAsync(featureInstance, scenarioOutlineName, exampleName, exampleRowIndex);

            //assert.
            _featureFileRepository.Verify();

            Assert.Equal(4, featureInstance.CallStack.Count);

            Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep1), featureInstance.CallStack[0].Key);
            Assert.NotNull(featureInstance.CallStack[0].Value);
            Assert.Single(featureInstance.CallStack[0].Value);
            Assert.Equal(a, featureInstance.CallStack[0].Value[0]);
            output.Verify(o => o.WriteLine($"{step1Text}: PASSED"), Times.Once);

            Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep2), featureInstance.CallStack[1].Key);
            Assert.NotNull(featureInstance.CallStack[1].Value);
            Assert.Single(featureInstance.CallStack[1].Value);
            Assert.Equal(b, featureInstance.CallStack[1].Value[0]);
            output.Verify(o => o.WriteLine($"{step2Text}: PASSED"), Times.Once);

            Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep3), featureInstance.CallStack[2].Key);
            Assert.Null(featureInstance.CallStack[2].Value);
            output.Verify(o => o.WriteLine($"{step3Text}: PASSED"), Times.Once);

            Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep4), featureInstance.CallStack[3].Key);
            Assert.NotNull(featureInstance.CallStack[3].Value);
            Assert.Single(featureInstance.CallStack[3].Value);
            Assert.Equal(sum, featureInstance.CallStack[3].Value[0]);
            output.Verify(o => o.WriteLine($"{step4Text}: PASSED"), Times.Once);
        }

        private static Gherkin.Ast.GherkinDocument CreateGherkinDocument(
            string outlineName,
            Gherkin.Ast.StepArgument stepArgument = null)
        {
			return new Gherkin.Ast.GherkinDocument(
				new GherkinFeatureBuilder()					
					.WithScenarioOutline(outlineName, sb => sb
						.Given("I chose <a> as first number", stepArgument)
						.And("I chose <b> as second number", stepArgument)
						.When("I press add", stepArgument)
						.Then("the result should be <sum> on the screen", stepArgument),
							eb => eb
							.WithExampleHeadings("a", "b", "sum")
							.WithExamples("", db => db
								.WithData(0, 1, 1)
								.WithData(1, 9, 10))
							.WithExamples("of bigger numbers", db => db
								.WithData(99, 1, 100)
								.WithData(100, 200, 300))
							.WithExamples("of large numbers", db => db
								.WithData(999, 1, 1000)
								.WithData(9999, 1, 10000)))
					.Build(),					
					new Gherkin.Ast.Comment[0]);
        }

        private sealed class FeatureWithScenarioSteps : Feature
        {
            public List<KeyValuePair<string, object[]>> CallStack { get; } = new List<KeyValuePair<string, object[]>>();

			[Given("background")]
			public void BackgroundStep()
			{
				CallStack.Add(new KeyValuePair<string, object[]>(nameof(BackgroundStep), null));
			}

            [Given("Non matching given")]
            public void NonMatchingStep1_before()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep1_before), null));
            }

            public const string ScenarioStep1Text = @"I chose (\d+) as first number";

            [Given(ScenarioStep1Text)]
            public void ScenarioStep1(int firstNumber)
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep1), new object[] { firstNumber }));
            }

            [Given("Non matching given")]
            public void NonMatchingStep1_after()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep1_after), null));
            }

            [And("Non matching and")]
            public void NonMatchingStep2_before()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep2_before), null));
            }

            public const string ScenarioStep2Text = @"I chose (\d+) as second number";

            [And(ScenarioStep2Text)]
            public void ScenarioStep2(int secondNumber)
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep2), new object[] { secondNumber }));
            }

            [And("Non matching and")]
            public void NonMatchingStep2_after()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep2_after), null));
            }

            [When("Non matching when")]
            public void NonMatchingStep3_before()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep3_before), null));
            }

            public const string ScenarioStep3Text = "I press add";

            [When(ScenarioStep3Text)]
            public void ScenarioStep3()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep3), null));
            }

            [When("Non matching when")]
            public void NonMatchingStep3_after()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep3_after), null));
            }

            [Then("Non matching then")]
            public void NonMatchingStep4_before()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep4_before), null));
            }

            public const string ScenarioStep4Text = @"the result should be (\d+) on the screen";

            [Then(ScenarioStep4Text)]
            public void ScenarioStep4(int result)
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep4), new object[] { result }));
            }

            [Then("Non matching then")]
            public void NonMatchingStep4_after()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep4_after), null));
            }
        }

        [Theory]
        [InlineData("", 0, 0, 1, 1)]
        [InlineData("", 1, 1, 9, 10)]
        [InlineData("of bigger numbers", 0, 99, 1, 100)]
        [InlineData("of bigger numbers", 1, 100, 200, 300)]
        [InlineData("of large numbers", 0, 999, 1, 1000)]
        [InlineData("of large numbers", 1, 9999, 1, 10000)]
        public async Task ExecuteScenarioOutlineAsync_Executes_Successful_Scenario_Steps_And_Skips_The_Rest(
            string exampleName,
            int exampleRowIndex,

            int a,
            int b,
            int sum
            )
        {
            //arrange.
            var step1Text = "Given " + FeatureWithScenarioSteps_And_Throwing.ScenarioStep1Text.Replace(@"(\d+)", $"{a}", StringComparison.InvariantCultureIgnoreCase);
            var step2Text = "And " + FeatureWithScenarioSteps_And_Throwing.ScenarioStep2Text.Replace(@"(\d+)", $"{b}", StringComparison.InvariantCultureIgnoreCase);
            var step3Text = "When " + FeatureWithScenarioSteps_And_Throwing.ScenarioStep3Text;
            var step4Text = "Then " + FeatureWithScenarioSteps_And_Throwing.ScenarioStep4Text.Replace(@"(\d+)", $"{sum}", StringComparison.InvariantCultureIgnoreCase);

            var outlineName = "scenario 12345";
            _featureFileRepository.Setup(r => r.GetByFilePath($"{nameof(FeatureWithScenarioSteps_And_Throwing)}.feature"))
                .Returns(new FeatureFile(CreateGherkinDocument(outlineName)))
                .Verifiable();

            var featureInstance = new FeatureWithScenarioSteps_And_Throwing();
            var output = new Mock<ITestOutputHelper>();
            featureInstance.InternalOutput = output.Object;

            //act.
            var exceptiion = await Assert.ThrowsAsync<TargetInvocationException>(async () => await _sut.ExecuteScenarioOutlineAsync(featureInstance, outlineName, exampleName, exampleRowIndex));
            Assert.IsType<InvalidOperationException>(exceptiion.InnerException);

            //assert.
            _featureFileRepository.Verify();

            Assert.Equal(2, featureInstance.CallStack.Count);

            Assert.Equal(nameof(FeatureWithScenarioSteps_And_Throwing.ScenarioStep1), featureInstance.CallStack[0].Key);
            Assert.NotNull(featureInstance.CallStack[0].Value);
            Assert.Single(featureInstance.CallStack[0].Value);
            Assert.Equal(a, featureInstance.CallStack[0].Value[0]);
            output.Verify(o => o.WriteLine($"{step1Text}: PASSED"), Times.Once);

            Assert.Equal(nameof(FeatureWithScenarioSteps_And_Throwing.ScenarioStep2), featureInstance.CallStack[1].Key);
            Assert.NotNull(featureInstance.CallStack[1].Value);
            Assert.Single(featureInstance.CallStack[1].Value);
            Assert.Equal(b, featureInstance.CallStack[1].Value[0]);
            output.Verify(o => o.WriteLine($"{step2Text}: FAILED"), Times.Once);

            output.Verify(o => o.WriteLine($"{step3Text}: SKIPPED"), Times.Once);

            output.Verify(o => o.WriteLine($"{step4Text}: SKIPPED"), Times.Once);
        }

        private sealed class FeatureWithScenarioSteps_And_Throwing : Feature
        {
            public List<KeyValuePair<string, object[]>> CallStack { get; } = new List<KeyValuePair<string, object[]>>();

            [Given("Non matching given")]
            public void NonMatchingStep1_before()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep1_before), null));
            }

            public const string ScenarioStep1Text = @"I chose (\d+) as first number";

            [Given(ScenarioStep1Text)]
            public void ScenarioStep1(int firstNumber)
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep1), new object[] { firstNumber }));
            }

            [Given("Non matching given")]
            public void NonMatchingStep1_after()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep1_after), null));
            }

            [And("Non matching and")]
            public void NonMatchingStep2_before()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep2_before), null));
            }

            public const string ScenarioStep2Text = @"I chose (\d+) as second number";

            [And(ScenarioStep2Text)]
            public void ScenarioStep2(int secondNumber)
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep2), new object[] { secondNumber }));

                throw new InvalidOperationException("Some exception to stop execution of next steps.");
            }

            [And("Non matching and")]
            public void NonMatchingStep2_after()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep2_after), null));
            }

            [When("Non matching when")]
            public void NonMatchingStep3_before()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep3_before), null));
            }

            public const string ScenarioStep3Text = "I press add";

            [When(ScenarioStep3Text)]
            public void ScenarioStep3()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep3), null));
            }

            [When("Non matching when")]
            public void NonMatchingStep3_after()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep3_after), null));
            }

            [Then("Non matching then")]
            public void NonMatchingStep4_before()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep4_before), null));
            }

            public const string ScenarioStep4Text = @"the result should be (\d+) on the screen";

            [Then(ScenarioStep4Text)]
            public void ScenarioStep4(int result)
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep4), new object[] { result }));
            }

            [Then("Non matching then")]
            public void NonMatchingStep4_after()
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep4_after), null));
            }
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("", 1)]
        [InlineData("of bigger numbers", 0)]
        [InlineData("of bigger numbers", 1)]
        [InlineData("of large numbers", 0)]
        [InlineData("of large numbers", 1)]
        public async Task ExecuteScenarioOutlineAsync_Executes_ScenarioStep_With_DataTable(
            string exampleName,
            int exampleRowIndex)
        {
            //arrange.
            var scenarioName = "scenario123";
            var featureInstance = new FeatureWithDataTableScenarioStep();
            var output = new Mock<ITestOutputHelper>();
            featureInstance.InternalOutput = output.Object;

            _featureFileRepository.Setup(r => r.GetByFilePath($"{nameof(FeatureWithDataTableScenarioStep)}.feature"))
                .Returns(new FeatureFile(FeatureWithDataTableScenarioStep.CreateGherkinDocument(scenarioName,
                    new Gherkin.Ast.DataTable(new Gherkin.Ast.TableRow[]
                    {
                        new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                        {
                            new Gherkin.Ast.TableCell(null, "First argument"),
                            new Gherkin.Ast.TableCell(null, "Second argument"),
                            new Gherkin.Ast.TableCell(null, "Result"),
                        }),
                        new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                        {
                            new Gherkin.Ast.TableCell(null, "1"),
                            new Gherkin.Ast.TableCell(null, "2"),
                            new Gherkin.Ast.TableCell(null, "3"),
                        }),
                        new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                        {
                            new Gherkin.Ast.TableCell(null, "a"),
                            new Gherkin.Ast.TableCell(null, "b"),
                            new Gherkin.Ast.TableCell(null, "c"),
                        })
                    }))))
                    .Verifiable();

            //act.
            await _sut.ExecuteScenarioOutlineAsync(featureInstance, scenarioName, exampleName, exampleRowIndex);

            //assert.
            _featureFileRepository.Verify();

            Assert.NotNull(featureInstance.ReceivedDataTable);
            Assert.Equal(3, featureInstance.ReceivedDataTable.Rows.Count());

            AssertDataTableCell(0, 0, "First argument");
            AssertDataTableCell(0, 1, "Second argument");
            AssertDataTableCell(0, 2, "Result");

            AssertDataTableCell(1, 0, "1");
            AssertDataTableCell(1, 1, "2");
            AssertDataTableCell(1, 2, "3");

            AssertDataTableCell(2, 0, "a");
            AssertDataTableCell(2, 1, "b");
            AssertDataTableCell(2, 2, "c");

            void AssertDataTableCell(int rowIndex, int cellIndex, string value)
            {
                Assert.True(featureInstance.ReceivedDataTable.Rows.Count() > rowIndex);
                Assert.NotNull(featureInstance.ReceivedDataTable.Rows.ElementAt(rowIndex));
                Assert.True(featureInstance.ReceivedDataTable.Rows.ElementAt(rowIndex).Cells.Count() > cellIndex);
                Assert.NotNull(featureInstance.ReceivedDataTable.Rows.ElementAt(rowIndex).Cells.ElementAt(cellIndex));
                Assert.Equal("First argument", featureInstance.ReceivedDataTable.Rows.ElementAt(0).Cells.ElementAt(0).Value);
            }
        }

        private sealed class FeatureWithDataTableScenarioStep : Feature
        {
            public Gherkin.Ast.DataTable ReceivedDataTable { get; private set; }

            public const string Steptext = @"Step text 1212121";

            [Given(Steptext)]
            public void When_DataTable_Is_Expected(Gherkin.Ast.DataTable dataTable)
            {
                ReceivedDataTable = dataTable;
            }

            public static Gherkin.Ast.GherkinDocument CreateGherkinDocument(
                string outlineName,
                Gherkin.Ast.StepArgument stepArgument = null)
            {
                return new Gherkin.Ast.GherkinDocument(
                    new Gherkin.Ast.Feature(new Gherkin.Ast.Tag[0], null, null, null, null, null, new Gherkin.Ast.ScenarioDefinition[]
                    {
                    new Gherkin.Ast.ScenarioOutline(
                        new Gherkin.Ast.Tag[0],
                        null,
                        null,
                        outlineName,
                        null,
                        new Gherkin.Ast.Step[]
                        {
                            new Gherkin.Ast.Step(null, "Given", Steptext, stepArgument)
                        },
                        new Gherkin.Ast.Examples[]
                        {
                            new Gherkin.Ast.Examples(null, null, null, "", null,
                                new Gherkin.Ast.TableRow(null,
                                    new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "a"),
                                        new Gherkin.Ast.TableCell(null, "b"),
                                        new Gherkin.Ast.TableCell(null, "sum"),
                                    }),
                                new Gherkin.Ast.TableRow[]
                                {
                                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "0"),
                                        new Gherkin.Ast.TableCell(null, "1"),
                                        new Gherkin.Ast.TableCell(null, "1")
                                    }),
                                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "1"),
                                        new Gherkin.Ast.TableCell(null, "9"),
                                        new Gherkin.Ast.TableCell(null, "10")
                                    })
                                }),
                            new Gherkin.Ast.Examples(null, null, null, "of bigger numbers", null,
                                new Gherkin.Ast.TableRow(null,
                                    new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "a"),
                                        new Gherkin.Ast.TableCell(null, "b"),
                                        new Gherkin.Ast.TableCell(null, "sum"),
                                    }),
                                new Gherkin.Ast.TableRow[]
                                {
                                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "99"),
                                        new Gherkin.Ast.TableCell(null, "1"),
                                        new Gherkin.Ast.TableCell(null, "100")
                                    }),
                                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "100"),
                                        new Gherkin.Ast.TableCell(null, "200"),
                                        new Gherkin.Ast.TableCell(null, "300")
                                    })
                                }),
                            new Gherkin.Ast.Examples(null, null, null, "of large numbers", null,
                                new Gherkin.Ast.TableRow(null,
                                    new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "a"),
                                        new Gherkin.Ast.TableCell(null, "b"),
                                        new Gherkin.Ast.TableCell(null, "sum"),
                                    }),
                                new Gherkin.Ast.TableRow[]
                                {
                                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "999"),
                                        new Gherkin.Ast.TableCell(null, "1"),
                                        new Gherkin.Ast.TableCell(null, "1000")
                                    }),
                                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "9999"),
                                        new Gherkin.Ast.TableCell(null, "1"),
                                        new Gherkin.Ast.TableCell(null, "10000")
                                    })
                                })
                        })
                    }),
                    new Gherkin.Ast.Comment[0]);
            }
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("", 1)]
        [InlineData("of bigger numbers", 0)]
        [InlineData("of bigger numbers", 1)]
        [InlineData("of large numbers", 0)]
        [InlineData("of large numbers", 1)]
        public async Task ExecuteScenarioOutlineAsync_Executes_ScenarioStep_With_DocString(
            string exampleName,
            int exampleRowIndex)
        {
            //arrange.
            var scenarioName = "scenario123";
            var featureInstance = new FeatureWithDocStringScenarioStep();
            var output = new Mock<ITestOutputHelper>();
            featureInstance.InternalOutput = output.Object;

            var docStringContent = "some content" + Environment.NewLine +
"+++" + Environment.NewLine +
"with multi lines" + Environment.NewLine +
"---" + Environment.NewLine +
"in it";

            _featureFileRepository.Setup(r => r.GetByFilePath($"{nameof(FeatureWithDocStringScenarioStep)}.feature"))
                .Returns(new FeatureFile(FeatureWithDocStringScenarioStep.CreateGherkinDocument(scenarioName,
                    new Gherkin.Ast.DocString(null, null, docStringContent))))
                    .Verifiable();

            //act.
            await _sut.ExecuteScenarioOutlineAsync(featureInstance, scenarioName, exampleName, exampleRowIndex);

            //assert.
            _featureFileRepository.Verify();

            Assert.NotNull(featureInstance.ReceivedDocString);
            Assert.Equal(docStringContent, featureInstance.ReceivedDocString.Content);
        }

        private sealed class FeatureWithDocStringScenarioStep : Feature
        {
            public Gherkin.Ast.DocString ReceivedDocString { get; private set; }

            public const string Steptext = @"Step text 1212121";

            [Given(Steptext)]
            public void When_DocString_Is_Expected(Gherkin.Ast.DocString docString)
            {
                ReceivedDocString = docString;
            }

            public static Gherkin.Ast.GherkinDocument CreateGherkinDocument(
                string outlineName,
                Gherkin.Ast.StepArgument stepArgument = null)
            {
                return new Gherkin.Ast.GherkinDocument(
                    new Gherkin.Ast.Feature(new Gherkin.Ast.Tag[0], null, null, null, null, null, new Gherkin.Ast.ScenarioDefinition[]
                    {
                    new Gherkin.Ast.ScenarioOutline(
                        new Gherkin.Ast.Tag[0],
                        null,
                        null,
                        outlineName,
                        null,
                        new Gherkin.Ast.Step[]
                        {
                            new Gherkin.Ast.Step(null, "Given", Steptext, stepArgument)
                        },
                        new Gherkin.Ast.Examples[]
                        {
                            new Gherkin.Ast.Examples(null, null, null, "", null,
                                new Gherkin.Ast.TableRow(null,
                                    new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "a"),
                                        new Gherkin.Ast.TableCell(null, "b"),
                                        new Gherkin.Ast.TableCell(null, "sum"),
                                    }),
                                new Gherkin.Ast.TableRow[]
                                {
                                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "0"),
                                        new Gherkin.Ast.TableCell(null, "1"),
                                        new Gherkin.Ast.TableCell(null, "1")
                                    }),
                                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "1"),
                                        new Gherkin.Ast.TableCell(null, "9"),
                                        new Gherkin.Ast.TableCell(null, "10")
                                    })
                                }),
                            new Gherkin.Ast.Examples(null, null, null, "of bigger numbers", null,
                                new Gherkin.Ast.TableRow(null,
                                    new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "a"),
                                        new Gherkin.Ast.TableCell(null, "b"),
                                        new Gherkin.Ast.TableCell(null, "sum"),
                                    }),
                                new Gherkin.Ast.TableRow[]
                                {
                                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "99"),
                                        new Gherkin.Ast.TableCell(null, "1"),
                                        new Gherkin.Ast.TableCell(null, "100")
                                    }),
                                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "100"),
                                        new Gherkin.Ast.TableCell(null, "200"),
                                        new Gherkin.Ast.TableCell(null, "300")
                                    })
                                }),
                            new Gherkin.Ast.Examples(null, null, null, "of large numbers", null,
                                new Gherkin.Ast.TableRow(null,
                                    new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "a"),
                                        new Gherkin.Ast.TableCell(null, "b"),
                                        new Gherkin.Ast.TableCell(null, "sum"),
                                    }),
                                new Gherkin.Ast.TableRow[]
                                {
                                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "999"),
                                        new Gherkin.Ast.TableCell(null, "1"),
                                        new Gherkin.Ast.TableCell(null, "1000")
                                    }),
                                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                                    {
                                        new Gherkin.Ast.TableCell(null, "9999"),
                                        new Gherkin.Ast.TableCell(null, "1"),
                                        new Gherkin.Ast.TableCell(null, "10000")
                                    })
                                })
                        })
                    }),
                    new Gherkin.Ast.Comment[0]);
            }
        }
    }
}
