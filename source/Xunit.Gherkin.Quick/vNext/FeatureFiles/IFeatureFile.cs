using System.IO;

namespace Xunit.Gherkin.Quick.vNext.FeatureFiles
{
    internal interface IFeatureFile
    {
        string Name { get; }

        string FullName { get; }

        TextReader OpenRead();
    }
}