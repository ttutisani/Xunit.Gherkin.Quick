using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Gherkin.Quick.ProjectConsumer.ReuseStepsAcrossFeatures
{
    public sealed class ConcatenationCommonSteps
    {
        public string FirstName { get; private set; }

        public void Given_I_type(string firstName)
        {
            FirstName = firstName;
        }

        public string LastName { get; private set; }

        public void And_I_type(string lastName)
        {
            LastName = lastName;
        }

        public string ConcatenationRsult { get; private set; }

        public void SetConcatenationResult(string result)
        {
            ConcatenationRsult = result;
        }

        public void Then_I_receive(string fullName)
        {
            Assert.Equal(fullName, ConcatenationRsult);
        }
    }
}
