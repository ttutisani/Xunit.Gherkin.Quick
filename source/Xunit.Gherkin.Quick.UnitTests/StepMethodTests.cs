using System.Threading.Tasks;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class StepMethodTests
    {
        [Fact]
        public void Ctor_Initializes_Properties()
        {
            //arrange.
            var featureInstance = new Feature_For_Ctor_Test();
            var stepMethodInfo = StepMethodInfo.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(Feature_For_Ctor_Test.But_This_Method)), featureInstance);
            var stepText = "some step text";
            var sut = new StepMethod(stepMethodInfo, stepText);

            //act.
            Assert.Same(stepText, sut.StepText);
            //Assert.Equal(stepMethodInfo.Kind, sut.Kind);
        }

        private sealed class Feature_For_Ctor_Test : Feature
        {
            [But("what")]
            public void But_This_Method()
            { }
        }

        [Fact]
        public async Task ExecuteAsync_Executes_StepMethodInfo()
        {
            //arrange.
            var featureInstance = new Feature_For_ExecuteAsync_Test();
            var stepMethodInfo = StepMethodInfo.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(Feature_For_ExecuteAsync_Test.But_This_Method)), featureInstance);
            var sut = new StepMethod(stepMethodInfo, "some step text");

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
    }
}
