using System;
using System.Collections.Generic;
using System.Linq;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioStepPattern
    {
        private readonly string _pattern;
        private readonly StepMethodKind _stepMethodKind;

        public ScenarioStepPattern(string pattern, StepMethodKind stepMethodKind)
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
                    StepMethodKindExtensions.ToStepMethodKind(attribute)))
                .ToList();
        }
    }
}
