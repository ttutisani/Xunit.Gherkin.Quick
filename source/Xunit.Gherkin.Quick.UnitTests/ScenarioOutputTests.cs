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
    }
}
