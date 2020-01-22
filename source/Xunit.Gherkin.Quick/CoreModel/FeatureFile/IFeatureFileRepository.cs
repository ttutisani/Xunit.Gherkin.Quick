using System.Collections.Generic;

namespace Xunit.Gherkin.Quick
{
    internal interface IFeatureFileRepository
    {
        FeatureFile GetByFilePath(string filePath);
        List<string> GetFeatureFilePaths();
    }
}
