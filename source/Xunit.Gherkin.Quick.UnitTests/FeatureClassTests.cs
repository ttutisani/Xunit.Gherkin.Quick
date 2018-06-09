using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class FeatureClassTests
    {
        [Fact]
        public void Ctor_Initializes_Properties()
        {
            //arrange.
            var featureFilePath = "some path 123";
            var featureInstance = new FeatureForCtorTest();
            var stepMethods = new List<StepMethodInfo>
            {
                StepMethodInfo.FromMethodInfo(
                    featureInstance.GetType().GetMethod(nameof(FeatureForCtorTest.When_Something)), 
                    featureInstance)
            };

            //act.
            var sut = new FeatureClass(featureFilePath, stepMethods.AsReadOnly());

            //assert.
            Assert.Equal(featureFilePath, sut.FeatureFilePath);
            Assert.NotNull(sut.StepMethods);
            Assert.Single(sut.StepMethods);
            Assert.Same(stepMethods[0], sut.StepMethods[0]);
        }

        private sealed class FeatureForCtorTest : Feature
        {
            [When("something")]
            public void When_Something()
            {

            }
        }

        [Fact]
        public void FromFeatureInstance_Creates_FeatureClass_With_Default_FilePath_If_No_Attribute()
        {
            //arrange.
            var featureInstance = new FeatureWithoutFilePath();

            //act.
            var sut = FeatureClass.FromFeatureInstance(featureInstance);

            //assert.
            Assert.NotNull(sut);
            Assert.Equal($"{nameof(FeatureWithoutFilePath)}.feature", sut.FeatureFilePath);
            Assert.Empty(sut.StepMethods);
        }

        private sealed class FeatureWithoutFilePath : Feature
        {
        }

        [Fact]
        public void FromFeatureInstance_Creates_FeatureClass_With_FilePath_From_Attribute()
        {
            //arrange.
            var featureInstance = new FeatureWithFilePath();

            //act.
            var sut = FeatureClass.FromFeatureInstance(featureInstance);

            //assert.
            Assert.NotNull(sut);
            Assert.Equal(FeatureWithFilePath.PathFor_FeatureWithFilePath, sut.FeatureFilePath);
            Assert.Empty(sut.StepMethods);
        }

        [FeatureFile(PathFor_FeatureWithFilePath)]
        private sealed class FeatureWithFilePath : Feature
        {
            public const string PathFor_FeatureWithFilePath = "some file path const 123";
        }

        [Fact]
        public void FromFeatureInstance_Creates_FeatureClass_With_StepMethods()
        {
            //arrange.
            var featureInstance = new FeatureWithStepMethods();

            //act.
            var sut = FeatureClass.FromFeatureInstance(featureInstance);

            //assert.
            Assert.NotNull(sut);
            Assert.NotNull(sut.StepMethods);
            Assert.Equal(5, sut.StepMethods.Count);

            var given = sut.StepMethods.FirstOrDefault(sm => sm.Kind == StepMethodKind.Given && sm.Pattern == FeatureWithStepMethods.GivenStepText);
            Assert.NotNull(given);
            Assert.NotNull(given.Arguments);
            Assert.Empty(given.Arguments);

            var when = sut.StepMethods.FirstOrDefault(sm => sm.Kind == StepMethodKind.When && sm.Pattern == FeatureWithStepMethods.WhenStepText);
            Assert.NotNull(when);
            Assert.NotNull(when.Arguments);
            Assert.Empty(when.Arguments);

            var then = sut.StepMethods.FirstOrDefault(sm => sm.Kind == StepMethodKind.Then && sm.Pattern == FeatureWithStepMethods.ThenStepText);
            Assert.NotNull(then);
            Assert.NotNull(when.Arguments);
            Assert.Empty(then.Arguments);

            var and = sut.StepMethods.FirstOrDefault(sm => sm.Kind == StepMethodKind.And && sm.Pattern == FeatureWithStepMethods.AndStepText);
            Assert.NotNull(and);
            Assert.NotNull(and.Arguments);
            Assert.Empty(and.Arguments);

            var but = sut.StepMethods.FirstOrDefault(sm => sm.Kind == StepMethodKind.But && sm.Pattern == FeatureWithStepMethods.ButStepText);
            Assert.NotNull(but);
            Assert.NotNull(but.Arguments);
            Assert.Empty(but.Arguments);
        }

        private sealed class FeatureWithStepMethods : Feature
        {
            public const string GivenStepText = "step1";

            [Given(GivenStepText)]
            public void GivenStepMethod111()
            {

            }

            public const string WhenStepText = "step2 when";

            [When(WhenStepText)]
            public void WhenStepMethod123()
            {

            }

            public const string ThenStepText = "then step3";

            [Then(ThenStepText)]
            public void ThenStepMethod432()
            {

            }

            public const string AndStepText = "and and 123";

            [And(AndStepText)]
            public void AndStepMethod512()
            {

            }

            public const string ButStepText = "but 123 but";

            [But(ButStepText)]
            public void ButStepMethod333()
            {

            }
        }

        [Fact]
        public void FromFeatureInstance_Extracts_StepMethodArguments()
        {
            //arrange.
            var featureInstance = new FeatureWithStepMethodArguments();

            //act.
            var sut = FeatureClass.FromFeatureInstance(featureInstance);

            //assert.
            Assert.NotNull(sut);
            Assert.Single(sut.StepMethods);

            var step = sut.StepMethods[0];
            Assert.NotNull(step);
            Assert.NotNull(step.Arguments);
            Assert.Equal(3, step.Arguments.Count);

            VerifyArgIsPrimitive(step.Arguments[0]);
            VerifyArgIsPrimitive(step.Arguments[1]);
            VerifyArgIsPrimitive(step.Arguments[2]);

            void VerifyArgIsPrimitive(StepMethodArgument arg)
            {
                Assert.NotNull(arg);
                Assert.IsType<PrimitiveTypeArgument>(arg);
            }
        }

        private sealed class FeatureWithStepMethodArguments : Feature
        {
            [When("when")]
            public void StepMethodToBeFound(int arg1, string arg2, DateTime arg3)
            {

            }
        }

        [Fact]
        public void ExtractScenario_Extracts_ScenarioSteps()
        {
            //arrange.
            var scenarioName = "some scenario name 123";
            var featureInstance = new FeatureWithMatchingScenarioStepsToExtract();
            var sut = FeatureClass.FromFeatureInstance(featureInstance);

            //act.
            var scenario = sut.ExtractScenario(scenarioName, new FeatureFile(CreateGherkinDocument(scenarioName, 
                "Given " + FeatureWithMatchingScenarioStepsToExtract.ScenarioStep1Text.Replace(@"(\d+)", "12", StringComparison.InvariantCultureIgnoreCase),
                "And " + FeatureWithMatchingScenarioStepsToExtract.ScenarioStep2Text.Replace(@"(\d+)", "15", StringComparison.InvariantCultureIgnoreCase),
                "When " + FeatureWithMatchingScenarioStepsToExtract.ScenarioStep3Text,
                "Then " + FeatureWithMatchingScenarioStepsToExtract.ScenarioStep4Text.Replace(@"(\d+)", "27", StringComparison.InvariantCultureIgnoreCase)
                )));

            //assert.
            Assert.NotNull(scenario);
            Assert.NotNull(scenario.Steps);
            Assert.Equal(4, scenario.Steps.Count);

            AssertScenarioStepCorrectness(scenario.Steps[0].StepMethodInfo, StepMethodKind.Given, FeatureWithMatchingScenarioStepsToExtract.ScenarioStep1Text, sut);
            Assert.NotNull(scenario.Steps[0].StepMethodInfo.Arguments);
            Assert.Single(scenario.Steps[0].StepMethodInfo.Arguments);
            Assert.IsType<PrimitiveTypeArgument>(scenario.Steps[0].StepMethodInfo.Arguments[0]);
            Assert.Equal(12, scenario.Steps[0].StepMethodInfo.Arguments[0].Value);

            AssertScenarioStepCorrectness(scenario.Steps[1].StepMethodInfo, StepMethodKind.And, FeatureWithMatchingScenarioStepsToExtract.ScenarioStep2Text, sut);
            Assert.NotNull(scenario.Steps[1].StepMethodInfo.Arguments);
            Assert.Single(scenario.Steps[1].StepMethodInfo.Arguments);
            Assert.IsType<PrimitiveTypeArgument>(scenario.Steps[1].StepMethodInfo.Arguments[0]);
            Assert.Equal(15, scenario.Steps[1].StepMethodInfo.Arguments[0].Value);

            AssertScenarioStepCorrectness(scenario.Steps[2].StepMethodInfo, StepMethodKind.When, FeatureWithMatchingScenarioStepsToExtract.ScenarioStep3Text, sut);
            Assert.NotNull(scenario.Steps[2].StepMethodInfo.Arguments);
            Assert.Empty(scenario.Steps[2].StepMethodInfo.Arguments);

            AssertScenarioStepCorrectness(scenario.Steps[3].StepMethodInfo, StepMethodKind.Then, FeatureWithMatchingScenarioStepsToExtract.ScenarioStep4Text, sut);
            Assert.NotNull(scenario.Steps[3].StepMethodInfo.Arguments);
            Assert.Single(scenario.Steps[3].StepMethodInfo.Arguments);
            Assert.IsType<PrimitiveTypeArgument>(scenario.Steps[3].StepMethodInfo.Arguments[0]);
            Assert.Equal(27, scenario.Steps[3].StepMethodInfo.Arguments[0].Value);

            void AssertScenarioStepCorrectness(StepMethodInfo step, StepMethodKind kind, string text, FeatureClass featureClass)
            {
                Assert.Single(featureClass.StepMethods, sm => sm.Kind == kind && sm.Pattern == text);
                var sourceStep = featureClass.StepMethods.Single(s => s.Kind == kind && s.Pattern == text);
                Assert.NotNull(step);
                Assert.NotEqual(sourceStep, step);
            }
        }

        private static Gherkin.Ast.GherkinDocument CreateGherkinDocument(string scenarioName, params string[] steps)
        {
            var gherkinText =
@"Feature: Some Sample Feature
    In order to learn Math
    As a regular human
    I want to add two numbers using Calculator

Scenario: " + scenarioName + @"
" + string.Join(Environment.NewLine, steps)
;
            using (var gherkinStream = new MemoryStream(Encoding.UTF8.GetBytes(gherkinText)))
            using (var gherkinReader = new StreamReader(gherkinStream))
            {
                var parser = new Gherkin.Parser();
                return parser.Parse(gherkinReader);
            }
        }

        private sealed class FeatureWithMatchingScenarioStepsToExtract : Feature
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
    }
}
