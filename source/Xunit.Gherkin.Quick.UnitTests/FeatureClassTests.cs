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
            var stepMethods = new List<StepMethod>
            {
                new StepMethod(StepMethodKind.Given, "some text 123", new List<StepMethodArgument>())
            };

            //act.
            var sut = new FeatureClass(featureFilePath, stepMethods.AsReadOnly());

            //assert.
            Assert.Equal(featureFilePath, sut.FeatureFilePath);
            Assert.NotNull(sut.StepMethods);
            Assert.Single(sut.StepMethods);
            Assert.Same(stepMethods[0], sut.StepMethods[0]);
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

            var given = sut.StepMethods.FirstOrDefault(sm => sm.Kind == StepMethodKind.Given && sm.Text == FeatureWithStepMethods.GivenStepText);
            Assert.NotNull(given);
            Assert.NotNull(given.Arguments);
            Assert.Empty(given.Arguments);

            var when = sut.StepMethods.FirstOrDefault(sm => sm.Kind == StepMethodKind.When && sm.Text == FeatureWithStepMethods.WhenStepText);
            Assert.NotNull(when);
            Assert.NotNull(when.Arguments);
            Assert.Empty(when.Arguments);

            var then = sut.StepMethods.FirstOrDefault(sm => sm.Kind == StepMethodKind.Then && sm.Text == FeatureWithStepMethods.ThenStepText);
            Assert.NotNull(then);
            Assert.NotNull(when.Arguments);
            Assert.Empty(then.Arguments);

            var and = sut.StepMethods.FirstOrDefault(sm => sm.Kind == StepMethodKind.And && sm.Text == FeatureWithStepMethods.AndStepText);
            Assert.NotNull(and);
            Assert.NotNull(and.Arguments);
            Assert.Empty(and.Arguments);

            var but = sut.StepMethods.FirstOrDefault(sm => sm.Kind == StepMethodKind.But && sm.Text == FeatureWithStepMethods.ButStepText);
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
                "Given " + FeatureWithMatchingScenarioStepsToExtract.ScenarioStep1Text,
                "And " + FeatureWithMatchingScenarioStepsToExtract.ScenarioStep2Text,
                "When " + FeatureWithMatchingScenarioStepsToExtract.ScenarioStep3Text,
                "Then " + FeatureWithMatchingScenarioStepsToExtract.ScenarioStep4Text
                )));

            //assert.
            Assert.NotNull(scenario);
            Assert.NotNull(scenario.Steps);
            Assert.Equal(4, scenario.Steps.Count);

            AssertScenarioStepCorrectness(scenario.Steps[0], StepMethodKind.Given, FeatureWithMatchingScenarioStepsToExtract.ScenarioStep1Text, sut);
            AssertScenarioStepCorrectness(scenario.Steps[1], StepMethodKind.And, FeatureWithMatchingScenarioStepsToExtract.ScenarioStep2Text, sut);
            AssertScenarioStepCorrectness(scenario.Steps[2], StepMethodKind.When, FeatureWithMatchingScenarioStepsToExtract.ScenarioStep3Text, sut);
            AssertScenarioStepCorrectness(scenario.Steps[3], StepMethodKind.Then, FeatureWithMatchingScenarioStepsToExtract.ScenarioStep4Text, sut);
            
            void AssertScenarioStepCorrectness(StepMethod step, StepMethodKind kind, string text, FeatureClass sourceFeature)
            {
                var sourceStep = sourceFeature.StepMethods.Single(s => s.Kind == kind && s.Text == text);
                Assert.NotNull(step);
                Assert.Same(step, sourceStep);
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
            public List<string> CallStack { get; } = new List<string>();

            public const string ScenarioStep1Text = "I chose 12 as first number";

            [Given(ScenarioStep1Text)]
            public void ScenarioStep1()
            {
                CallStack.Add(nameof(ScenarioStep1));
            }

            [Given("Non matching given")]
            public void NonMatchingStep1()
            {
                CallStack.Add(nameof(NonMatchingStep1));
            }

            public const string ScenarioStep2Text = "I chose 15 as second number";

            [And(ScenarioStep2Text)]
            public void ScenarioStep2()
            {
                CallStack.Add(nameof(ScenarioStep2));
            }

            [And("Non matching and")]
            public void NonMatchingStep2()
            {
                CallStack.Add(nameof(NonMatchingStep2));
            }

            public const string ScenarioStep3Text = "I press add";

            [When(ScenarioStep3Text)]
            public void ScenarioStep3()
            {
                CallStack.Add(nameof(ScenarioStep3));
            }

            [When("Non matching when")]
            public void NonMatchingStep3()
            {
                CallStack.Add(nameof(NonMatchingStep3));
            }

            public const string ScenarioStep4Text = "the result should be 27 on the screen";

            [Then(ScenarioStep4Text)]
            public void ScenarioStep4()
            {
                CallStack.Add(nameof(ScenarioStep4));
            }

            [Then("Non matching then")]
            public void NonMatchingStep4()
            {
                CallStack.Add(nameof(NonMatchingStep4));
            }
        }
    }
}
