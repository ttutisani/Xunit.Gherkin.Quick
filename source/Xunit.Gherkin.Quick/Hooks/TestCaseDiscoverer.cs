using System;
using System.Collections.Generic;
using System.Linq;
using Gherkin;
using Gherkin.Ast;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick.FeatureDocuments;
using Xunit.Gherkin.Quick.TestScenarios;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick.Hooks
{
    internal class TestCaseDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly TestScenarioMapper _testScenarioMapper = new TestScenarioMapper(new GherkinDialectProvider());
        private readonly IReadOnlyCollection<string> _IgnoreTags = new List<string> { "ignore" };
        private readonly IMessageSink _messageSink;
        private readonly FeatureDocumentMatcher _featureDocumentMatcher = new FeatureDocumentMatcher();

        public TestCaseDiscoverer(IMessageSink messageSink)
            => _messageSink = messageSink;

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
            => _featureDocumentMatcher
                .GetMatchingDocuments(testMethod.TestClass.Class.ToRuntimeType())
                .SelectMany(matchingFeatureDocument =>
                {
                    if (matchingFeatureDocument.Error is object)
                        return Enumerable.Repeat(
                            new UnavailableTestCase(
                                _messageSink,
                                discoveryOptions.MethodDisplayOrDefault(),
                                testMethod,
                                $"'{matchingFeatureDocument.Name}' :: Invalid Feature File",
                                $"The '{matchingFeatureDocument.Name}' feature file is invalid, {matchingFeatureDocument.Error.Message}."
                            ),
                            1
                        );
                    else
                        return _GetTestCases(discoveryOptions, matchingFeatureDocument.Content, testMethod)
                            .DefaultIfEmpty(
                                new UnavailableTestCase(
                                    _messageSink,
                                    discoveryOptions.MethodDisplayOrDefault(),
                                    testMethod,
                                    $"'{matchingFeatureDocument.Name}' :: No Scenarios Defined",
                                    $"Feature file '{matchingFeatureDocument.Name}' does not contain any scenarios."
                                )
                            );
                })
                .DefaultIfEmpty(
                    new UnavailableTestCase(
                        _messageSink,
                        discoveryOptions.MethodDisplayOrDefault(),
                        testMethod,
                        "No Matching Feature Files",
                        $"There are no matching feature files for '{testMethod.TestClass.Class.Name}.{testMethod.Method.Name}'."
                    )
                );

        private IEnumerable<IXunitTestCase> _GetTestCases(ITestFrameworkDiscoveryOptions discoveryOptions, global::Gherkin.Ast.GherkinDocument document, ITestMethod testMethod)
        {
            global::Gherkin.Ast.Background scenarioBackground = null;
            foreach (var scenarioDefinition in document.Feature.Children)
                if (scenarioDefinition is global::Gherkin.Ast.Background background)
                    scenarioBackground = background;
                else if (scenarioDefinition is global::Gherkin.Ast.Scenario scenario)
                    yield return _GetScenarioTestCase(discoveryOptions, testMethod, document, scenarioBackground, scenario);
                else if (scenarioDefinition is global::Gherkin.Ast.ScenarioOutline scenarioOutline)
                    foreach (var testCase in _GetScenarioOutlineTestCases(discoveryOptions, testMethod, document, scenarioBackground, scenarioOutline))
                        yield return testCase;
        }

        private IXunitTestCase _GetScenarioTestCase(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, global::Gherkin.Ast.GherkinDocument document, global::Gherkin.Ast.Background scenarioBackground, global::Gherkin.Ast.Scenario scenario)
        {
            var displayName = _GetDisplayName(document.Feature, scenario);
            var testScenario = _testScenarioMapper.Map(document, _ApplyBackground(scenario, scenarioBackground));

            if (_IsIgnored(testScenario))
                return new UnavailableTestCase(
                    _messageSink,
                    discoveryOptions.MethodDisplayOrDefault(),
                    testMethod,
                    displayName,
                    "This scenario is skipped"
                );
            else
                return new TestCase(
                    _messageSink,
                    discoveryOptions.MethodDisplayOrDefault(),
                    testMethod,
                    displayName,
                    testScenario
                );
        }

        private IEnumerable<IXunitTestCase> _GetScenarioOutlineTestCases(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, global::Gherkin.Ast.GherkinDocument document, global::Gherkin.Ast.Background scenarioBackground, global::Gherkin.Ast.ScenarioOutline scenarioOutline)
        {
            if (scenarioOutline.Examples is null || !scenarioOutline.Examples.Any())
                yield return new UnavailableTestCase(
                    _messageSink,
                    discoveryOptions.MethodDisplayOrDefault(),
                    testMethod,
                    $"{_GetDisplayName(document.Feature, scenarioOutline)} :: No Examples Defined",
                    $"Scenario outline '{scenarioOutline.Name}' does not contain any examples."
                );
            else
                foreach (var example in scenarioOutline.Examples)
                {
                    var displayName = _GetDisplayName(document.Feature, scenarioOutline, example);

                    if (example.TableHeader is null || example.TableBody is null || !example.TableBody.Any())
                        yield return new UnavailableTestCase(
                            _messageSink,
                            discoveryOptions.MethodDisplayOrDefault(),
                            testMethod,
                            $"{displayName} :: No Cases Defined",
                            $"Example '{example.Name}' for scenario outline '{scenarioOutline.Name}' does not contain any cases."
                        );
                    else if (
                            example
                                .TableHeader
                                .Cells
                                .GroupBy(headerCell => headerCell.Value, StringComparer.OrdinalIgnoreCase)
                                .Any(group => group.Count() > 1)
                        )
                        yield return new UnavailableTestCase(
                            _messageSink,
                            discoveryOptions.MethodDisplayOrDefault(),
                            testMethod,
                            $"{displayName} :: Duplicate Parameters",
                            $"Example '{example.Name}' for scenario outline '{scenarioOutline.Name}' contains multiple parameters with the same name (case-insensitive check)."
                        );
                    else
                        foreach (var testCase in _GetScenarioOutlineExampleTestCases(discoveryOptions, testMethod, document, scenarioBackground, scenarioOutline, example))
                            yield return testCase;
                }
        }

        private IEnumerable<IXunitTestCase> _GetScenarioOutlineExampleTestCases(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, global::Gherkin.Ast.GherkinDocument document, global::Gherkin.Ast.Background scenarioBackground, global::Gherkin.Ast.ScenarioOutline scenarioOutline, global::Gherkin.Ast.Examples example)
        {
            var generatedScenario = _ApplyBackground(
                new global::Gherkin.Ast.Scenario(
                    (scenarioOutline.Tags ?? Enumerable.Empty<global::Gherkin.Ast.Tag>())
                        .Concat(example.Tags ?? Enumerable.Empty<global::Gherkin.Ast.Tag>())
                        .ToArray(),
                    scenarioOutline.Location,
                    scenarioOutline.Keyword,
                    scenarioOutline.Name,
                    scenarioOutline.Description,
                    scenarioOutline.Steps as global::Gherkin.Ast.Step[] ?? scenarioOutline.Steps?.ToArray()
                ),
                scenarioBackground
            );

            foreach (var @case in example.TableBody)
            {
                var arguments = example
                    .TableHeader
                    .Cells
                    .Zip(@case.Cells, (headerCell, caseCell) => new { Name = headerCell.Value, Value = caseCell.Value })
                    .ToDictionary(argument => argument.Name, argument => argument.Value, StringComparer.OrdinalIgnoreCase);

                var argumentsDisplay = string.Join(
                    ", ",
                    example
                        .TableHeader
                        .Cells
                        .Zip(@case.Cells, (headerCell, caseCell) => $"{headerCell.Value} = {caseCell.Value}")
                );

                var displayName = $"{_GetDisplayName(document.Feature, scenarioOutline, example)} ({argumentsDisplay})";
                var testScenario = _testScenarioMapper.Map(document, generatedScenario, arguments);
                if (_IsIgnored(testScenario))
                    yield return new UnavailableTestCase(
                        _messageSink,
                        discoveryOptions.MethodDisplayOrDefault(),
                        testMethod,
                        displayName,
                        "This scenario is skipped"
                    );
                else
                    yield return new TestCase(
                        _messageSink,
                        discoveryOptions.MethodDisplayOrDefault(),
                        testMethod,
                        displayName,
                        testScenario
                    );
            }
        }

        private bool _IsIgnored(TestScenario testScenario)
            => _IgnoreTags.Any(ignoreTag => testScenario.Tags.Contains(ignoreTag, StringComparer.OrdinalIgnoreCase));

        private static string _GetDisplayName(params IHasDescription[] hasDescriptions)
            => string.Join(" :: ", hasDescriptions.Where(hasDescription => !string.IsNullOrWhiteSpace(hasDescription.Name)));

        private static global::Gherkin.Ast.Scenario _ApplyBackground(global::Gherkin.Ast.Scenario scenario, global::Gherkin.Ast.Background background)
            => background is null || !background.Steps.Any()
            ? scenario
            : new global::Gherkin.Ast.Scenario(
                scenario.Tags as global::Gherkin.Ast.Tag[] ?? scenario.Tags.ToArray(),
                scenario.Location,
                scenario.Keyword,
                scenario.Name,
                scenario.Description,
                background.Steps.Concat(scenario.Steps).ToArray()
            );
    }
}