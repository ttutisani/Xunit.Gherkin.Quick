using System.Collections.Generic;

namespace Xunit.Gherkin.Quick.vNext.FeatureFiles
{
    internal interface IFeatureFilesProvider
    {
        IEnumerable<IFeatureFile> GetFeatureFiles();
    }
}