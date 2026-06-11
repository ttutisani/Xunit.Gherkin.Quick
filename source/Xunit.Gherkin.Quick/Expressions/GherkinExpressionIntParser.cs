using System;
using System.Globalization;

namespace Xunit.Gherkin.Quick.Expressions
{
    internal class GherkinExpressionIntParser : GherkinExpressionParser<int>
    {
        public override string Name
            => "int";

        public override bool TryParse(string rawValue, IFormatProvider provider, out int value)
            => int.TryParse(rawValue, NumberStyles.Integer, provider, out value);
    }
}