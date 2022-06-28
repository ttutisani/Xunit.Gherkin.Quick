using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Gherkin.Quick.ProjectConsumer.ReuseStepsAcrossFeatures.BaseClassApproach
{
    [FeatureFile("./ReuseStepsAcrossFeatures/Concatenation.feature")]
    public class Concatenation : ConcatenationBase
    {
        [FeatureFile("./ReuseStepsAcrossFeatures/Concatenation.em.feature")]
        public class Emoji : Concatenation { }

        [When(@"I ask to concatenate")]
        public void When_I_ask_to_concatenate()
        {
            //HACK: must call an application to calculate result.
            base.SetConcatenationResult($"{base.FirstName} {base.LastName}");
        }
    }
}
