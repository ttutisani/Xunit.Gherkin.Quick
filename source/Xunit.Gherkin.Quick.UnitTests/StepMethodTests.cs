using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class StepMethodTests
    {
        [Fact]
        internal void Ctor_Initializes_Properties()
        {
            //arrange.
            var featureInstance = new FeatureForCtorTest();

            //act.
            var sut = StepMethod.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForCtorTest.When_Something)), featureInstance);

            //assert.
            Assert.Equal(StepMethodKind.When, sut.Kind);
            Assert.Equal(FeatureForCtorTest.WhenStepText, sut.Pattern);
            Assert.NotNull(sut.Arguments);
            Assert.Empty(sut.Arguments);
        }

        private sealed class FeatureForCtorTest : Feature
        {
            public const string WhenStepText = "some text 123";

            [When(WhenStepText)]
            public void When_Something()
            {

            }
        }

        [Fact]
        public void Execute_Invokes_StepMethod()
        {
            //arrange.
            var featureInstance = new FeatureForExecuteTest();
            var sut = StepMethod.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForExecuteTest.Call_This_Method)), featureInstance);

            //act.
            sut.Execute();

            //assert.
            Assert.True(featureInstance.Called);
        }

        private sealed class FeatureForExecuteTest : Feature
        {
            public bool Called { get; private set; } = false;

            [And("Call this")]
            public void Call_This_Method()
            {
                Called = true;
            }
        }
    }
}
