using System;
using System.Collections.Generic;
using System.Linq;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioStepPattern
    {
        private readonly string _pattern;

        /// <summary>
        /// Original pattern as supplied from step attributes.
        /// </summary>
        public string Pattern { get { return _pattern; } }

        private readonly string _regexPattern;
        /// <summary>
        /// Original pattern where cucumber specific patterns (such as "{word}") have been replaced with regex patterns.
        /// </summary>
        public string RegexPattern { get { return _regexPattern; } }

        public PatternKind Kind { get; }

        public ScenarioStepPattern(string pattern, PatternKind stepMethodKind)
        {
            _pattern = !string.IsNullOrWhiteSpace(pattern) 
                ? pattern 
                : throw new ArgumentNullException(nameof(pattern));
            _regexPattern = ConvertSpecialPatternsToRegex(_pattern);
            Kind = stepMethodKind;
        }

        private string ConvertSpecialPatternsToRegex(string pattern)
        {
            string p = pattern.Replace("{word}", @"(\w+)");
            p = p.Replace("{int}", @"([+-]?\d+)");
            p = p.Replace("{float}", @"([+-]?([0-9]*[.])?[0-9]+)");
            p = p.Replace("{string}", @"""([^""]*)""");
            p = p.Replace("{}", @"(.*)");
            return p;
        }

        public static List<ScenarioStepPattern> ListFromStepAttributes(IEnumerable<BaseStepDefinitionAttribute> baseStepDefinitionAttributes)
        {
            return baseStepDefinitionAttributes.Select(attribute => 
                new ScenarioStepPattern(
                    attribute.Pattern, 
                    PatternKindExtensions.ToPatternKind(attribute)))
                .ToList();
        }
    }
}
