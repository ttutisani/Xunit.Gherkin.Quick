using System;
using System.Collections.Generic;
using System.Linq;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioStepPattern
    {
        private readonly string _originalPattern;

        /// <summary>
        /// Original pattern as supplied from step attributes.
        /// </summary>
        public string OriginalPattern { get { return _originalPattern; } }

        private readonly string _regexPattern;
        /// <summary>
        /// Original pattern where cucumber specific patterns (such as "{word}") have been replaced with regex patterns.
        /// </summary>
        public string RegexPattern { get { return _regexPattern; } }

        public PatternKind Kind { get; }

        public ScenarioStepPattern(string pattern, string regexPattern, PatternKind stepMethodKind)
        {
            _originalPattern = !string.IsNullOrWhiteSpace(pattern) 
                ? pattern 
                : throw new ArgumentNullException(nameof(pattern));
            _regexPattern = !string.IsNullOrWhiteSpace(regexPattern)
                ? regexPattern
                : throw new ArgumentNullException(nameof(regexPattern));
            Kind = stepMethodKind;
        }

        private static string ConvertSpecialPatternsToRegex(string pattern)
        {
            string p = pattern.Replace("{int}", @"([+-]?\d+)");
            p = p.Replace("{float}", @"([+-]?([0-9]*[.])?[0-9]+)");
            p = p.Replace("{word}", @"(\w+)");
            p = p.Replace("{string}", @"(?:""|')([^\1]*)(?:""|')");
            p = p.Replace("{}", @"(.*)");
            return p;
        }

        public static List<ScenarioStepPattern> ListFromStepAttributes(IEnumerable<BaseStepDefinitionAttribute> baseStepDefinitionAttributes)
        {
            return baseStepDefinitionAttributes.Select(attribute => 
                new ScenarioStepPattern(
                    attribute.Pattern,
                    ConvertSpecialPatternsToRegex(attribute.Pattern),
                    PatternKindExtensions.ToPatternKind(attribute)))
                .ToList();
        }
    }
}
