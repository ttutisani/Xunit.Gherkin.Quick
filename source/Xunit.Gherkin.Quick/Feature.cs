using System;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick.Evaluators;
using Xunit.Gherkin.Quick.Hooks;
using Xunit.Gherkin.Quick.TestScenarios;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    /// <summary>
    /// Base class for feature classes.
    /// Derived classes should define scenario step methods by using
    /// <see cref="GivenAttribute"/>, <see cref="WhenAttribute"/>, 
    /// <see cref="ThenAttribute"/>, <see cref="AndAttribute"/>, 
    /// and <see cref="ButAttribute"/>.
    /// Derived classes can also specify the feature text file by using
    /// <see cref="FeatureFileAttribute"/>.
    /// </summary>
    public abstract class Feature
    {
        internal TestScenario TestScenario { get; private set; }
        internal TestStep TestStep { get; private set; }

        [TestScenario]
        internal async Task TestScenarioAsync(ITestOutputHelper testOutputHelper, TestScenario testScenario)
        {
            var featureEvaluator = new FeatureEvaluator(this);

            TestScenario = testScenario;
            using (var testStep = testScenario.Steps.GetEnumerator())
                while (testStep.MoveNext())
                    try
                    {
                        await featureEvaluator.EvaluateStepAsync(testStep.Current);
                        testOutputHelper.WriteLine($"{testStep.Current.Text}: PASSED");
                    }
                    catch (Exception exception)
                    {
                        testOutputHelper.WriteLine($"{testStep.Current.Text}: FAILED");

                        while (testStep.MoveNext())
                            testOutputHelper.WriteLine($"{testStep.Current.Text}: SKIPPED");

                        if (exception is XunitException)
                            throw;
                        else
                            throw new TestScenarioException("An unhandled exception was thrown while evaluating scenario.", exception);
                    }
        }
    }
}