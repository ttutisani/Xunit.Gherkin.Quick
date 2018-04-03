using Gherkin.Ast;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick
{
    public abstract class Feature
    {
        /// <summary>Allows you to log extra data to the result of the test.</summary>
        protected internal ITestOutputHelper Output { get; internal set; }

        [Scenario]
        internal void Scenario(string scenarioName)
        {
            var gherkinDocument = ScenarioDiscoverer.GetGherkinDocumentByType(GetType());

            var parsedScenario = gherkinDocument.Feature.Children.FirstOrDefault(scenario => scenario.Name == scenarioName);
            if (parsedScenario == null)
                throw new InvalidOperationException($"Cannot find scenario `{scenarioName}`.");

            var stepMethods = GetType().GetTypeInfo().GetMethods()
                .Where(m => m.IsDefined(typeof(BaseStepDefinitionAttribute)))
                .Select(m => new { method = m, keywordAttribute = m.GetCustomAttribute<BaseStepDefinitionAttribute>() });

            foreach (var parsedStep in parsedScenario.Steps)
            {
                var matchingStepMethod = stepMethods.FirstOrDefault(stepMethod => 
                    stepMethod.keywordAttribute.MatchesStep(parsedStep.Keyword, parsedStep.Text));
                if (matchingStepMethod == null)
                    throw new InvalidOperationException($"Cannot find scenario step `{parsedStep.Keyword}{parsedStep.Text}` for scenario `{scenarioName}`.");

                var stepRegexMatch = matchingStepMethod.keywordAttribute.MatchRegex(parsedStep.Text);
                if (!string.Equals(stepRegexMatch.Value, parsedStep.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"Step method partially matched but not selected. Step `{parsedStep.Text.Trim()}`, Method pattern `{matchingStepMethod.keywordAttribute.Pattern}`.");

                var methodParams = matchingStepMethod.method.GetParameters();
                object[] methodParamValues = null;
                if (methodParams.Length > 0)
                {
                    var methodParamStringValues = stepRegexMatch.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToList();

                    if (methodParamStringValues.Count < methodParams.Length)
                        throw new InvalidOperationException($"Method `{matchingStepMethod.method.Name}` for step `{parsedStep.Keyword}{parsedStep.Text}` is expecting {methodParams.Length} params, but only {methodParamStringValues.Count} param values were supplied.");

                    methodParamValues = methodParams.Select((p, i) => Convert.ChangeType(methodParamStringValues[i], p.ParameterType))
                        .ToArray();
                }

                try
                {
                    matchingStepMethod.method.Invoke(this, methodParamValues);
                    Output.WriteLine($"{parsedStep.Keyword} {parsedStep.Text}: PASSED");
                }
                catch
                {
                    Output.WriteLine($"{parsedStep.Keyword} {parsedStep.Text}: FAILED");
                    throw;
                }
            }
        }
    }
}
