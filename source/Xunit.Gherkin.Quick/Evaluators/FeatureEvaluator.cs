using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gherkin.Ast;
using Xunit.Gherkin.Quick.TestScenarios;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick.Evaluators
{
    internal class FeatureEvaluator
    {
        private readonly IReadOnlyCollection<TestStepType> _concreteStepTypes = new[] { TestStepType.Given, TestStepType.When, TestStepType.Then, TestStepType.And, TestStepType.But };
        private static readonly IReadOnlyDictionary<string, string> _Expressions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "int", @"([+-]?\d+)" },
            { "float", @"([+-]?([0-9]*[.])?[0-9]+)" },
            { "word", @"(\w+)" },
            { "string", @"(?:""|')([^\1]*)(?:""|')" },
            { "", @"(.*)" }
        };
        private readonly Feature _feature;
        private readonly IReadOnlyDictionary<TestStepType, IReadOnlyCollection<StepDefinition>> _stepDefinitionsByType;

        internal FeatureEvaluator(Feature feature)
        {
            if (feature is null)
                throw new ArgumentNullException(nameof(feature));

            _feature = feature;
            _stepDefinitionsByType = _GetStepDefinitions(feature.GetType())
               .GroupBy(stepInfo => stepInfo.Type)
               .ToDictionary(group => group.Key, group => group.ToList() as IReadOnlyCollection<StepDefinition>);
        }

        public Task EvaluateStepAsync(TestStep testStep)
        {
            if (testStep is null)
                throw new ArgumentNullException(nameof(testStep));

            var matchedStepDefinitions = _GetMatchedStepDefinitions(testStep).ToList();
            switch (matchedStepDefinitions.Count)
            {
                case 0:
                    throw new XunitException($"No methods matching '{testStep.Text}' were found.");

                case 1:
                    var matchedStepDefinition = matchedStepDefinitions.Single();
                    var result = matchedStepDefinition.Method.IsStatic
                        ? matchedStepDefinition.Method.Invoke(null, matchedStepDefinition.Arguments)
                        : matchedStepDefinition.Method.Invoke(_feature, matchedStepDefinition.Arguments);

                    if (matchedStepDefinition.Method.ReturnType == typeof(void))
                        if (matchedStepDefinition.Method.IsDefined(typeof(AsyncStateMachineAttribute)))
                            throw new XunitException($"Method '{matchedStepDefinition.Method.Name}' of '{matchedStepDefinition.Method.DeclaringType.Name}' class is async and void, which looks like a mistake. Use either async with Task or void without async.");
                        else
                            return Task.CompletedTask;

                    else if (result is Task taskResult)
                        return taskResult;

                    else
                        throw new XunitException($"Method return type '{matchedStepDefinition.Method.ReturnType.Name}' is not supported.");

                default:
                    var matchedMethodNames = matchedStepDefinitions.Select(stepDefinition => stepDefinition.Method.Name);
                    throw new XunitException($"Multiple methods match step '{testStep.Text}', {string.Join(", ", matchedMethodNames)}");
            }
        }

        private IEnumerable<MatchedStepDefinition> _GetMatchedStepDefinitions(TestStep testStep)
        {
            foreach (var concreteStepType in _concreteStepTypes)
                if ((testStep.Type & concreteStepType) == concreteStepType && _stepDefinitionsByType.TryGetValue(concreteStepType, out var stepDefinitions))
                    foreach (var stepDefinition in stepDefinitions)
                    {
                        var match = stepDefinition.Pattern.Match(testStep.Text);
                        if (match.Success)
                        {
                            var methodParameters = stepDefinition.Method.GetParameters();
                            var methodArguments = new object[methodParameters.Length];
                            if (methodArguments.Length > 0)
                            {
                                var methodArgumentIndex = 0;
                                var rootGroupCaptures = _GetRootGroupCaptures(match);
                                for (var methodParameterIndex = 0; methodParameterIndex < methodParameters.Length; methodParameterIndex++)
                                {
                                    var methodParameter = methodParameters[methodParameterIndex];
                                    if (_TryGetStepBodyArgument(testStep, methodParameter.ParameterType, out var stepBodyArgument))
                                        methodArguments[methodArgumentIndex] = stepBodyArgument;
                                    else if (rootGroupCaptures.Count < methodArgumentIndex)
                                        throw new XunitException($"Cannot extract value for parameter `{methodParameter.Name}` at index {methodParameterIndex}; only {rootGroupCaptures.Count} parameters were provided. Method `{methodParameter.Member.Name}`.");
                                    else
                                        methodArguments[methodArgumentIndex] = Convert.ChangeType(rootGroupCaptures[methodArgumentIndex], methodParameter.ParameterType, CultureInfo.InvariantCulture);
                                    methodArgumentIndex++;
                                }
                            }

                            yield return new MatchedStepDefinition(stepDefinition, methodArguments);
                        }
                    }
        }

        private static IReadOnlyList<string> _GetRootGroupCaptures(Match match)
        {
            var rootGroupCaptures = new List<string>(match.Groups.Count);

            var startIndex = 0;
            foreach (var group in match.Groups.Cast<Group>().Skip(1))
                if (group.Success && startIndex <= group.Index)
                {
                    rootGroupCaptures.Add(group.Value);
                    startIndex = group.Index + group.Length;
                }

            return rootGroupCaptures;
        }

        private static bool _TryGetStepBodyArgument(TestStep testStep, Type parameterType, out object stepBodyArgument)
        {
            if (parameterType == typeof(TestStepDocStringArgument))
            {
                stepBodyArgument = testStep.DocStringArgument ?? throw new XunitException("Expected doc string argument");
                return true;
            }

            else if (parameterType == typeof(DocString))
                if (testStep.DocStringArgument is null)
                    throw new XunitException("Expected doc string argument");
                else
                {
                    stepBodyArgument = new DocString(
                            new Location(testStep.DocStringArgument.Location.Line, testStep.DocStringArgument.Location.Column),
                            testStep.DocStringArgument.ContentType,
                            testStep.DocStringArgument.Content
                        );
                    return true;
                }

            else if (parameterType == typeof(TestStepTableArgument))
            {
                stepBodyArgument = testStep.TableArgument ?? throw new XunitException("Expected table argument");
                return true;
            }

            else if (parameterType == typeof(DataTable))
                if (testStep.TableArgument is null)
                    throw new XunitException("Expected table argument");
                else
                {
                    stepBodyArgument = new DataTable(
                        testStep
                            .TableArgument
                            .Rows
                            .Select(
                                row => new TableRow(
                                    new Location(row.Location.Line, row.Location.Column),
                                    row
                                        .Cells
                                        .Select(cell => new TableCell(new Location(cell.Location.Line, cell.Location.Column), cell.Value))
                                        .ToArray()
                                )
                            )
                            .ToArray()
                    );
                    return true;
                }

            else
            {
                stepBodyArgument = null;
                return false;
            }
        }

        private static IEnumerable<StepDefinition> _GetStepDefinitions(Type type)
        {
            var expressionReplacePattern = new Regex($@"\{{(?<expressionKey>{string.Join("|", _Expressions.Keys.Select(Regex.Escape))})\}}", RegexOptions.IgnoreCase);
            Regex _GetPattern(string stepPattern)
            {
                var expressionPattern = expressionReplacePattern.Replace(
                    stepPattern,
                    match =>
                    {
                        if (!_Expressions.TryGetValue(match.Groups["expressionKey"].Value, out var matchingPattern))
                            throw new XunitException($"Unknown Gherkin expression: '{match.Value}'");

                        return matchingPattern;
                    }
                );

                return new Regex($"^{expressionPattern}$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }

            return type
                .GetRuntimeMethods()
                .Where(method => (method.IsPublic || method.IsAssembly) && !method.IsAbstract && !method.IsGenericMethodDefinition)
                .SelectMany(method => method
                    .GetCustomAttributes<BaseStepDefinitionAttribute>(inherit: true)
                    .Select(
                        stepAttribute => new StepDefinition(
                            _GetTestStepType(stepAttribute.Keyword),
                            _GetPattern(stepAttribute.Pattern),
                            method
                        )
                    )
                );
        }

        private static TestStepType _GetTestStepType(string keyword)
        {
            if (keyword.Equals("given", StringComparison.OrdinalIgnoreCase))
                return TestStepType.Given;
            else if (keyword.Equals("when", StringComparison.OrdinalIgnoreCase))
                return TestStepType.When;
            else if (keyword.Equals("then", StringComparison.OrdinalIgnoreCase))
                return TestStepType.Then;
            else if (keyword.Equals("and", StringComparison.OrdinalIgnoreCase))
                return TestStepType.And;
            else if (keyword.Equals("but", StringComparison.OrdinalIgnoreCase))
                return TestStepType.But;
            else
                return TestStepType.Unknown;
        }
    }
}
