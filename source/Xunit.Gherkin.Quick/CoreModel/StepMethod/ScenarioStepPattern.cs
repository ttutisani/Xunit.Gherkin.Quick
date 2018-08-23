using System;
using System.Collections.Generic;
using System.Linq;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioStepPattern
    {
        private readonly string _pattern;
        public string Pattern { get { return _pattern; } }

        private readonly PatternKind _stepMethodKind;
        public PatternKind Kind { get { return _stepMethodKind; } }

        public ScenarioStepPattern(string pattern, PatternKind stepMethodKind)
        {
            _pattern = !string.IsNullOrWhiteSpace(pattern) 
                ? pattern 
                : throw new ArgumentNullException(nameof(pattern));
            _stepMethodKind = stepMethodKind;
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
