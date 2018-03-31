using Gherkin.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick
{
    public abstract class Feature
    {
        /// <summary>Allows you to log extra data to the result of the test.</summary>
        protected ITestOutputHelper Output { get; }

        /// <summary>Create a new Feature.</summary>
        protected Feature() {}

        /// <summary>Create a new Feature.</summary>
        /// <param name="output">
        /// Allows you to log extra data to the result of the test. Xunit will provide you with an instance
        /// in your constructor which you can pass straight through e.g. public MyFeatureClass(ITestOutputHelper output) : base(output)
        /// </param>
        protected Feature(ITestOutputHelper output) => Output = output;

        [Scenario]
        internal void Scenario(string scenarioName)
        {
            var gherkinDocument = ScenarioDiscoverer.GetGherkinDocumentByType(GetType());

            var parsedScenario = gherkinDocument.Feature.Children.FirstOrDefault(scenario => scenario.Name == scenarioName);
            if (parsedScenario == null)
                throw new GherkinException($"Cannot find scenario `{scenarioName}`.");

            var stepMethods = GetType().GetTypeInfo().GetMethods()
                .Where(m => m.IsDefined(typeof(StepDefinitionAttributeBase)))
                .Select(m => new { method = m, keywordAttribute = m.GetCustomAttribute<StepDefinitionAttributeBase>() });

            foreach (var parsedStep in parsedScenario.Steps)
            {
                var matchingStepMethod = stepMethods.FirstOrDefault(stepMethod => 
                    stepMethod.keywordAttribute.MatchesStep(parsedStep.Keyword, parsedStep.Text));
                if (matchingStepMethod == null)
                    throw new GherkinException($"Cannot find scenario step `{parsedStep.Keyword}{parsedStep.Text}` for scenario `{scenarioName}`.");
                
                var stepRegexMatch = matchingStepMethod.keywordAttribute.MatchRegex(parsedStep.Text);
                if (!string.Equals(stepRegexMatch.Value, parsedStep.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    throw new GherkinException($"Step method partially matched but not selected. Step `{parsedStep.Text.Trim()}`, Method pattern `{matchingStepMethod.keywordAttribute.Pattern}`.");

                var methodParams = matchingStepMethod.method.GetParameters();

                var methodParamsCount = (methodParams.LastOrDefault()?.ParameterType == typeof(DataTable))
                    ? methodParams.Length - 1 // Skip the final param, which is to be filled with table data
                    : methodParams.Length;    //No table data requested

                var methodParamValues = new List<object>();
                if (methodParamsCount > 0)
                {
                    var methodParamStringValues = stepRegexMatch.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToList();

                    if (methodParamStringValues.Count < methodParamsCount)
                        throw new GherkinException($"Method `{matchingStepMethod.method.Name}` for step `{parsedStep.Keyword}{parsedStep.Text}` is expecting {methodParams.Length} params, but only {methodParamStringValues.Count} param values were supplied.");

                    methodParamValues = methodParams.Select((p, i) => Convert.ChangeType(methodParamStringValues[i], p.ParameterType))
                        .ToList();
                }

                if (methodParams.LastOrDefault()?.ParameterType == typeof(DataTable))
                {
                    if (!(parsedStep.Argument is DataTable))
                        throw new GherkinException($"Method `{matchingStepMethod.method.Name}` for step `{parsedStep.Keyword}{parsedStep.Text}` is expecting a table parameter, but none was supplied.");

                    methodParamValues.Add(parsedStep.Argument as DataTable);
                }

                try
                {
                    matchingStepMethod.method.Invoke(this, methodParamValues.Any() ? methodParamValues.ToArray() : null);
                    Output?.WriteLine($"{parsedStep.Keyword} {parsedStep.Text}: PASSED");
                }
                catch
                {
                    Output?.WriteLine($"{parsedStep.Keyword} {parsedStep.Text}: FAILED");
                    throw;
                }
            }
        }
    }
}
