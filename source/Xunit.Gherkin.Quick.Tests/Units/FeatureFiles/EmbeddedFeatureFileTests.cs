using System;
using System.Linq;
using Xunit.Gherkin.Quick.FeatureFiles;

namespace Xunit.Gherkin.Quick.Tests.Units.FeatureFiles;

public class EmbeddedFeatureFileTests
{
    [Theory]
    [InlineData("test", "test")]
    [InlineData("test.feature", "test.feature")]
    [InlineData("test1.test2.feature", "test2.feature")]
    [InlineData("test1.test2.test3.feature", "test3.feature")]
    [InlineData("test1.test2.test3.test4.feature", "test4.feature")]
    [InlineData("test1.test2", "test1.test2")]
    public void Initialize_SetsFileName(string resourceFileName, string featureFileName)
    {
        var embeddedFeatureFile = new EmbeddedFeatureFile(typeof(EmbeddedFeatureFileTests).Assembly, resourceFileName);

        Assert.Multiple(
            () => Assert.Equal(featureFileName, embeddedFeatureFile.Name),
            () => Assert.Equal(resourceFileName, embeddedFeatureFile.FullName)
        );
    }

    [Fact]
    public void OpenRead_ReadsFileContents()
    {
        var testAssembly = typeof(EmbeddedFeatureFileTests).Assembly;
        var embeddedFeatureFile = new EmbeddedFeatureFile(
            testAssembly,
            testAssembly.GetManifestResourceNames().First(embeddedFileName => embeddedFileName.EndsWith(".feature"))
        );

        string fileContents = null;
        using (var embeddedFeatureFileReader = embeddedFeatureFile.OpenRead())
            fileContents = embeddedFeatureFileReader.ReadToEnd();

        Assert.NotEmpty(fileContents);
        Assert.Contains("feature:", fileContents, StringComparison.OrdinalIgnoreCase);
    }
}