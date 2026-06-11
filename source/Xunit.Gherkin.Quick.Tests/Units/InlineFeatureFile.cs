using System.IO;
using Xunit.Gherkin.Quick.FeatureFiles;

namespace Xunit.Gherkin.Quick.Tests.Units;

internal class InlineFeatureFile(string name, string fullName, string contents) : IFeatureFile
{
    public string Name
        => name;

    public string FullName 
        => fullName;

    public TextReader OpenRead()
        => new StringReader(contents);
}