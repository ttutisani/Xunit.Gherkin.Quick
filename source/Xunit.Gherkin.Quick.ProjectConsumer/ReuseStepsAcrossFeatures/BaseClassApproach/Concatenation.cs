using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Gherkin.Quick.ProjectConsumer.ReuseStepsAcrossFeatures.BaseClassApproach
{
    [FeatureFile("./ReuseStepsAcrossFeatures/Concatenation.feature")]
    public sealed class Concatenation : ConcatenationBase
    {
        [When(@"I ask to concatenate")]
        public void When_I_ask_to_concatenate()
        {
            //Hack: must call an application to calculate result.
            base.SetConcatenationResult($"{base.FirstName} {base.LastName}");
        }
    }
}
