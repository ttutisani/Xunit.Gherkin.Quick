using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit.Gherkin.Quick.vNext.Evaluators;
using Xunit.Gherkin.Quick.vNext.TestScenarios;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick.ProjectConsumer.UnitTests.Evaluators;

public sealed class FeatureEvaluatorTests
{
    [Fact]
    public async Task ExecuteScenario_Executes_All_Scenario_Steps()
    {
        var testScenario = new TestScenario(
            "Test feature",
            "Test scenario",
            CultureInfo.InvariantCulture,
            [],
            [
                new(TestStepType.Given, "I chose 12 as first number"),
                new(TestStepType.And, "I chose 15 as second number"),
                new(TestStepType.When, "I press add"),
                new(TestStepType.Then, "the result should be 27 on the screen")
            ]
        );
        var feature = new FeatureWithScenarioSteps();
        var featureEvaluator = new FeatureEvaluator(feature);

        foreach (var testStep in testScenario.Steps)
            await featureEvaluator.EvaluateStepAsync(testStep);

        Assert.Equal(4, feature.ExecutedSteps.Count);

        Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep1), feature.ExecutedSteps[0].Key);
        Assert.NotNull(feature.ExecutedSteps[0].Value);
        Assert.Single(feature.ExecutedSteps[0].Value);
        Assert.Equal(12, feature.ExecutedSteps[0].Value[0]);

        Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep2), feature.ExecutedSteps[1].Key);
        Assert.NotNull(feature.ExecutedSteps[1].Value);
        Assert.Single(feature.ExecutedSteps[1].Value);
        Assert.Equal(15, feature.ExecutedSteps[1].Value[0]);

        Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep3), feature.ExecutedSteps[2].Key);
        Assert.Null(feature.ExecutedSteps[2].Value);

        Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep4), feature.ExecutedSteps[3].Key);
        Assert.NotNull(feature.ExecutedSteps[3].Value);
        Assert.Single(feature.ExecutedSteps[3].Value);
        Assert.Equal(27, feature.ExecutedSteps[3].Value[0]);
    }

    private sealed class FeatureWithScenarioSteps : Feature
    {
        private readonly List<KeyValuePair<string, object[]>> _executedSteps = [];

        public IReadOnlyList<KeyValuePair<string, object[]>> ExecutedSteps
            => _executedSteps;

        [Given("Non matching given")]
        public void NonMatchingStep1_before()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep1_before), null));

        [Given(@"I chose (\d+) as first number")]
        public void ScenarioStep1(int firstNumber)
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep1), [firstNumber]));

        [Given("Non matching given")]
        public void NonMatchingStep1_after()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep1_after), null));

        [And("Non matching and")]
        public void NonMatchingStep2_before()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep2_before), null));

        [And(@"I chose (\d+) as second number")]
        public void ScenarioStep2(int secondNumber)
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep2), [secondNumber]));

        [And("Non matching and")]
        public void NonMatchingStep2_after()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep2_after), null));

        [When("Non matching when")]
        public void NonMatchingStep3_before()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep3_before), null));

        [When("I press add")]
        public void ScenarioStep3()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep3), null));

        [When("Non matching when")]
        public void NonMatchingStep3_after()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep3_after), null));

        [Then("Non matching then")]
        public void NonMatchingStep4_before()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep4_before), null));

        [Then(@"the result should be (\d+) on the screen")]
        public void ScenarioStep4(int result)
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep4), [result]));

        [Then("Non matching then")]
        public void NonMatchingStep4_after()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep4_after), null));
    }

    [Fact]
    public async Task ExecuteScenario_Executes_Successful_Scenario_Steps_And_Skips_The_Rest()
    {
        var testScenario = new TestScenario(
            "Test feature",
            "Test scenario",
            CultureInfo.InvariantCulture,
            [],
            [
                new(TestStepType.Given, "I chose 12 as first number"),
                new(TestStepType.And, "I chose 15 as second number"),
                new(TestStepType.When, "I press add"),
                new(TestStepType.Then, "the result should be 27 on the screen")
            ]
        );
        var feature = new FeatureWithScenarioSteps_And_Throwing();
        var featureEvaluator = new FeatureEvaluator(feature);

        var exception = await Assert.ThrowsAsync<TargetInvocationException>(async () =>
        {
            foreach (var testStep in testScenario.Steps)
                await featureEvaluator.EvaluateStepAsync(testStep);
        });

        Assert.IsType<InvalidOperationException>(exception.InnerException);
        Assert.Equal(2, feature.ExecutedSteps.Count);

        Assert.Equal(nameof(FeatureWithScenarioSteps_And_Throwing.ScenarioStep1), feature.ExecutedSteps[0].Key);
        Assert.NotNull(feature.ExecutedSteps[0].Value);
        Assert.Single(feature.ExecutedSteps[0].Value);
        Assert.Equal(12, feature.ExecutedSteps[0].Value[0]);

        Assert.Equal(nameof(FeatureWithScenarioSteps_And_Throwing.ScenarioStep2), feature.ExecutedSteps[1].Key);
        Assert.NotNull(feature.ExecutedSteps[1].Value);
        Assert.Single(feature.ExecutedSteps[1].Value);
        Assert.Equal(15, feature.ExecutedSteps[1].Value[0]);
    }

    private sealed class FeatureWithScenarioSteps_And_Throwing : Feature
    {
        private readonly List<KeyValuePair<string, object[]>> _executedSteps = [];

        public IReadOnlyList<KeyValuePair<string, object[]>> ExecutedSteps
            => _executedSteps;

        [Given("Non matching given")]
        public void NonMatchingStep1_before()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep1_before), null));


        [Given(@"I chose (\d+) as first number")]
        public void ScenarioStep1(int firstNumber)
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep1), [firstNumber]));

        [Given("Non matching given")]
        public void NonMatchingStep1_after()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep1_after), null));

        [And("Non matching and")]
        public void NonMatchingStep2_before()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep2_before), null));

        [And(@"I chose (\d+) as second number")]
        public void ScenarioStep2(int secondNumber)
        {
            _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep2), [secondNumber]));

            throw new InvalidOperationException("Some exception to stop execution of next steps.");
        }

        [And("Non matching and")]
        public void NonMatchingStep2_after()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep2_after), null));

        [When("Non matching when")]
        public void NonMatchingStep3_before()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep3_before), null));

        [When("I press add")]
        public void ScenarioStep3()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep3), null));

        [When("Non matching when")]
        public void NonMatchingStep3_after()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep3_after), null));

        [Then("Non matching then")]
        public void NonMatchingStep4_before()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep4_before), null));

        [Then(@"the result should be (\d+) on the screen")]
        public void ScenarioStep4(int result)
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(ScenarioStep4), [result]));

        [Then("Non matching then")]
        public void NonMatchingStep4_after()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(NonMatchingStep4_after), null));
    }

    [Fact]
    public async Task ExecuteScenario_Executes_Background_Steps_First()
    {
        var testScenario = new TestScenario(
            "Test feature",
            "Test scenario",
            CultureInfo.InvariantCulture,
            [],
            [
                new(TestStepType.Given, "given background"),
                new(TestStepType.When, "when background"),
                new(TestStepType.Then, "then background"),
                new(TestStepType.Then, "step one")
            ]
        );
        var feature = new FeatureWithBackgroundSteps();
        var featureEvaluator = new FeatureEvaluator(feature);

        foreach (var testStep in testScenario.Steps)
            await featureEvaluator.EvaluateStepAsync(testStep);

        Assert.Equal("abcd", feature.OrderValidator);
    }

    private sealed class FeatureWithBackgroundSteps : Feature
    {
        public string OrderValidator = string.Empty;

        [Given("given background")]
        public void GivenBackground()
            => OrderValidator += "a";

        [When("when background")]
        public void WhenBackground()
            => OrderValidator += "b";

        [Then("then background")]
        public void ThenBackground()
            => OrderValidator += "c";

        [Then("step one")]
        public void ThenScenario()
            => OrderValidator += "d";
    }

    [Fact]
    public async Task ExecuteScenario_Executes_ScenarioStep_With_DataTable()
    {
        var testScenario = new TestScenario(
            "Test feature",
            "Test scenario",
            CultureInfo.InvariantCulture,
            [],
            [
                new(TestStepType.When, "Some step text", new TestStepTableArgument(
                    [
                        new(
                            [
                                new("First argument", new TestStepArgumentLocation(0, 0)),
                                new("Second argument", new TestStepArgumentLocation(0, 0)),
                                new("Result", new TestStepArgumentLocation(0, 0))
                            ],
                            new TestStepArgumentLocation(0, 0)
                        ),
                        new(
                            [
                                new("1", new TestStepArgumentLocation(0, 0)),
                                new("2", new TestStepArgumentLocation(0, 0)),
                                new("3", new TestStepArgumentLocation(0, 0))
                            ],
                            new TestStepArgumentLocation(0, 0)
                        ),
                        new(
                            [
                                new("a", new TestStepArgumentLocation(0, 0)),
                                new("b", new TestStepArgumentLocation(0, 0)),
                                new("c", new TestStepArgumentLocation(0, 0))
                            ],
                            new TestStepArgumentLocation(0, 0)
                        )
                    ],
                    new TestStepArgumentLocation(0, 0)))
            ]
        );
        var feature = new FeatureWithDataTableScenarioStep();
        var featureEvaluator = new FeatureEvaluator(feature);

        foreach (var testStep in testScenario.Steps)
            await featureEvaluator.EvaluateStepAsync(testStep);

        Assert.NotNull(feature.ReceivedDataTable);
        Assert.Equal(3, feature.ReceivedDataTable.Rows.Count());

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
            Assert.True(feature.ReceivedDataTable.Rows.Count() > rowIndex);
            Assert.NotNull(feature.ReceivedDataTable.Rows.ElementAt(rowIndex));
            Assert.True(feature.ReceivedDataTable.Rows.ElementAt(rowIndex).Cells.Count() > cellIndex);
            Assert.NotNull(feature.ReceivedDataTable.Rows.ElementAt(rowIndex).Cells.ElementAt(cellIndex));
            Assert.Equal("First argument", feature.ReceivedDataTable.Rows.ElementAt(0).Cells.ElementAt(0).Value);
        }
    }

    private sealed class FeatureWithDataTableScenarioStep : Feature
    {
        public global::Gherkin.Ast.DataTable ReceivedDataTable { get; private set; }

        [When("Some step text")]
        public void When_DataTable_Is_Expected(global::Gherkin.Ast.DataTable dataTable)
            => ReceivedDataTable = dataTable;
    }

    [Fact]
    public async Task ExecuteScenario_Executes_ScenarioStep_With_DocString()
    {
        var docStringContent = "some content" + Environment.NewLine +
"+++" + Environment.NewLine +
"with multi lines" + Environment.NewLine +
"---" + Environment.NewLine +
"in it";
        var testScenario = new TestScenario(
            "Test feature",
            "Test scenario",
            CultureInfo.InvariantCulture,
            [],
            [
                new(TestStepType.Given, "Step with docstirng", new TestStepDocStringArgument(docStringContent, "text", new TestStepArgumentLocation(0, 0)))
            ]
        );
        var feature = new FeatureWithDocStringScenarioStep();
        var featureEvaluator = new FeatureEvaluator(feature);

        foreach (var testStep in testScenario.Steps)
            await featureEvaluator.EvaluateStepAsync(testStep);

        Assert.NotNull(feature.ReceivedDocString);
        Assert.Equal(docStringContent, feature.ReceivedDocString.Content);
        Assert.Equal("text", feature.ReceivedDocString.ContentType);
    }

    private sealed class FeatureWithDocStringScenarioStep : Feature
    {
        public global::Gherkin.Ast.DocString ReceivedDocString { get; private set; }

        [Given("Step with docstirng")]
        public void Step_With_DocString_Argument(global::Gherkin.Ast.DocString docString)
            => ReceivedDocString = docString;
    }

    [Fact]
    public async Task ExecuteScenario_Executes_Scenario_With_Shared_StepMethod()
    {
        //arrange.
        var testScenario = new TestScenario(
            "Test feature",
            "Test scenario",
            CultureInfo.InvariantCulture,
            [],
            [
                new(TestStepType.Given, "I chose 1 as first number"),
                new(TestStepType.And, "I chose 2 as second number"),
                new(TestStepType.And, "I chose 3 as third number"),
                new(TestStepType.When, "I choose 4 as fourth number"),
                new(TestStepType.And, "I choose 5 as fifth number"),
                new(TestStepType.And, "I choose 6 as sixth number"),
                new(TestStepType.Then, $"Result should be {1 + 2 + 3 + 4 + 5 + 6} on the screen")
            ]
        );
        var feature = new FeatureWithSharedStepMethod();
        var featureEvaluator = new FeatureEvaluator(feature);

        foreach (var testStep in testScenario.Steps)
            await featureEvaluator.EvaluateStepAsync(testStep);

        Assert.Equal(7, feature.ExecutedSteps.Count);

        Assert_Callback(0, nameof(FeatureWithSharedStepMethod.Selecting_numbers), 1);
        Assert_Callback(1, nameof(FeatureWithSharedStepMethod.Selecting_numbers), 2);
        Assert_Callback(2, nameof(FeatureWithSharedStepMethod.Selecting_numbers), 3);
        Assert_Callback(3, nameof(FeatureWithSharedStepMethod.Selecting_numbers), 4);
        Assert_Callback(4, nameof(FeatureWithSharedStepMethod.Selecting_numbers), 5);
        Assert_Callback(5, nameof(FeatureWithSharedStepMethod.Selecting_numbers), 6);
        Assert_Callback(6, nameof(FeatureWithSharedStepMethod.Result_should_be_x_on_the_screen), (1 + 2 + 3 + 4 + 5 + 6));

        void Assert_Callback(int index, string methodName, int value)
        {
            Assert.Equal(methodName, feature.ExecutedSteps[index].Key);
            Assert.NotNull(feature.ExecutedSteps[index].Value);
            Assert.Single(feature.ExecutedSteps[index].Value);
            Assert.Equal(value, feature.ExecutedSteps[index].Value[0]);
        }
    }

    private sealed class FeatureWithSharedStepMethod : Feature
    {
        private readonly List<KeyValuePair<string, object[]>> _executedSteps = [];

        public IReadOnlyList<KeyValuePair<string, object[]>> ExecutedSteps
            => _executedSteps;

        [Given(@"I chose (\d+) as first number")]
        [And(@"I chose (\d+) as second number")]
        [And(@"I chose (\d+) as third number")]
        [When(@"I choose (\d+) as fourth number")]
        [And(@"I choose (\d+) as fifth number")]
        [And(@"I choose (\d+) as sixth number")]
        public void Selecting_numbers(int x)
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(Selecting_numbers), [x]));

        [Then(@"Result should be (\d+) on the screen")]
        public void Result_should_be_x_on_the_screen(int x)
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(Result_should_be_x_on_the_screen), [x]));
    }

    [Fact]
    public async Task ExecuteScenario_Executes_Scenario_With_Star_Notation()
    {
        var testScenario = new TestScenario(
            "Test feature",
            "Test scenario",
            CultureInfo.InvariantCulture,
            [],
            [
                new(TestStepType.Given, "I have some cukes")
            ]
        );

        var feature = new FeatureWithStarNotation();
        var featureEvaluator = new FeatureEvaluator(feature);

        foreach (var testStep in testScenario.Steps)
            await featureEvaluator.EvaluateStepAsync(testStep);

        Assert.Single(feature.ExecutedSteps);
        Assert.Equal(nameof(FeatureWithStarNotation.I_Have_Some_Cukes), feature.ExecutedSteps[0].Key);
    }

    private sealed class FeatureWithStarNotation : Feature
    {
        public List<KeyValuePair<string, object[]>> _executedSteps = [];

        public IReadOnlyList<KeyValuePair<string, object[]>> ExecutedSteps
            => _executedSteps;

        [Given("I have some cukes")]
        public void I_Have_Some_Cukes()
            => _executedSteps.Add(new KeyValuePair<string, object[]>(nameof(I_Have_Some_Cukes), null));
    }

    [Fact]
    public async Task ExecuteScenario_DoesNotAllow_AsyncVoid_Steps()
    {
        var testScenario = new TestScenario(
            "Test feature",
            "Test scenario",
            CultureInfo.InvariantCulture,
            [],
            [
                new TestStep(TestStepType.Given, "Any step text")
            ]
        );
        var feature = new FeatureWithAsyncVoidStep();
        var featureEvaluator = new FeatureEvaluator(feature);

        var exception = await Assert.ThrowsAsync<XunitException>(async () =>
        {
            foreach (var testStep in testScenario.Steps)
                await featureEvaluator.EvaluateStepAsync(testStep);
        });
        Assert.Equal("Method 'StepWithAsyncVoid' of 'FeatureWithAsyncVoidStep' class is async and void, which looks like a mistake. Use either async with Task or void without async.", exception.Message);
    }

    private sealed class FeatureWithAsyncVoidStep : Feature
    {
        [Given("Any step text")]
        public async void StepWithAsyncVoid()
        {
            await Task.CompletedTask;
        }
    }
}
