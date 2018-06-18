using Gherkin.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioXunitTestCase : XunitTestCase
    {
        [Obsolete]
        public ScenarioXunitTestCase()
        {
        }

        public ScenarioXunitTestCase(IMessageSink messageSink, ITestMethod testMethod, string featureName, string scenarioName, object[] testMethodArguments = null, IEnumerable<string> tags = null)
            : base(messageSink, TestMethodDisplay.Method, testMethod, testMethodArguments)
        {
            DisplayName = $"{featureName} :: {scenarioName}";

            // These traits allow support for the picklesdoc results visualizer (http://www.picklesdoc.com/)
            Traits = new Dictionary<string, List<string>>
            {
                {  "FeatureTitle", new List<string> { featureName } },
                {  "Description", new List<string> { scenarioName } }
            };

            if (tags != null && tags.Any())
            {
                Traits["Category"] = tags.ToList();
            }
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
