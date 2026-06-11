namespace Xunit.Gherkin.Quick.Tests.Features.ReuseStepsAcrossFeatures.BaseClassApproach;

[FeatureFile("./Features/ReuseStepsAcrossFeatures/Concatenation.feature")]
public sealed class Concatenation : ConcatenationBase
{
    [When(@"I ask to concatenate")]
    public void When_I_ask_to_concatenate()
    {
        //HACK: must call an application to calculate result.
        base.SetConcatenationResult($"{base.FirstName} {base.LastName}");
    }
}