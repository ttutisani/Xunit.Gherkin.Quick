using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Gherkin.Quick.ProjectConsumer.Emoji
{

    using Xunit.Gherkin.Quick.ProjectConsumer.ReuseStepsAcrossFeatures.InjectionApproach;

    public partial class ReuseStepsAcrossFeatures
    {
        public partial class InjectionApproach
        {
            [FeatureFile("./ReuseStepsAcrossFeatures/InverseConcatenation.em.feature")]
            public class InverseConcatenation : ProjectConsumer.ReuseStepsAcrossFeatures.InjectionApproach.InverseConcatenation { 

                public InverseConcatenation(ConcatenationCommonSteps steps) : base(steps) { }

            }
        }
    }
}

namespace Xunit.Gherkin.Quick.ProjectConsumer.ReuseStepsAcrossFeatures.InjectionApproach
{
    [FeatureFile("./ReuseStepsAcrossFeatures/InverseConcatenation.feature")]
    public class InverseConcatenation : Feature, IClassFixture<ConcatenationCommonSteps>
    {
        private readonly ConcatenationCommonSteps _steps;

        public InverseConcatenation(ConcatenationCommonSteps steps)
        {
            _steps = steps;
        }

        [Given(@"I type ""([\w]+)""")]
        public void Given_I_type(string firstName) => _steps.Given_I_type(firstName);

        [And(@"I type ""([\w]+)""")]
        public void And_I_type(string lastName) => _steps.And_I_type(lastName);

        [When(@"I ask to inverse concatenate")]
        public void When_I_ask_to_inverse_concatenate()
        {
            //HACK: must call an application to calculate result.
            _steps.SetConcatenationResult($"{_steps.LastName}, {_steps.FirstName}");
        }

        [Then(@"I receive ""([\w\s,]+)""")]
        public void Then_I_receive(string fullName) => _steps.Then_I_receive(fullName);
    }
}
