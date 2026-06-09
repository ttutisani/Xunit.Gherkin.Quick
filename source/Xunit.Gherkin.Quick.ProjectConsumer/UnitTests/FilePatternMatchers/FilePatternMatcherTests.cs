using System;
using Xunit.Gherkin.Quick.vNext.FilePatternMatchers;

namespace Xunit.Gherkin.Quick.ProjectConsumer.UnitTests.FilePatternMatchers;

public class FilePatternMatcherTests
{
    [Theory]
    [InlineData("./file.txt", "./file.txt", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("./file.txt", "./FILE.TXT", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("./file.txt", "./file.txt", StringComparison.Ordinal, true)]
    [InlineData("./file.txt", "./FILE.TXT", StringComparison.Ordinal, false)]
    [InlineData("./file.txt", "file.txt", StringComparison.OrdinalIgnoreCase, false)]
    [InlineData("./file.txt", "../directory/file.txt", StringComparison.OrdinalIgnoreCase, false)]
    public void StrictFilePatternMatcher_Matches(string filePattern, string filePath, StringComparison stringComparison, bool expectedMatch)
    {
        var matcher = new StrictFilePatternMatcher(filePattern, stringComparison);

        var matches = matcher.Matches(filePath);

        if (expectedMatch)
            Assert.True(matches);
        else
            Assert.False(matches);
    }

    [Theory]
    [InlineData("file.txt", "./file.txt", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("file.txt", "./FILE.TXT", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("file.txt", "./file.txt", StringComparison.Ordinal, true)]
    [InlineData("file.txt", "./FILE.TXT", StringComparison.Ordinal, false)]
    [InlineData("file.txt", "file.txt", StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("file.txt", "../directory/file.txt", StringComparison.OrdinalIgnoreCase, true)]
    public void EndsWithFilePatternMatcher_Matches(string filePattern, string filePath, StringComparison stringComparison, bool expectedMatch)
    {
        var matcher = new EndsWithFilePatternMatcher(filePattern, stringComparison);

        var matches = matcher.Matches(filePath);

        if (expectedMatch)
            Assert.True(matches);
        else
            Assert.False(matches);
    }
}