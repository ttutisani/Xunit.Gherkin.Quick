using System;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.ProjectConsumer.BeforeAfterHooks
{
    [FeatureFile("./BeforeAfterHooks/BeforeAfter.feature")]
    public sealed class BeforeAfter : Feature, IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BeforeAfter(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            _testOutputHelper.WriteLine("Before executing scenario.");
        }

        [Given("this scenario executed")]
        public void This_Scenario_Executed()
        {
            _testOutputHelper.WriteLine("While executing scenario.");
        }

        public void Dispose()
        {
            _testOutputHelper.WriteLine("After scenario execution.");
        }
    }
}
