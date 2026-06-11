using System;

namespace Xunit.Gherkin.Quick.FilePatternMatchers
{
    internal interface IFilePatternMatcher
    {
        bool Matches(string filePath);
    }
}