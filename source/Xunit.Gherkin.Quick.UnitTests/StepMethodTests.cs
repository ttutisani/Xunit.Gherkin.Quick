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
            var sut = StepMethod.FromStepMethodInfo(stepMethodInfo, new Gherkin.Ast.Step(null, "But", "what 123 exactly", null));

            //act.
            await sut.ExecuteAsync();

            //assert.
            Assert.True(featureInstance.Called);
            Assert.Equal(123, featureInstance.Value);
        }

        private sealed class Feature_For_ExecuteAsync_Test : Feature
        {
            public bool Called { get; private set; }

            public int Value { get; private set; }

            [But(@"what (\d+) exactly")]
            public void But_This_Method(int value)
            {
                Called = true;
                Value = value;
            }
        }

        [Theory]
        [InlineData("Given")]
        [InlineData("*")]
        public void FromStepMethodInfo_Creates_Instance_When_Step_Matches(string keyword)
        {
            //arrange.
            var featureInstance = new Feature_For_FromStepMethodInfo();
            var stepMethodInfo = StepMethodInfo.FromMethodInfo(
                featureInstance.GetType().GetMethod(nameof(Feature_For_FromStepMethodInfo.Step_With_Multiple_Patterns)),
                featureInstance
                );

            //act.
            var sut = StepMethod.FromStepMethodInfo(stepMethodInfo, new Gherkin.Ast.Step(null, keyword, "something 123 else", null));

            //assert.
            Assert.NotNull(sut);
            Assert.Equal(stepMethodInfo.ScenarioStepPatterns[1].Kind, sut.Kind);
            Assert.Equal(stepMethodInfo.ScenarioStepPatterns[1].OriginalPattern, sut.Pattern);
            Assert.Equal("something 123 else", sut.StepText);
        }

        private sealed class Feature_For_FromStepMethodInfo : Feature
        {
            [Given(@"something (\d+) exact")]
            [Given(@"something (\d+) else")]
            public void Step_With_Multiple_Patterns(int value)
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
