using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioXunitTestInvoker : XunitTestInvoker
    {
        private readonly TestOutputHelper _testOutputHelper;

        public ScenarioXunitTestInvoker(
            ITest test, 
            IMessageBus messageBus, 
            Type testClass, 
            object[] constructorArguments, 
            MethodInfo testMethod, 
            object[] testMethodArguments, 
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, 
            ExceptionAggregator aggregator, 
            CancellationTokenSource cancellationTokenSource,
            
            TestOutputHelper testOutputHelper) 
            
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, beforeAfterAttributes, aggregator, cancellationTokenSource)
        {
            _testOutputHelper = testOutputHelper;
        }

        protected override async Task<decimal> InvokeTestMethodAsync(object testClassInstance)
        {
            var featureClassInstance = testClassInstance as Feature;
            if (featureClassInstance == null)
                throw new InvalidOperationException($"Test class should derive from `{nameof(Feature)}`.");

            featureClassInstance.Output = _testOutputHelper;

            await featureClassInstance.InvokeAsync(BeforeAfterAttributeHelper.InvokeMethodType.BeforeFeature);
            var r = await base.InvokeTestMethodAsync(testClassInstance);
            await featureClassInstance.InvokeAsync(BeforeAfterAttributeHelper.InvokeMethodType.AfterFeature);

            return r;
        }
    }
}
