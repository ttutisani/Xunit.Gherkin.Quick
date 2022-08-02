using System;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.ProjectConsumer.Emoji
{
    [FeatureFile("./BeforeAfterHooks/BeforeAfter.em.feature")]
    public class BeforeAfter : BeforeAfterHooks.BeforeAfter
    {

        public BeforeAfter(ITestOutputHelper helper) : base(helper){ }

    }
}

namespace Xunit.Gherkin.Quick.ProjectConsumer.BeforeAfterHooks
{
    [FeatureFile("./BeforeAfterHooks/BeforeAfter.feature")]
    public class BeforeAfter : Feature, IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BeforeAfter(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            _testOutputHelper.WriteLine("Before executing scenario.");
        }

        [Given("first step executed")]
        public void First_Step_Executed()
        {
            _testOutputHelper.WriteLine("First step.");
        }

        [And("second step executed")]
        public void Second_Step_Executed()
        {
            _testOutputHelper.WriteLine("Second step.");
        }

        public void Dispose()
        {
            _testOutputHelper.WriteLine("After scenario execution.");
        }
    }
}
