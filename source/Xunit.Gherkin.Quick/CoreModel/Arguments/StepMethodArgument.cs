using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xunit.Gherkin.Quick
{
    internal abstract class StepMethodArgument
    {
        public static List<StepMethodArgument> ListFromParameters(ParameterInfo[] parameters)
        {
            return parameters.Select(p => FromParameter(p))
                .ToList();
        }

        private static StepMethodArgument FromParameter(ParameterInfo parameter)
        {
            return new PrimitiveTypeArgument();
        }

        public object Value { get; }
    }
}
