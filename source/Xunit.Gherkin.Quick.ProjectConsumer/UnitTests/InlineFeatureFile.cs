using System.IO;
using Xunit.Gherkin.Quick.vNext.FeatureFiles;

namespace Xunit.Gherkin.Quick.ProjectConsumer.UnitTests;

internal class InlineFeatureFile(string name, string fullName, string contents) : IFeatureFile
{
    public string Name
        => name;

    public string FullName 
        => fullName;

    public TextReader OpenRead()
        => new StringReader(contents);
}