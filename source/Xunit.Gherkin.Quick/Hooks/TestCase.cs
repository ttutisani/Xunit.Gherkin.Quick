using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick.TestScenarios;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick.Hooks
{
    internal class TestCase : XunitTestCase
    {
        private string _displayName;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestCase()
        {
        }

        public TestCase(
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay testMethodDisplay,
            ITestMethod testMethod,
            string displayName,
            TestScenario testScenario
        )
            : base(diagnosticMessageSink, testMethodDisplay, testMethod, new[] { testScenario })
        {
            _displayName = displayName;
        }

        public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            => new XunitGherkinQuickTestCaseRunner(
                this,
                DisplayName,
                SkipReason,
                constructorArguments,
                TestMethodArguments,
                messageBus,
                aggregator,
                cancellationTokenSource
            ).RunAsync();

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

        protected override void Initialize()
        {
            base.Initialize();

            var testScenario = TestMethodArguments.OfType<TestScenario>().Single();

            // These traits allow support for the picklesdoc results visualizer (http://www.picklesdoc.com/)
            Traits["FeatureTitle"] = new List<string> { testScenario.FeatureName };
            Traits["Description"] = new List<string> { testScenario.ScenarioName };

            Traits["Category"] = new List<string>(testScenario.Tags);
        }

        private class XunitGherkinQuickTestCaseRunner : XunitTestCaseRunner
        {
            public XunitGherkinQuickTestCaseRunner(IXunitTestCase testCase, string displayName, string skipReason, object[] constructorArguments, object[] testMethodArguments, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
                : base(testCase, displayName, skipReason, constructorArguments, testMethodArguments, messageBus, aggregator, cancellationTokenSource)
            {
            }

            protected override XunitTestRunner CreateTestRunner(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, string skipReason, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            {
                var testOutputHelper = new TestOutputHelper();
                testOutputHelper.Initialize(messageBus, test);

                var updatedTestMethodArguments = new object[(testMethodArguments?.Length ?? 0) + 1];
                Array.Copy(
                    sourceArray: testMethodArguments,
                    sourceIndex: 0,
                    destinationArray: updatedTestMethodArguments,
                    destinationIndex: 1,
                    length: testMethodArguments.Length
                );
                updatedTestMethodArguments[0] = testOutputHelper;

                return base.CreateTestRunner(
                    test,
                    messageBus,
                    testClass,
                    constructorArguments,
                    testMethod,
                    updatedTestMethodArguments,
                    skipReason,
                    beforeAfterAttributes,
                    aggregator,
                    cancellationTokenSource
                );
            }
        }
    }
}