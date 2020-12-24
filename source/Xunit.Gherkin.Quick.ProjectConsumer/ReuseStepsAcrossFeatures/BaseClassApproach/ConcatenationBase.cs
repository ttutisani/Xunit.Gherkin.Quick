using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Gherkin.Quick.ProjectConsumer.ReuseStepsAcrossFeatures.BaseClassApproach
{
    public abstract class ConcatenationBase : Feature
    {
        protected string FirstName { get; private set; }

        [Given(@"I type ""([\w]+)""")]
        public void Given_I_type(string firstName)
        {
            FirstName = firstName;
        }

        protected string LastName { get; private set; }

        [And(@"I type ""([\w]+)""")]
        public void And_I_type(string lastName)
        {
            LastName = lastName;
        }

        private string _concatenationRsult;

        //hack: this will not need to exist in real tests.
        protected void SetConcatenationResult(string result)
        {
            _concatenationRsult = result;
        }

        [Then(@"I receive ""([\w\s,]+)""")]
        public void Then_I_receive(string fullName)
        {
            Assert.Equal(fullName, _concatenationRsult);
        }
    }
}
