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
                  TestMethodDisplayOptions.None,
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
            //This may seem an overhead, but this ensures that SkipReason is calculated correctly.
            //Reason: base class in Xunit library, strangely, initializes during deserialization
            //routine because EnsureInitialize() call is made inside every property setter.
            //that results in calling Initialize() too ealy, even before Deserialize() is called.
            //Deserialize is not able to set the _skip flag to its correct value early enough.
            //So here, we are forcidly repeating initialization call - so that _skip flag is used
            //after is has correct value in it.
            Initialize();

            return await new ScenarioXunitTestCaseRunner(
                this, 
                _displayName, 
                SkipReason, 
                constructorArguments, 
                TestMethodArguments, 
                messageBus, 
                aggregator, 
                cancellationTokenSource).RunAsync();
        }
    }
}
