using System.IO;

namespace Xunit.Gherkin.Quick.FeatureFiles
{
    internal interface IFeatureFile
    {
        string Name { get; }

        string FullName { get; }

        TextReader OpenRead();
    }
}