using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class StepMethodTests
    {
        [Fact]
        public async Task ExecuteAsync_Executes_StepMethodInfo()
        {
            //arrange.
            var featureInstance = new Feature_For_ExecuteAsync_Test();
            var stepMethodInfo = StepMethodInfo.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(Feature_For_ExecuteAsync_Test.But_This_Method)), featureInstance);
            var sut = StepMethod.FromStepMethodInfo(stepMethodInfo, new Gherkin.Ast.Step(null, "But", "what", null));

            //act.
            await sut.ExecuteAsync();

            //assert.
            Assert.True(featureInstance.Called);
        }

        private sealed class Feature_For_ExecuteAsync_Test : Feature
        {
            public bool Called { get; private set; }

            [But("what")]
            public void But_This_Method()
            {
                Called = true;
            }
        }

        [Fact]
        public void FromStepMethodInfo_Creates_Instance()
        {
            //arrange.
            var featureInstance = new Feature_For_FromStepMethodInfo();
            var stepMethodInfo = StepMethodInfo.FromMethodInfo(
                featureInstance.GetType().GetMethod(nameof(Feature_For_FromStepMethodInfo.Step_With_Multiple_Patterns)),
                featureInstance
                );

            //act.
            var sut = StepMethod.FromStepMethodInfo(stepMethodInfo, new Gherkin.Ast.Step(null, "Given", "something else", null));

            //assert.
            Assert.NotNull(sut);
            Assert.Equal(stepMethodInfo.ScenarioStepPatterns[1].Kind, sut.Kind);
            Assert.Equal(stepMethodInfo.ScenarioStepPatterns[1].Pattern, sut.Pattern);
            Assert.Equal("something else", sut.StepText);
        }

        private sealed class Feature_For_FromStepMethodInfo : Feature
        {
            [Given("something")]
            [Given("something else")]
            public void Step_With_Multiple_Patterns()
            {

            }
        }

        [Fact]
        public void FromStepMethodInfo_Throws_When_Method_Cannot_Match_Pattern()
        {
            //arrange.
            var featureInstance = new Feature_For_FromStepMethodInfo();
            var stepMethodInfo = StepMethodInfo.FromMethodInfo(
                featureInstance.GetType().GetMethod(nameof(Feature_For_FromStepMethodInfo.Step_With_Multiple_Patterns)),
                featureInstance
                );

            //act / assert.
            Assert.Throws<InvalidOperationException>(() => StepMethod.FromStepMethodInfo(stepMethodInfo, new Gherkin.Ast.Step(null, "Given", "something else NOT", null)));
        }
    }
}
