using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Gherkin.Quick.ProjectConsumer.ReuseStepsAcrossFeatures.InjectionApproach
{
    [FeatureFile("./ReuseStepsAcrossFeatures/Concatenation.feature")]
    public sealed class Concatenation : Feature, IClassFixture<ConcatenationCommonSteps>
    {
        private readonly ConcatenationCommonSteps _steps;

        public Concatenation(ConcatenationCommonSteps steps)
        {
            _steps = steps;
        }

        [Given(@"I type ""([\w]+)""")]
        public void Given_I_type(string firstName) => _steps.Given_I_type(firstName);

        [And(@"I type ""([\w]+)""")]
        public void And_I_type(string lastName) => _steps.And_I_type(lastName);

        [When(@"I ask to concatenate")]
        public void When_I_ask_to_concatenate()
        {
            //Hack: must call an application to calculate result.
            _steps.SetConcatenationResult($"{_steps.FirstName} {_steps.LastName}");
        }

        [Then(@"I receive ""([\w\s,]+)""")]
        public void Then_I_receive(string fullName) => _steps.Then_I_receive(fullName);
    }
}
