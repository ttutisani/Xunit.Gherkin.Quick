namespace Xunit.Gherkin.Quick
{
    internal interface IScenarioOutput
    {
        void StepPassed(string stepText);

        void StepFailed(string stepText);

        void StepSkipped(string stepText);
    }
}
