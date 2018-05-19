using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
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
        public void ExecuteScenario_Requires_FeatureInstance()
        {
            //act / assert.
            Assert.Throws<ArgumentNullException>(() => _sut.ExecuteScenario(null, "valid scenario name"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("                 ")]
        public void ExecuteScenario_Requires_ScenarioName(string scenarioName)
        {
            //arrange.
            var featureInstance = new UselessFeature();

            //act / assert.
            Assert.Throws<ArgumentNullException>(() => _sut.ExecuteScenario(featureInstance, scenarioName));
        }

        private sealed class UselessFeature : Feature { }

        [Fact]
        public void ExecuteScenario_Executes_Scenario_Steps()
        {
            //arrange.
            _featureFileRepository.Setup(r => r.GetByFilePath($"{nameof(FeatureWithScenarioSteps)}.feature"))
                .Returns(new FeatureFile(CreateGherkinDocument(
                    "Given " + FeatureWithScenarioSteps.ScenarioStep1Text,
                    "When " + FeatureWithScenarioSteps.ScenarioStep2Text,
                    "And " + FeatureWithScenarioSteps.ScenarioStep3Text,
                    "Then " + FeatureWithScenarioSteps.ScenarioStep4Text
                    )))
                .Verifiable();

            var featureInstance = new FeatureWithScenarioSteps();

            //act.
            _sut.ExecuteScenario(featureInstance, "scenario 12345");

            //assert.
            _featureFileRepository.Verify();

            Assert.Equal(4, featureInstance.CallStack.Count);
            Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep1), featureInstance.CallStack[0]);
            Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep2), featureInstance.CallStack[1]);
            Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep3), featureInstance.CallStack[2]);
            Assert.Equal(nameof(FeatureWithScenarioSteps.ScenarioStep4), featureInstance.CallStack[3]);
        }

        private static Gherkin.Ast.GherkinDocument CreateGherkinDocument(params string[] steps)
        {
            var gherkinText =
@"Feature: Some Sample Feature
    In order to learn Math
    As a regular human
    I want to add two numbers using Calculator

Scenario: Add two numbers
"+ string.Join(Environment.NewLine, steps)
;
            using (var gherkinStream = new MemoryStream(Encoding.UTF8.GetBytes(gherkinText)))
            using (var gherkinReader = new StreamReader(gherkinStream))
            {
                var parser = new Gherkin.Parser();
                return parser.Parse(gherkinReader);
            }
        }

        private sealed class FeatureWithScenarioSteps : Feature
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
