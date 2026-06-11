using System;
using System.Collections.Generic;
using System.Globalization;

namespace Xunit.Gherkin.Quick.Expressions
{
    internal class GherkinExpressionLongParser : GherkinExpressionParser<long>
    {
        public override string Name
            => "int";

        public override IEnumerable<string> Aliases { get; } = new[] { "long" };

        public override bool TryParse(string rawValue, IFormatProvider provider, out long value)
            => long.TryParse(rawValue, NumberStyles.Integer, provider, out value);
    }
}