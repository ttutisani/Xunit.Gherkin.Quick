using System;
using System.Collections.Generic;
using System.Linq;

namespace Xunit.Gherkin.Quick.Expressions
{
    internal abstract class GherkinExpressionParser
    {
        public abstract string Name { get; }

        public virtual IEnumerable<string> Aliases { get; } = Enumerable.Empty<string>();

        public abstract Type TargetType { get; }

        public abstract bool TryParse(string rawValue, IFormatProvider provider, out object value);
    }

    internal abstract class GherkinExpressionParser<TValue> : GherkinExpressionParser
    {
        public sealed override Type TargetType
            => typeof(TValue);

        public abstract bool TryParse(string rawValue, IFormatProvider formatProvider, out TValue value);

        public sealed override bool TryParse(string rawValue, IFormatProvider provider, out object value)
        {
            if (TryParse(rawValue, provider, out var concreteValue))
            {
                value = concreteValue;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
    }
}