using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick.vNext.FeatureDocuments;
using Xunit.Gherkin.Quick.vNext.TestScenarios;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick.vNext.Hooks
{
    // TODO:
    // * add tests to cover parser + gherkin document mapping to test scenario
    // * add regex file pattern matcher
    // * add scenario outline support
    // * test test test, add feature tests where necessary
    // * cleanup/remove old code, remove global:: references where possible
    // * expose other relevant types + xml doc comment them
    // * send to code review explaining the new approach, limitations of the previous one
    //   and how matching + parsing files during discovery stage (compared to parsing &
    //   limitation from file repository during execution stage) allows for improved reporting
    internal class TestCaseDiscoverer : IXunitTestCaseDiscoverer
    {
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
                                $"Invalid :: '{matchingFeatureDocument.Name}'",
                                $"The '{matchingFeatureDocument.Name}' feature file is invalid, {matchingFeatureDocument.Error.Message}."
                            ),
                            1
                        );
                    else
                        return _GetTestCases(discoveryOptions, matchingFeatureDocument.Feature, testMethod)
                            .DefaultIfEmpty(
                                new UnavailableTestCase(
                                    _messageSink,
                                    discoveryOptions.MethodDisplayOrDefault(),
                                    testMethod,
                                    $"No Scenarios :: '{matchingFeatureDocument.Name}'",
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

        private IEnumerable<IXunitTestCase> _GetTestCases(ITestFrameworkDiscoveryOptions discoveryOptions, global::Gherkin.Ast.Feature feature, ITestMethod testMethod)
        {
            global::Gherkin.Ast.Background scenarioBackground = null;
            foreach (var scenarioDefinition in feature.Children)
                if (scenarioDefinition is global::Gherkin.Ast.Background background)
                    scenarioBackground = background;
                else if (scenarioDefinition is global::Gherkin.Ast.Scenario scenario)
                {
                    var testScenario = TestScenario.From(feature, scenarioBackground is object ? scenario.ApplyBackground(scenarioBackground) : scenario);
                    var displayName = $"{testScenario.FeatureName} :: {testScenario.ScenarioName}";
                    if (_IgnoreTags.Any(ignoreTag => testScenario.Tags.Contains(ignoreTag, StringComparer.OrdinalIgnoreCase)))
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
                else if (scenarioDefinition is global::Gherkin.Ast.ScenarioOutline scenarioOutline)
                {
                    var displayName = $"{feature.Name} :: {scenarioOutline.Name} :: Not yet implemented";
                    yield return new UnavailableTestCase(
                            _messageSink,
                            discoveryOptions.MethodDisplayOrDefault(),
                            testMethod,
                            displayName,
                            "Scenario outlines are not yet implemented"
                        );
                }
        }
    }
}