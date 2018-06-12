namespace Xunit.Gherkin.Quick
{
    internal interface IFeatureFileRepository
    {
        FeatureFile GetByFilePath(string filePath);
    }
}
