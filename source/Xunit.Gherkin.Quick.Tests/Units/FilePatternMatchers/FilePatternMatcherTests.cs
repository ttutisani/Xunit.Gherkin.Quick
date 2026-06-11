using System;
using System.Text.RegularExpressions;
using Xunit.Gherkin.Quick.FilePatternMatchers;

namespace Xunit.Gherkin.Quick.Tests.Units.FilePatternMatchers;

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

    [Theory]
    [InlineData(@"file\.txt", "./file.txt", RegexOptions.IgnoreCase, true)]
    [InlineData(@".+", "./file.txt", RegexOptions.IgnoreCase, true)]
    [InlineData(@".+\.txt", "./file.txt", RegexOptions.IgnoreCase, true)]
    [InlineData(@".*\.txt", ".txt", RegexOptions.IgnoreCase, true)]
    [InlineData(@".+\.txt", ".txt", RegexOptions.IgnoreCase, false)]
    [InlineData(@"[a-z]+\.txt", "./file.txt", RegexOptions.IgnoreCase, true)]
    [InlineData(@"[a-z]+\.txt", "./FILE.TXT", RegexOptions.IgnoreCase, true)]
    [InlineData(@"[a-z]+\.txt", "./FILE.TXT", RegexOptions.None, false)]
    [InlineData(@"[a-z]+\.feature", "./file.feature.txt", RegexOptions.IgnoreCase, true)]
    [InlineData(@"[a-z]+\.feature$", "./file.feature.txt", RegexOptions.IgnoreCase, false)]
    public void RegexFilePatternMatcher_Matches(string filePattern, string filePath, RegexOptions regexOptions, bool expectedMatch)
    {
        var matcher = new RegexFilePatternMatcher(filePattern, regexOptions);

        var matches = matcher.Matches(filePath);

        if (expectedMatch)
            Assert.True(matches);
        else
            Assert.False(matches);
    }
}