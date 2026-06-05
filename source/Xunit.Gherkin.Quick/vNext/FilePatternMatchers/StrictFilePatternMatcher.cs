using System;

namespace Xunit.Gherkin.Quick.vNext.FilePatternMatchers
{
    internal class StrictFilePatternMatcher : IFilePatternMatcher
    {
        private readonly string _pattern;
        private readonly StringComparison _stringComparison;

        public StrictFilePatternMatcher(string pattern, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            _pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            _stringComparison = stringComparison;
        }

        public bool Matches(string filePath)
            => filePath?.Equals(_pattern, _stringComparison) ?? throw new ArgumentNullException(nameof(filePath));

        public override string ToString()
            => new { _pattern }.ToString();
    }
}