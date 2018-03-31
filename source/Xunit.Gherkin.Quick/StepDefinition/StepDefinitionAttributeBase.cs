﻿using System;
using System.Text.RegularExpressions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Xunit.Gherkin.Quick.IgnoreMethodDiscoverer", "Xunit.Gherkin.Quick")]
    public abstract class StepDefinitionAttributeBase : FactAttribute
    {
        public string Keyword { get; }

        public string Pattern { get; }

        protected StepDefinitionAttributeBase(string keyword, string pattern)
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
