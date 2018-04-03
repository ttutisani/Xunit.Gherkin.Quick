using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{

    public sealed class ScenarioXUnitTestCase : XunitTestCase
    {
        [Obsolete]
        public ScenarioXUnitTestCase()
        {
        }

        public ScenarioXUnitTestCase(IMessageSink messageSink, ITestMethod testMethod, string scenarioName, object[] testMethodArguments = null)
            : base(messageSink, TestMethodDisplay.Method, testMethod, testMethodArguments)
        {
            DisplayName = scenarioName;
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
