using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Gherkin.Ast;
using Xunit.Gherkin.Quick.Evaluators;
using Xunit.Gherkin.Quick.TestScenarios;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick.Tests.Units.Evaluators;

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

        Assert.Collection(
            feature.ExecutedSteps,
            firstStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep1), firstStep.Name),
                () => Assert.Equal([12], firstStep.Arguments)
            ),
            secondStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep2), secondStep.Name),
                () => Assert.Equal([15], secondStep.Arguments)
            ),
            thirdStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep3), thirdStep.Name),
                () => Assert.Empty(thirdStep.Arguments)
            ),
            fourthStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep4), fourthStep.Name),
                () => Assert.Equal([27], fourthStep.Arguments)
            )
        );
    }

    private sealed class FeatureWithScenarioSteps : Feature
    {
        private readonly List<(string Name, IReadOnlyList<object> Arguments)> _executedSteps = [];

        public IReadOnlyList<(string Name, IReadOnlyList<object> Arguments)> ExecutedSteps
            => _executedSteps;

        [Given("Non matching given")]
        public void NonMatchingStep1_before()
            => _executedSteps.Add(new(nameof(NonMatchingStep1_before), []));

        [Given(@"I chose (\d+) as first number")]
        public void ScenarioStep1(int firstNumber)
            => _executedSteps.Add(new(nameof(ScenarioStep1), [firstNumber]));

        [Given("Non matching given")]
        public void NonMatchingStep1_after()
            => _executedSteps.Add(new(nameof(NonMatchingStep1_after), []));

        [And("Non matching and")]
        public void NonMatchingStep2_before()
            => _executedSteps.Add(new(nameof(NonMatchingStep2_before), []));

        [And(@"I chose (\d+) as second number")]
        public void ScenarioStep2(int secondNumber)
            => _executedSteps.Add(new(nameof(ScenarioStep2), [secondNumber]));

        [And("Non matching and")]
        public void NonMatchingStep2_after()
            => _executedSteps.Add(new(nameof(NonMatchingStep2_after), []));

        [When("Non matching when")]
        public void NonMatchingStep3_before()
            => _executedSteps.Add(new(nameof(NonMatchingStep3_before), []));

        [When("I press add")]
        public void ScenarioStep3()
            => _executedSteps.Add(new(nameof(ScenarioStep3), []));

        [When("Non matching when")]
        public void NonMatchingStep3_after()
            => _executedSteps.Add(new(nameof(NonMatchingStep3_after), []));

        [Then("Non matching then")]
        public void NonMatchingStep4_before()
            => _executedSteps.Add(new(nameof(NonMatchingStep4_before), []));

        [Then(@"the result should be (\d+) on the screen")]
        public void ScenarioStep4(int result)
            => _executedSteps.Add(new(nameof(ScenarioStep4), [result]));

        [Then("Non matching then")]
        public void NonMatchingStep4_after()
            => _executedSteps.Add(new(nameof(NonMatchingStep4_after), []));
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

        Assert.Multiple(
            () => Assert.IsType<InvalidOperationException>(exception.InnerException),
            () => Assert.Collection(
                feature.ExecutedSteps,
                firstStep => Assert.Multiple(
                    () => Assert.Equal(nameof(FeatureWithScenarioSteps_And_Throwing.ScenarioStep1), firstStep.Name),
                    () => Assert.Equal([12], firstStep.Arguments)
                ),
                secondStep => Assert.Multiple(
                    () => Assert.Equal(nameof(FeatureWithScenarioSteps_And_Throwing.ScenarioStep2), secondStep.Name),
                    () => Assert.Equal([15], secondStep.Arguments)
                )
            )
        );
    }

    private sealed class FeatureWithScenarioSteps_And_Throwing : Feature
    {
        private readonly List<(string Name, IReadOnlyList<object> Arguments)> _executedSteps = [];

        public IReadOnlyList<(string Name, IReadOnlyList<object> Arguments)> ExecutedSteps
            => _executedSteps;

        [Given("Non matching given")]
        public void NonMatchingStep1_before()
            => _executedSteps.Add(new(nameof(NonMatchingStep1_before), []));


        [Given(@"I chose (\d+) as first number")]
        public void ScenarioStep1(int firstNumber)
            => _executedSteps.Add(new(nameof(ScenarioStep1), [firstNumber]));

        [Given("Non matching given")]
        public void NonMatchingStep1_after()
            => _executedSteps.Add(new(nameof(NonMatchingStep1_after), []));

        [And("Non matching and")]
        public void NonMatchingStep2_before()
            => _executedSteps.Add(new(nameof(NonMatchingStep2_before), []));

        [And(@"I chose (\d+) as second number")]
        public void ScenarioStep2(int secondNumber)
        {
            _executedSteps.Add(new(nameof(ScenarioStep2), [secondNumber]));

            throw new InvalidOperationException("Some exception to stop execution of next steps.");
        }

        [And("Non matching and")]
        public void NonMatchingStep2_after()
            => _executedSteps.Add(new(nameof(NonMatchingStep2_after), []));

        [When("Non matching when")]
        public void NonMatchingStep3_before()
            => _executedSteps.Add(new(nameof(NonMatchingStep3_before), []));

        [When("I press add")]
        public void ScenarioStep3()
            => _executedSteps.Add(new(nameof(ScenarioStep3), []));

        [When("Non matching when")]
        public void NonMatchingStep3_after()
            => _executedSteps.Add(new(nameof(NonMatchingStep3_after), []));

        [Then("Non matching then")]
        public void NonMatchingStep4_before()
            => _executedSteps.Add(new(nameof(NonMatchingStep4_before), []));

        [Then(@"the result should be (\d+) on the screen")]
        public void ScenarioStep4(int result)
            => _executedSteps.Add(new(nameof(ScenarioStep4), [result]));

        [Then("Non matching then")]
        public void NonMatchingStep4_after()
            => _executedSteps.Add(new(nameof(NonMatchingStep4_after), []));
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

        Assert.Collection(
            feature.ExecutedSteps,
            firstStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithBackgroundSteps.GivenBackground), firstStep.Name),
                () => Assert.Empty(firstStep.Arguments)
            ),
            secondStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithBackgroundSteps.WhenBackground), secondStep.Name),
                () => Assert.Empty(secondStep.Arguments)
            ),
            thirdStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithBackgroundSteps.ThenBackground), thirdStep.Name),
                () => Assert.Empty(thirdStep.Arguments)
            ),
            fourthStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithBackgroundSteps.ThenScenario), fourthStep.Name),
                () => Assert.Empty(fourthStep.Arguments)
            )
        );
    }

    private sealed class FeatureWithBackgroundSteps : Feature
    {
        private readonly List<(string Name, IReadOnlyList<object> Arguments)> _executedSteps = [];

        public IReadOnlyList<(string Name, IReadOnlyList<object> Arguments)> ExecutedSteps
            => _executedSteps;

        [Given("given background")]
        public void GivenBackground()
            => _executedSteps.Add(new(nameof(GivenBackground), []));

        [When("when background")]
        public void WhenBackground()
            => _executedSteps.Add(new(nameof(WhenBackground), []));

        [Then("then background")]
        public void ThenBackground()
            => _executedSteps.Add(new(nameof(ThenBackground), []));

        [Then("step one")]
        public void ThenScenario()
            => _executedSteps.Add(new(nameof(ThenScenario), []));
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
                    ]))
            ]
        );
        var feature = new FeatureWithDataTableScenarioStep();
        var featureEvaluator = new FeatureEvaluator(feature);

        foreach (var testStep in testScenario.Steps)
            await featureEvaluator.EvaluateStepAsync(testStep);

        Assert.NotNull(feature.ReceivedDataTable);
        Assert.Collection(
            feature.ReceivedDataTable.Rows,
            firstRow => Assert.Collection(
                firstRow.Cells,
                firstCell => Assert.Equal("First argument", firstCell.Value),
                secondCell => Assert.Equal("Second argument", secondCell.Value),
                thirdCell => Assert.Equal("Result", thirdCell.Value)
            ),
            secondRow => Assert.Collection(
                secondRow.Cells,
                firstCell => Assert.Equal("1", firstCell.Value),
                secondCell => Assert.Equal("2", secondCell.Value),
                thirdCell => Assert.Equal("3", thirdCell.Value)
            ),
            thirdRow => Assert.Collection(
                thirdRow.Cells,
                firstCell => Assert.Equal("a", firstCell.Value),
                secondCell => Assert.Equal("b", secondCell.Value),
                thirdCell => Assert.Equal("c", thirdCell.Value)
            )
        );
    }

    private sealed class FeatureWithDataTableScenarioStep : Feature
    {
        public DataTable ReceivedDataTable { get; private set; }

        [When("Some step text")]
        public void When_DataTable_Is_Expected(DataTable dataTable)
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
        Assert.Multiple(
            () => Assert.Equal("text", feature.ReceivedDocString.ContentType),
            () => Assert.Equal(docStringContent, feature.ReceivedDocString.Content)
        );
    }

    private sealed class FeatureWithDocStringScenarioStep : Feature
    {
        public DocString ReceivedDocString { get; private set; }

        [Given("Step with docstirng")]
        public void Step_With_DocString_Argument(DocString docString)
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

        Assert.Collection(
            feature.ExecutedSteps,
            firstStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithSharedStepMethod.Selecting_numbers), firstStep.Name),
                () => Assert.Equal([1], firstStep.Arguments)
            ),
            secondStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithSharedStepMethod.Selecting_numbers), secondStep.Name),
                () => Assert.Equal([2], secondStep.Arguments)
            ),
            thirdStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithSharedStepMethod.Selecting_numbers), thirdStep.Name),
                () => Assert.Equal([3], thirdStep.Arguments)
            ),
            fourthStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithSharedStepMethod.Selecting_numbers), fourthStep.Name),
                () => Assert.Equal([4], fourthStep.Arguments)
            ),
            fifthStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithSharedStepMethod.Selecting_numbers), fifthStep.Name),
                () => Assert.Equal([5], fifthStep.Arguments)
            ),
            sixthStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithSharedStepMethod.Selecting_numbers), sixthStep.Name),
                () => Assert.Equal([6], sixthStep.Arguments)
            ),
            seventhStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithSharedStepMethod.Result_should_be_x_on_the_screen), seventhStep.Name),
                () => Assert.Equal([1 + 2 + 3 + 4 + 5 + 6], seventhStep.Arguments)
            )
        );
    }

    private sealed class FeatureWithSharedStepMethod : Feature
    {
        private readonly List<(string Name, IReadOnlyList<object> Arguments)> _executedSteps = [];

        public IReadOnlyList<(string Name, IReadOnlyList<object> Arguments)> ExecutedSteps
            => _executedSteps;

        [Given(@"I chose (\d+) as first number")]
        [And(@"I chose (\d+) as second number")]
        [And(@"I chose (\d+) as third number")]
        [When(@"I choose (\d+) as fourth number")]
        [And(@"I choose (\d+) as fifth number")]
        [And(@"I choose (\d+) as sixth number")]
        public void Selecting_numbers(int x)
            => _executedSteps.Add(new(nameof(Selecting_numbers), [x]));

        [Then(@"Result should be (\d+) on the screen")]
        public void Result_should_be_x_on_the_screen(int x)
            => _executedSteps.Add(new(nameof(Result_should_be_x_on_the_screen), [x]));
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

        Assert.Collection(
            feature.ExecutedSteps,
            onlyStep => Assert.Multiple(
                () => Assert.Equal(nameof(FeatureWithStarNotation.I_Have_Some_Cukes), onlyStep.Name),
                () => Assert.Empty(onlyStep.Arguments)
            )
        );
    }

    private sealed class FeatureWithStarNotation : Feature
    {
        public List<(string Name, IReadOnlyList<object> Arguments)> _executedSteps = [];

        public IReadOnlyList<(string Name, IReadOnlyList<object> Arguments)> ExecutedSteps
            => _executedSteps;

        [Given("I have some cukes")]
        public void I_Have_Some_Cukes()
            => _executedSteps.Add(new(nameof(I_Have_Some_Cukes), []));
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
            => await Task.CompletedTask;
    }
}
