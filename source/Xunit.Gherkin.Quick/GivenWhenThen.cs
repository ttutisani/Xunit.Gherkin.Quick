using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Xunit.Gherkin.Quick.IgnoreMethodDiscoverer", "Xunit.Gherkin.Quick")]
    public abstract class KeyWordAttributeBase : FactAttribute
    {
        public string Keyword { get; }

        public string Pattern { get; }

        protected KeyWordAttributeBase(string keyword, string pattern)
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

    public sealed class IgnoreMethodDiscoverer : IXunitTestCaseDiscoverer
    {
        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            yield break;
        }
    }

    public sealed class GivenAttribute : KeyWordAttributeBase
    {
        public GivenAttribute(string pattern)
            : base("Given", pattern)
        {

        }
    }

    public sealed class WhenAttribute : KeyWordAttributeBase
    {
        public WhenAttribute(string pattern)
            : base("When", pattern)
        {

        }
    }

    public sealed class ThenAttribute : KeyWordAttributeBase
    {
        public ThenAttribute(string pattern)
            : base("Then", pattern)
        {

        }
    }

    public sealed class AndAttribute : KeyWordAttributeBase
    {
        public AndAttribute(string pattern)
            : base("And", pattern)
        {
        }
    }

    public sealed class ButAttribute : KeyWordAttributeBase
    {
        public ButAttribute(string pattern)
            : base("But", pattern)
        {
        }
    }
}
