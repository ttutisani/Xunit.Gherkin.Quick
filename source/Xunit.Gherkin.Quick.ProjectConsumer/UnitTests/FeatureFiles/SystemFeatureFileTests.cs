using System;
using System.IO;
using Xunit.Gherkin.Quick.vNext.FeatureFiles;

namespace Xunit.Gherkin.Quick.ProjectConsumer.UnitTests.FeatureFiles;

public class SystemFeatureFileTests
{
    [Fact]
    public void Initialize_SetsFileName()
    {
        var systemFeatureFile = new SystemFeatureFile(new DirectoryInfo(Environment.CurrentDirectory), new FileInfo(Path.Join(Environment.CurrentDirectory, "Addition_ForMultipleUseCases", "AddTwoNumbers.feature")));

        Assert.Equal("AddTwoNumbers.feature", systemFeatureFile.Name);
        Assert.Equal("./Addition_ForMultipleUseCases/AddTwoNumbers.feature", systemFeatureFile.FullName);
    }

    [Fact]
    public void OpenRead_ReadsFileContents()
    {
        var systemFeatureFile = new SystemFeatureFile(new DirectoryInfo(Environment.CurrentDirectory), new FileInfo(Path.Join(Environment.CurrentDirectory, "Addition_ForMultipleUseCases", "AddTwoNumbers.feature")));

        string fileContents = null;
        using (var embeddedFeatureFileReader = systemFeatureFile.OpenRead())
            fileContents = embeddedFeatureFileReader.ReadToEnd();

        Assert.NotEmpty(fileContents);
        Assert.Contains("feature:", fileContents, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Initialize_WhenFileNameIsOutsideOfRootDirectory_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(
            "featureFileInfo",
            () => new SystemFeatureFile(
                new DirectoryInfo(Path.Join(Environment.CurrentDirectory, "Async")),
                new FileInfo(Path.Join(Environment.CurrentDirectory, "Addition_ForMultipleUseCases", "AddTwoNumbers.feature"))
            )
        );

        Assert.Equal(new ArgumentException("Feature file info is not placed inside of the root content directory.", "featureFileInfo").Message, exception.Message);
    }

    [Fact]
    public void Initialize_NormalizezFullName()
    {
        var systemFeatureFile = new SystemFeatureFile(new DirectoryInfo(Environment.CurrentDirectory), new FileInfo(Environment.CurrentDirectory + "\\Addition_ForMultipleUseCases\\AddTwoNumbers.feature"));

        Assert.Equal("AddTwoNumbers.feature", systemFeatureFile.Name);
        Assert.Equal("./Addition_ForMultipleUseCases/AddTwoNumbers.feature", systemFeatureFile.FullName);
    }
}