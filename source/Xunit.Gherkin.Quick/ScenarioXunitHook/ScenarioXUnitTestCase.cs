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

        private bool _skip;
        private string _displayName;

        public ScenarioXunitTestCase(
            IMessageSink messageSink,
            ITestMethod testMethod,
            string featureName,
            string scenarioName,
            IEnumerable<string> tags,
            bool skip = false,
            object[] testMethodArguments = null)
            
            : base(
                  messageSink, 
                  TestMethodDisplay.Method, 
                  testMethod, 
                  testMethodArguments)
        {
            DisplayName = _displayName = $"{featureName} :: {scenarioName}";

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

            _skip = skip;
        }

        protected override string GetSkipReason(IAttributeInfo factAttribute)
        {
            if (_skip)
                return $"Scenario `{_displayName}` is ignored.";

            return null;
        }

        public override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);

            data.AddValue(nameof(_skip), _skip);
            data.AddValue(nameof(_displayName), _displayName);
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);

            _skip = data.GetValue<bool>(nameof(_skip));
            _displayName = data.GetValue<string>(nameof(_displayName));
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
