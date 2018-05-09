using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioXUnitTestCase : XunitTestCase
    {
        [Obsolete]
        public ScenarioXUnitTestCase()
        {
        }

        public ScenarioXUnitTestCase(IMessageSink messageSink, ITestMethod testMethod, string featureName, string scenarioName, object[] testMethodArguments = null)
            : base(messageSink, TestMethodDisplay.Method, testMethod, testMethodArguments)
        {
            DisplayName = $"{featureName} :: {scenarioName}";

            // These traits allow support for the excelent picklesdoc results visualizer
            Traits = new Dictionary<string, List<string>>
            {
                {  "FeatureTitle", new List<string> { featureName } },
                {  "Description", new List<string> { scenarioName } }
            };
        }

        public override async Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink, 
            IMessageBus messageBus, 
            object[] constructorArguments, 
            ExceptionAggregator aggregator, 
            CancellationTokenSource cancellationTokenSource)
        {
            return await new ScenarioXunitTestCaseRunner(
                this, 
                DisplayName, 
                SkipReason, 
                constructorArguments, 
                TestMethodArguments, 
                messageBus, 
                aggregator, 
                cancellationTokenSource).RunAsync();
        }
    }
}
