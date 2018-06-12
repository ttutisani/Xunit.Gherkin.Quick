using Moq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class ScenarioOutputTests
    {
        [Fact]
        public void StepPassed_Logs_Into_Output()
        {
            //arrange.
            var output = new Mock<ITestOutputHelper>();

            var stepText = "Given something 123";
            var sut = new ScenarioOutput(output.Object);

            //act.
            sut.StepPassed(stepText);

            //assert.
            output.Verify(o => o.WriteLine($"{stepText}: PASSED"), Times.Once);
        }

        [Fact]
        public void StepFailed_Logs_Into_Output()
        {
            //arrange.
            var output = new Mock<ITestOutputHelper>();

            var stepText = "Given something 123";
            var sut = new ScenarioOutput(output.Object);

            //act.
            sut.StepFailed(stepText);

            //assert.
            output.Verify(o => o.WriteLine($"{stepText}: FAILED"), Times.Once);
        }

        [Fact]
        public void StepSkipped_Logs_Into_Output()
        {
            //arrange.
            var output = new Mock<ITestOutputHelper>();

            var stepText = "Given something 123";
            var sut = new ScenarioOutput(output.Object);

            //act.
            sut.StepSkipped(stepText);

            //assert.
            output.Verify(o => o.WriteLine($"{stepText}: SKIPPED"), Times.Once);
        }
    }
}
