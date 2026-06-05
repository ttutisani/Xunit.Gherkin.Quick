using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gherkin.Ast;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick.vNext
{
    internal enum TestStepSection
    {
        Prepare,
        Action,
        Effect
    }

    internal class TestStep : IXunitSerializable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestStep()
        {
        }

        public TestStep(TestStepSection section, string text)
        {
            Section = section;
            Text = text;
        }

        public TestStepSection Section { get; private set; }

        public string Text { get; private set; }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Section), Section, typeof(TestStepSection));
            info.AddValue(nameof(Text), Text, typeof(string));
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            Section = info.GetValue<TestStepSection>(nameof(Section));
            Text = info.GetValue<string>(nameof(Text));
        }
    }

    internal class TestScenario : IXunitSerializable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestScenario()
        {
        }

        public TestScenario(string featureName, string scenarioName, IReadOnlyList<string> tags, IReadOnlyList<TestStep> steps)
        {
            FeatureName = featureName;
            ScenarioName = scenarioName;
            Tags = tags ?? Array.Empty<string>();
            Steps = steps ?? Array.Empty<TestStep>();
        }

        public string FeatureName { get; private set; }
        public string ScenarioName { get; private set; }
        public IReadOnlyCollection<string> Tags { get; private set; }
        public IReadOnlyList<TestStep> Steps { get; private set; }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(FeatureName), FeatureName, typeof(string));
            info.AddValue(nameof(ScenarioName), ScenarioName, typeof(string));
            info.AddValue(nameof(Tags), Tags.ToArray(), typeof(string[]));
            info.AddValue(nameof(Steps), Steps.ToArray(), typeof(TestStep[]));
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            FeatureName = info.GetValue<string>(nameof(FeatureName));
            ScenarioName = info.GetValue<string>(nameof(ScenarioName));
            Tags = info.GetValue<string[]>(nameof(Tags)) ?? Array.Empty<string>();
            Steps = info.GetValue<TestStep[]>(nameof(Steps)) ?? Array.Empty<TestStep>();
        }
    }

    internal class XunitGherkinQuickTestCaseDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IReadOnlyCollection<string> _IgnoreTags = new List<string> { "ignore" };
        private readonly IMessageSink _messageSink;
        private readonly FeatureDocumentMatcher _featureDocumentMatcher = new FeatureDocumentMatcher();

        public XunitGherkinQuickTestCaseDiscoverer(IMessageSink messageSink)
            => _messageSink = messageSink;

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
            => _featureDocumentMatcher
                .GetMatchingDocuments(testMethod.TestClass.Class.ToRuntimeType())
                .SelectMany(matchingFeatureDocument =>
                {
                    if (matchingFeatureDocument.Error is object)
                        return Enumerable.Repeat(
                            new XunitGherkinQuickUnavailableTestCase(
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
                                new XunitGherkinQuickUnavailableTestCase(
                                    _messageSink,
                                    discoveryOptions.MethodDisplayOrDefault(),
                                    testMethod,
                                    $"No Scenarios :: '{matchingFeatureDocument.Name}'",
                                    $"Feature file '{matchingFeatureDocument.Name}' does not contain any scenarios."
                                )
                            );
                })
                .DefaultIfEmpty(
                    new XunitGherkinQuickUnavailableTestCase(
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
            {
                if (scenarioDefinition is global::Gherkin.Ast.Background background)
                    scenarioBackground = background;
                else if (scenarioDefinition is global::Gherkin.Ast.Scenario scenario)
                {
                    var testScenario = _GetTestScenario(feature, scenarioBackground is object ? scenario.ApplyBackground(scenarioBackground) : scenario);
                    var displayName = $"{testScenario.FeatureName} :: {testScenario.ScenarioName}";
                    if (_IgnoreTags.Any(ignoreTag => testScenario.Tags.Contains(ignoreTag, StringComparer.OrdinalIgnoreCase)))
                        yield return new XunitGherkinQuickUnavailableTestCase(
                            _messageSink,
                            discoveryOptions.MethodDisplayOrDefault(),
                            testMethod,
                            displayName,
                            "This scenario is skipped"
                        );
                    else
                        yield return new XunitGherkinQuickTestCase(
                            _messageSink,
                            discoveryOptions.MethodDisplayOrDefault(),
                            testMethod,
                            displayName,
                            testScenario
                        );
                }

                // var scenarioOutline = scenarioDefinition as global::Gherkin.Ast.ScenarioOutline;
                // if (scenarioOutline is object)
                //     yield return new XunitGherkinQuickTestCase(
                //         _messageSink,
                //         TestMethodDisplay.Method,
                //         testMethod,
                //         feature,
                //         scenario,
                //         feature.Tags.Concat(scenario.Tags).Any(tag => _IgnoreTag.Equals(tag.Name, StringComparison.OrdinalIgnoreCase))
                //             ? "This scenario is skipped"
                //             : null
                //     );
            }
        }

        private TestScenario _GetTestScenario(global::Gherkin.Ast.Feature feature, global::Gherkin.Ast.Scenario scenario)
        {
            var tags = (feature.Tags ?? Enumerable.Empty<Tag>())
                .Concat(scenario.Tags ?? Enumerable.Empty<Tag>())
                .Where(tag => !string.IsNullOrWhiteSpace(tag.Name))
                .Select(tag => tag.Name.StartsWith("@") ? tag.Name.Substring(1) : tag.Name)
                .ToList();

            var testSteps = new List<TestStep>(scenario.Steps.Count());
            var testStepSection = TestStepSection.Prepare;
            foreach (var step in scenario.Steps)
            {
                if (step.Keyword.IndexOf("given", StringComparison.OrdinalIgnoreCase) >= 0)
                    testStepSection = TestStepSection.Prepare;
                else if (step.Keyword.IndexOf("when", StringComparison.OrdinalIgnoreCase) >= 0)
                    testStepSection = TestStepSection.Action;
                else if (step.Keyword.IndexOf("then", StringComparison.OrdinalIgnoreCase) >= 0)
                    testStepSection = TestStepSection.Effect;

                testSteps.Add(new TestStep(testStepSection, step.Text));
            }

            return new TestScenario(
                feature.Name,
                scenario.Name,
                tags,
                testSteps
            );
        }
    }

    internal class XunitGherkinQuickUnavailableTestCase : XunitTestCase
    {
        private string _skipReason;
        private string _displayName;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public XunitGherkinQuickUnavailableTestCase()
        {
        }

        public XunitGherkinQuickUnavailableTestCase(
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay testMethodDisplay,
            ITestMethod testMethod,
            string displayName,
            string skipReason,
            object[] testMethodArguments = null
        )
            : base(diagnosticMessageSink, testMethodDisplay, testMethod, testMethodArguments)
        {
            if (displayName is null)
                throw new ArgumentNullException(nameof(displayName));

            _displayName = testMethodArguments != null && testMethodArguments.Length > 0 ? $"{displayName}({string.Join(", ", testMethodArguments)})" : displayName;
            _skipReason = skipReason;
        }

        public override void Serialize(IXunitSerializationInfo data)
        {
            data.AddValue(nameof(_displayName), _displayName, typeof(string));
            data.AddValue(nameof(_skipReason), _skipReason, typeof(string));
            base.Serialize(data);
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);
            _displayName = data.GetValue<string>(nameof(_displayName));
            _skipReason = data.GetValue<string>(nameof(_skipReason));
        }

        public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            => Task.FromResult(new RunSummary { Total = 1, Skipped = 1, Failed = 0, Time = 0 });

        protected override string GetDisplayName(IAttributeInfo factAttribute, string displayName)
        {
            switch (DefaultMethodDisplay)
            {
                case TestMethodDisplay.Method:
                    return _displayName;

                default:
                case TestMethodDisplay.ClassAndMethod:
                    return $"{TestMethod.TestClass.Class.Name}.{_displayName}";
            }
        }

        protected override string GetSkipReason(IAttributeInfo factAttribute)
            => _skipReason ?? base.GetSkipReason(factAttribute);
    }

    internal class XunitGherkinQuickTestCase : XunitTestCase
    {
        private string _displayName;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public XunitGherkinQuickTestCase()
        {
        }

        public XunitGherkinQuickTestCase(
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay testMethodDisplay,
            ITestMethod testMethod,
            string displayName,
            TestScenario testScenario
        )
            : base(diagnosticMessageSink, testMethodDisplay, testMethod, new[] { testScenario })
        {
            _displayName = displayName;

            // These traits allow support for the picklesdoc results visualizer (http://www.picklesdoc.com/)
            Traits = Traits ?? new Dictionary<string, List<string>>();
            Traits["FeatureTitle"] = new List<string> { testScenario.FeatureName };
            Traits["Description"] = new List<string> { testScenario.ScenarioName };

            Traits["Category"] = new List<string>(testScenario.Tags);
        }

        public override void Serialize(IXunitSerializationInfo data)
        {
            data.AddValue(nameof(_displayName), _displayName, typeof(string));
            base.Serialize(data);
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);
            _displayName = data.GetValue<string>(nameof(_displayName));
        }

        protected override string GetDisplayName(IAttributeInfo factAttribute, string displayName)
        {
            switch (DefaultMethodDisplay)
            {
                case TestMethodDisplay.Method:
                    return _displayName;

                default:
                case TestMethodDisplay.ClassAndMethod:
                    return $"{TestMethod.TestClass.Class.Name}.{_displayName}";
            }
        }
    }
}