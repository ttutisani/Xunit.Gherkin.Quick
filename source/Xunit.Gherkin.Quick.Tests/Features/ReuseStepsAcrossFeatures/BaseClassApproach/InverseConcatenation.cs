namespace Xunit.Gherkin.Quick.Tests.Features.ReuseStepsAcrossFeatures.BaseClassApproach;

[FeatureFile("./Features/ReuseStepsAcrossFeatures/InverseConcatenation.feature")]
public sealed class InverseConcatenation : ConcatenationBase
{
    [When(@"I ask to inverse concatenate")]
    public void When_I_ask_to_inverse_concatenate()
    {
        //HACK: must call an application to calculate result.
        base.SetConcatenationResult($"{base.LastName}, {base.FirstName}");
    }
}