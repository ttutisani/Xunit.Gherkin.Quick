using System;

namespace Xunit.Gherkin.Quick.FilePatternMatchers
{
    internal class EndsWithFilePatternMatcher : IFilePatternMatcher
    {
        private readonly string _pattern;
        private readonly StringComparison _stringComparison;

        public EndsWithFilePatternMatcher(string pattern, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            _pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            _stringComparison = stringComparison;
        }

        public bool Matches(string filePath)
            => filePath?.EndsWith(_pattern, _stringComparison) ?? throw new ArgumentNullException(nameof(filePath));
    }
}