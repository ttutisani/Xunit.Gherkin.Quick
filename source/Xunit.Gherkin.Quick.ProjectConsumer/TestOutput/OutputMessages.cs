using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.ProjectConsumer.TestOutput
{
    [FeatureFile(@"./TestOutput/OutputMessages.feature")]
    public class OutputMessages : Feature
    {
        [FeatureFile(@"./TestOutput/OutputMessages.em.feature")]
        public class Emoji : OutputMessages { 

            public Emoji(ITestOutputHelper helper) : base(helper) { }

        }

        private readonly ITestOutputHelper _testOutputHelper;

        public OutputMessages(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Given(@"output is available")]
        public void Given_output_is_available()
        {
            Assert.NotNull(_testOutputHelper);
        }

        [When(@"I log message ""(.+)""")]
        public void Given_message(string message)
        {
            _testOutputHelper.WriteLine(message);
        }

        [Then(@"I should see it in the output")]
        public void Then_I_should_see_it_in_the_output()
        {
            //check the output.
        }
    }
}
