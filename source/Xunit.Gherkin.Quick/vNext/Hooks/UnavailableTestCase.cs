using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick.vNext.Hooks
{
    internal class UnavailableTestCase : XunitTestCase
    {
        private string _skipReason;
        private string _displayName;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public UnavailableTestCase()
        {
        }

        public UnavailableTestCase(
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
}