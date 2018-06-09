using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioOutput : IScenarioOutput
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ScenarioOutput(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new System.ArgumentNullException(nameof(testOutputHelper));
        }

        public void StepPassed(string stepText)
        {
            _testOutputHelper.WriteLine($"{stepText}: PASSED");
        }
    }
}
