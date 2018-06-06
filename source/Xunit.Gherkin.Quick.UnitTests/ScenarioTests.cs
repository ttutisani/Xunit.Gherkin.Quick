using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class ScenarioTests
    {
        [Fact]
        public void Ctor_Initializes_Properties()
        {
            //arrange.
            var featureInstance = new FeatureForCtorTest();
            var stepMethods = new List<StepMethod>
            {
                StepMethod.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForCtorTest.Then_Something)), featureInstance),
                StepMethod.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForCtorTest.When_Something)), featureInstance),
                StepMethod.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForCtorTest.Given_Something)), featureInstance)
            };

            //act.
            var sut = new Scenario(stepMethods);

            //assert.
            Assert.NotNull(sut.Steps);
            Assert.Equal(stepMethods, sut.Steps);
        }

        private sealed class FeatureForCtorTest : Feature
        {
            [Then("something")]
            public void Then_Something()
            {

            }

            [When("something")]
            public void When_Something()
            {

            }

            [Given("something")]
            public void Given_Something()
            {

            }
        }

        [Fact]
        public async Task Execute_Invokes_All_StepMethods()
        {
            //arrange.
            var featureInstance = new FeatureWithStepMethodsToInvoke();

            var sut = new Scenario(new List<StepMethod>
            {
                StepMethod.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureWithStepMethodsToInvoke.ScenarioStep1)), featureInstance),
                StepMethod.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureWithStepMethodsToInvoke.ScenarioStep2)), featureInstance),
                StepMethod.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureWithStepMethodsToInvoke.ScenarioStep3)), featureInstance),
                StepMethod.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureWithStepMethodsToInvoke.ScenarioStep4)), featureInstance)
            });

            //act.
            await sut.ExecuteAsync();

            //assert.
            Assert.NotNull(featureInstance.CallStack);
            Assert.Equal(4, featureInstance.CallStack.Count);
            Assert.Equal(nameof(FeatureWithStepMethodsToInvoke.ScenarioStep1), featureInstance.CallStack[0]);
            Assert.Equal(nameof(FeatureWithStepMethodsToInvoke.ScenarioStep2), featureInstance.CallStack[1]);
            Assert.Equal(nameof(FeatureWithStepMethodsToInvoke.ScenarioStep3), featureInstance.CallStack[2]);
            Assert.Equal(nameof(FeatureWithStepMethodsToInvoke.ScenarioStep4), featureInstance.CallStack[3]);
        }

        private sealed class FeatureWithStepMethodsToInvoke : Feature
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
