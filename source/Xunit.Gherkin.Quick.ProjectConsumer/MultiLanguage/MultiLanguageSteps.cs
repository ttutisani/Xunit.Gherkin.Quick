using System.Collections.Generic;

namespace Xunit.Gherkin.Quick.ProjectConsumer.MultiLanguage
{
    [FeatureFile(@"MultiLanguage/.*\.feature", FeatureFilePathType.Regex)]
    public class MultiLanguageSteps : Feature
    {
        private readonly List<string> _executedSteps = [];

        [Given("given")]
        public void GivenStep()
        {
            _executedSteps.Add(nameof(GivenStep));
        }

        [And("and")]
        public void AndStep()
        {
            _executedSteps.Add(nameof(AndStep));
        }

        [But("but")]
        public void ButStep()
        {
            _executedSteps.Add(nameof(ButStep));
        }

        [When("when")]
        public void WhenStep()
        {
            _executedSteps.Add(nameof(WhenStep));
        }

        [Then("then")]
        public void ThenStep()
        {
            _executedSteps.Add(nameof(ThenStep));

            Assert.Equal(
                [
                    nameof(GivenStep),
                    nameof(AndStep),
                    nameof(ButStep),
                    nameof(WhenStep),
                    nameof(ThenStep)
                ],
                _executedSteps
            );
        }
    }
}