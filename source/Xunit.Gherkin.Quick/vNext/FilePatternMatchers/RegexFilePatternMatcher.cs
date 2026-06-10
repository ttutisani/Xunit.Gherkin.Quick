using System.Text.RegularExpressions;

namespace Xunit.Gherkin.Quick.vNext.FilePatternMatchers
{
    internal class RegexFilePatternMatcher : IFilePatternMatcher
    {
        private readonly Regex _pathRegex;

        public RegexFilePatternMatcher(string pathRegex, RegexOptions regexOptions = RegexOptions.IgnoreCase)
            => _pathRegex = new Regex(pathRegex, regexOptions);

        public bool Matches(string filePath)
            => _pathRegex.IsMatch(filePath);
    }
}
