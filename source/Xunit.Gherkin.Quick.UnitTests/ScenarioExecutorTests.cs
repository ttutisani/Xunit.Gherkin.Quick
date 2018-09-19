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
    public sealed class ScenarioExecutorTests
    {
        private readonly Mock<IFeatureFileRepository> _featureFileRepository = new Mock<IFeatureFileRepository>();
        private readonly ScenarioExecutor _sut;

        public ScenarioExecutorTests()
        {
            _sut = new ScenarioExecutor(_featureFileRepository.Object);
        }

        [Fact]
        public async Task ExecuteScenario_Requires_FeatureInstance()
        {
            //act / assert.
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ExecuteScenarioAsync(null, "valid scenario name"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("                 ")]
        public async Task ExecuteScenario_Requires_ScenarioName(string scenarioName)
        {
            //arrange.
            var featureInstance = new UselessFeature();

            //act / assert.
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ExecuteScenarioAsync(featureInstance, scenarioName));
        }

        private sealed class UselessFeature : Feature { }		

        [Fact]
        public async Task ExecuteScenario_Executes_All_Scenario_Steps()
        {
            //arrange.
            var step1Text = "Given " + FeatureWithScenarioSteps.ScenarioStep1Text.Replace(@"(\d+)", "12", StringComparison.InvariantCultureIgnoreCase);
            var step2Text = "And " + FeatureWithScenarioSteps.ScenarioStep2Text.Replace(@"(\d+)", "15", StringComparison.InvariantCultureIgnoreCase);
            var step3Text = "When " + FeatureWithScenarioSteps.ScenarioStep3Text;
            var step4Text = "Then " + FeatureWithScenarioSteps.ScenarioStep4Text.Replace(@"(\d+)", "27", StringComparison.InvariantCultureIgnoreCase);

            var scenarioName = "scenario 12345";
            _featureFileRepository.Setup(r => r.GetByFilePath($"{nameof(FeatureWithScenarioSteps)}.feature"))
                .Returns(new FeatureFile(CreateGherkinDocument(scenarioName,
                    new string[] 
                    {
                        step1Text,
                        step2Text,
                        step3Text,
                        step4Text
                    })))
                .Verifiable();

            var featureInstance = new FeatureWithScenarioSteps();
            var output = new Mock<ITestOutputHelper>();
            featureInstance.InternalOutput = output.Object;

            //act.
            await _sut.ExecuteScenarioAsync(featureInstance, scenarioName);

            //assert.
            _featureFileRepository.Verify();

            Assert.Equal(4, featureInstance.CallStack.Count);

            Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep1), featureInstance.CallStack[0].Key);
            Assert.NotNull(featureInstance.CallStack[0].Value);
            Assert.Single(featureInstance.CallStack[0].Value);
            Assert.Equal(12, featureInstance.CallStack[0].Value[0]);
            output.Verify(o => o.WriteLine($"{step1Text}: PASSED"), Times.Once);

            Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep2), featureInstance.CallStack[1].Key);
            Assert.NotNull(featureInstance.CallStack[1].Value);
            Assert.Single(featureInstance.CallStack[1].Value);
            Assert.Equal(15, featureInstance.CallStack[1].Value[0]);
            output.Verify(o => o.WriteLine($"{step2Text}: PASSED"), Times.Once);

            Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep3), featureInstance.CallStack[2].Key);
            Assert.Null(featureInstance.CallStack[2].Value);
            output.Verify(o => o.WriteLine($"{step3Text}: PASSED"), Times.Once);

            Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep4), featureInstance.CallStack[3].Key);
            Assert.NotNull(featureInstance.CallStack[3].Value);
            Assert.Single(featureInstance.CallStack[3].Value);
            Assert.Equal(27, featureInstance.CallStack[3].Value[0]);
            output.Verify(o => o.WriteLine($"{step4Text}: PASSED"), Times.Once);
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

        private sealed class FeatureWithScenarioSteps : Feature
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

        [Fact]
        public async Task ExecuteScenario_Executes_Successful_Scenario_Steps_And_Skips_The_Rest()
        {
            //arrange.
            var step1Text = "Given " + FeatureWithScenarioSteps_And_Throwing.ScenarioStep1Text.Replace(@"(\d+)", "12", StringComparison.InvariantCultureIgnoreCase);
            var step2Text = "And " + FeatureWithScenarioSteps_And_Throwing.ScenarioStep2Text.Replace(@"(\d+)", "15", StringComparison.InvariantCultureIgnoreCase);
            var step3Text = "When " + FeatureWithScenarioSteps_And_Throwing.ScenarioStep3Text;
            var step4Text = "Then " + FeatureWithScenarioSteps_And_Throwing.ScenarioStep4Text.Replace(@"(\d+)", "27", StringComparison.InvariantCultureIgnoreCase);

            var scenarioName = "scenario 12345";
            _featureFileRepository.Setup(r => r.GetByFilePath($"{nameof(FeatureWithScenarioSteps_And_Throwing)}.feature"))
                .Returns(new FeatureFile(CreateGherkinDocument(scenarioName,
                    new string[] 
                    {
                        step1Text,
                        step2Text,
                        step3Text,
                        step4Text
                    })))
                .Verifiable();

            var featureInstance = new FeatureWithScenarioSteps_And_Throwing();
            var output = new Mock<ITestOutputHelper>();
            featureInstance.InternalOutput = output.Object;

            //act.
            var exceptiion = await Assert.ThrowsAsync<TargetInvocationException>(async () => await _sut.ExecuteScenarioAsync(featureInstance, scenarioName));
            Assert.IsType<InvalidOperationException>(exceptiion.InnerException);

            //assert.
            _featureFileRepository.Verify();

            Assert.Equal(2, featureInstance.CallStack.Count);

            Assert.Equal(nameof(FeatureWithScenarioSteps_And_Throwing.ScenarioStep1), featureInstance.CallStack[0].Key);
            Assert.NotNull(featureInstance.CallStack[0].Value);
            Assert.Single(featureInstance.CallStack[0].Value);
            Assert.Equal(12, featureInstance.CallStack[0].Value[0]);
            output.Verify(o => o.WriteLine($"{step1Text}: PASSED"), Times.Once);

            Assert.Equal(nameof(FeatureWithScenarioSteps_And_Throwing.ScenarioStep2), featureInstance.CallStack[1].Key);
            Assert.NotNull(featureInstance.CallStack[1].Value);
            Assert.Single(featureInstance.CallStack[1].Value);
            Assert.Equal(15, featureInstance.CallStack[1].Value[0]);
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

		[Fact]
		public async Task ExecuteScenario_Executes_Background_Steps_First()
		{
			var gherkinFeaure = new GherkinFeatureBuilder()
				.WithBackground(sb => sb
					.Given("given background", null)
					.When("when background", null)
					.Then("then background", null))
				.WithScenario("test scenario", sb => sb
					.Then("step one", null))
				.Build();

			var gherkinDocument = new Gherkin.Ast.GherkinDocument(gherkinFeaure, new Gherkin.Ast.Comment[0]);

			_featureFileRepository.Setup(r => r.GetByFilePath($"{nameof(FeatureWithBackgroundSteps)}.feature"))
				.Returns(new FeatureFile(gherkinDocument))
				.Verifiable();

			var featureInstance = new FeatureWithBackgroundSteps();
			var output = new Mock<ITestOutputHelper>();
			featureInstance.InternalOutput = output.Object;

			//act.
			await _sut.ExecuteScenarioAsync(featureInstance, "test scenario");

			//assert.
			Assert.Equal("abcd", featureInstance.OrderValidator);
			output.Verify(o => o.WriteLine($"Given given background: PASSED"), Times.Once);
			output.Verify(o => o.WriteLine($"When when background: PASSED"), Times.Once);
			output.Verify(o => o.WriteLine($"Then then background: PASSED"), Times.Once);
			output.Verify(o => o.WriteLine($"Then step one: PASSED"), Times.Once);
		}

		private sealed class FeatureWithBackgroundSteps : Feature
		{
			public string OrderValidator = String.Empty;

			[Given("given background")]
			public void GivenBackground()
			{
				OrderValidator += "a";
			}

			[When("when background")]
			public void WhenBackground()
			{
				OrderValidator += "b";
			}

			[Then("then background")]
			public void ThenBackground()
			{
				OrderValidator += "c";
			}

			[Then("step one")]
			public void ThenScenario()
			{
				OrderValidator += "d";
			}
		}

		[Fact]
        public async Task ExecuteScenario_Executes_ScenarioStep_With_DataTable()
        {
            //arrange.
            var scenarioName = "scenario123";
            var featureInstance = new FeatureWithDataTableScenarioStep();
            var output = new Mock<ITestOutputHelper>();
            featureInstance.InternalOutput = output.Object;

            _featureFileRepository.Setup(r => r.GetByFilePath($"{nameof(FeatureWithDataTableScenarioStep)}.feature"))
                .Returns(new FeatureFile(CreateGherkinDocument(scenarioName,
                    new string[] 
                    {
                        "When " + FeatureWithDataTableScenarioStep.Steptext
                    },
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
            await _sut.ExecuteScenarioAsync(featureInstance, scenarioName);

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

            public const string Steptext = "Some step text";

            [When(Steptext)]
            public void When_DataTable_Is_Expected(Gherkin.Ast.DataTable dataTable)
            {
                ReceivedDataTable = dataTable;
            }
        }

        [Fact]
        public async Task ExecuteScenario_Executes_ScenarioStep_With_DocString()
        {
            //arrange.
            var featureInstance = new FeatureWithDocStringScenarioStep();
            var output = new Mock<ITestOutputHelper>();
            featureInstance.InternalOutput = output.Object;
            var docStringContent = "some content" + Environment.NewLine +
"+++" + Environment.NewLine +
"with multi lines" + Environment.NewLine +
"---" + Environment.NewLine +
"in it";
            var scenarioName = "scenario 1231121";

            _featureFileRepository.Setup(r => r.GetByFilePath(nameof(FeatureWithDocStringScenarioStep) + ".feature"))
                .Returns(new FeatureFile(CreateGherkinDocument(scenarioName,
                    new string[] { "Given " + FeatureWithDocStringScenarioStep.StepWithDocStringText },
                    new Gherkin.Ast.DocString(null, null, docStringContent))))
                .Verifiable();

            //act.
            await _sut.ExecuteScenarioAsync(featureInstance, scenarioName);

            //assert.
            _featureFileRepository.Verify();

            Assert.NotNull(featureInstance.ReceivedDocString);
            Assert.Equal(docStringContent, featureInstance.ReceivedDocString.Content);
        }

        private sealed class FeatureWithDocStringScenarioStep : Feature
        {
            public Gherkin.Ast.DocString ReceivedDocString { get; private set; }

            public const string StepWithDocStringText = "Step with docstirng";

            [Given(StepWithDocStringText)]
            public void Step_With_DocString_Argument(Gherkin.Ast.DocString docString)
            {
                ReceivedDocString = docString;
            }
        }

        [Fact]
        public async Task ExecuteScenario_Executes_Scenario_With_Shared_StepMethod()
        {
            //arrange.
            const string scenarioName = "scenario 123";

            var featureInstance = new FeatureWithSharedStepMethod();
            var output = new Mock<ITestOutputHelper>();
            featureInstance.InternalOutput = output.Object;

            _featureFileRepository.Setup(r => r.GetByFilePath($"{nameof(FeatureWithSharedStepMethod)}.feature"))
                .Returns(new FeatureFile(CreateGherkinDocument(scenarioName, new string[]
                {
                    "Given I chose 1 as first number",
                    "And I chose 2 as second number",
                    "And I chose 3 as third number",
                    "When I choose 4 as fourth number",
                    "And I choose 5 as fifth number",
                    "And I choose 6 as sixth number",
                    $"Then Result should be {1+2+3+4+5+6} on the screen"
                })));

            //act.
            await _sut.ExecuteScenarioAsync(featureInstance, scenarioName);

            //assert.
            Assert.Equal(7, featureInstance.CallStack.Count);

            Assert_Callback(0, nameof(FeatureWithSharedStepMethod.Selecting_numbers), 1);
            Assert_Callback(1, nameof(FeatureWithSharedStepMethod.Selecting_numbers), 2);
            Assert_Callback(2, nameof(FeatureWithSharedStepMethod.Selecting_numbers), 3);
            Assert_Callback(3, nameof(FeatureWithSharedStepMethod.Selecting_numbers), 4);
            Assert_Callback(4, nameof(FeatureWithSharedStepMethod.Selecting_numbers), 5);
            Assert_Callback(5, nameof(FeatureWithSharedStepMethod.Selecting_numbers), 6);
            Assert_Callback(6, nameof(FeatureWithSharedStepMethod.Result_should_be_x_on_the_screen), (1 + 2 + 3 + 4 + 5 + 6));

            void Assert_Callback(int index, string methodName, int value)
            {
                Assert.Equal(methodName, featureInstance.CallStack[index].Key);
                Assert.NotNull(featureInstance.CallStack[index].Value);
                Assert.Single(featureInstance.CallStack[index].Value);
                Assert.Equal(value, featureInstance.CallStack[index].Value[0]);
            }
        }

        private sealed class FeatureWithSharedStepMethod : Feature
        {
            public List<KeyValuePair<string, object[]>> CallStack { get; } = new List<KeyValuePair<string, object[]>>();

            [Given(@"I chose (\d+) as first number")]
            [And(@"I chose (\d+) as second number")]
            [And(@"I chose (\d+) as third number")]
            [When(@"I choose (\d+) as fourth number")]
            [And(@"I choose (\d+) as fifth number")]
            [And(@"I choose (\d+) as sixth number")]
            public void Selecting_numbers(int x)
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(Selecting_numbers), new object[] { x }));
            }

            [Then(@"Result should be (\d+) on the screen")]
            public void Result_should_be_x_on_the_screen(int x)
            {
                CallStack.Add(new KeyValuePair<string, object[]>(nameof(Result_should_be_x_on_the_screen), new object[] { x }));
            }
        }
    }
}
