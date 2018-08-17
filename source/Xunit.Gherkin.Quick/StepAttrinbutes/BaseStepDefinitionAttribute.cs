using System;
using System.Text.RegularExpressions;

namespace Xunit.Gherkin.Quick
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class BaseStepDefinitionAttribute : Attribute
    {
        public string Keyword { get; }

        public string Pattern { get; }

        protected BaseStepDefinitionAttribute(string keyword, string pattern)
        {
            Keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        }

        public bool MatchesStep(string keyword, string text)
        {
            return Keyword.Equals(keyword.Trim(), StringComparison.OrdinalIgnoreCase)
                && Regex.IsMatch(text.Trim(), Pattern, RegexOptions.IgnoreCase);
        }

        public Match MatchRegex(string text)
        {
            return Regex.Match(text.Trim(), Pattern, RegexOptions.IgnoreCase);
        }
    }
}
