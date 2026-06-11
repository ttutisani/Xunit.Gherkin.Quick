namespace Xunit.Gherkin.Quick.Tests.Features.TxtFileHandling;

[FeatureFile(@".*/FeatureInDatFile.dat", FeatureFilePathType.Regex)]
public sealed class FeatureInDatFile : Feature
{
    [Given("I have a feature in DAT file")]
    public void Given_I_have_feature_in_DAT_file() { }
}