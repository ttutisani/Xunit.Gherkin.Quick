using System;

namespace Xunit.Gherkin.Quick.vNext.FilePatternMatchers
{
    internal interface IFilePatternMatcher
    {
        bool Matches(string filePath);
    }
}