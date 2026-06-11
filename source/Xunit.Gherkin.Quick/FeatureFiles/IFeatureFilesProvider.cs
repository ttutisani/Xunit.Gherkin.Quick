using System.Collections.Generic;

namespace Xunit.Gherkin.Quick.FeatureFiles
{
    internal interface IFeatureFilesProvider
    {
        IEnumerable<IFeatureFile> GetFeatureFiles();
    }
}