using System;
using System.Text.RegularExpressions;

namespace Xunit.Gherkin.Quick
{
    /// <summary>
    /// Base class for attributes that associate step methods with
    /// scenario steps.
    /// You don't need to use this attribute directly (this is an abstract class anyway).
    /// Instead, use one of the derived attributes:
    /// <see cref="GivenAttribute"/>,
    /// <see cref="WhenAttribute"/>,
    /// <see cref="ThenAttribute"/>
    /// <see cref="AndAttribute"/>
    /// <see cref="ButAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class BaseStepDefinitionAttribute : Attribute
    {
        private string Keyword { get; }

        internal string Pattern { get; }

        /// <summary>
        /// Initializes new instance of <see cref="BaseStepDefinitionAttribute"/> class.
        /// </summary>
        /// <param name="keyword">Keyword that starts the scenario step (e.g. "Given").</param>
        /// <param name="pattern">Regex pattern that matches the scenario step text.</param>
        protected BaseStepDefinitionAttribute(string keyword, string pattern)
        {
            Keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        }

        internal bool MatchesStep(string keyword, string text)
        {
            return Keyword.Equals(keyword.Trim(), StringComparison.OrdinalIgnoreCase)
                && Regex.IsMatch(text.Trim(), Pattern, RegexOptions.IgnoreCase);
        }

        internal Match MatchRegex(string text)
        {
            return Regex.Match(text.Trim(), Pattern, RegexOptions.IgnoreCase);
        }
    }
}
