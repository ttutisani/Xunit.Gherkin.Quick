using Gherkin.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xunit.Gherkin.Quick
{
    internal static class ParameterHelper
    {
        public static List<object> GetParamValues(MethodInfo method, Step step, List<string> specificationParameters)
        {
            var methodParams = method.GetParameters();
            var dataTable = step.Argument as DataTable;

            var expectedParamCount = (methodParams.Count(p => p.ParameterType != typeof(DataTable)));

            var methodParamValues = new List<object>();

            if (expectedParamCount > 0 && specificationParameters.Count < expectedParamCount)
                throw new Exception($"Method `{method.Name}` for step `{step.Keyword}{step.Text}` is expecting {expectedParamCount} params, but only {specificationParameters.Count} param values were supplied.");
            
            short specificationParameterIndex = 0;

            foreach (var param in methodParams)
            {
                if (param.ParameterType == typeof(DataTable))
                {
                    if (dataTable == null)
                        throw new Exception($"Method `{method.Name}` for step `{step.Keyword}{step.Text}` is expecting a table parameter, but none was supplied.");

                    methodParamValues.Add(dataTable);
                }
                else
                {
                    methodParamValues.Add(Convert.ChangeType(specificationParameters[specificationParameterIndex], param.ParameterType));
                    specificationParameterIndex++;
                }
            }

            return methodParamValues;
        }
    }
}