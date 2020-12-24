using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Gherkin.Quick.ProjectConsumer.ReuseStepsAcrossFeatures
{
    [FeatureFile("./ReuseStepsAcrossFeatures/Concatenation.feature")]
    public sealed class Concatenation : ConcatenationBase
    {
        [When(@"I ask to concatenate")]
        public void When_I_ask_to_concatenate()
        {
            base.SetConcatenationResult($"{base.FirstName} {base.LastName}");
        }
    }
}
