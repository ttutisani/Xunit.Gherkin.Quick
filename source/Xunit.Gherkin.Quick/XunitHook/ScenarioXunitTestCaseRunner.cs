using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioXunitTestCaseRunner : XunitTestCaseRunner
    {
        public ScenarioXunitTestCaseRunner(
            IXunitTestCase testCase, 
            string displayName, 
            string skipReason, 
            object[] constructorArguments, 
            object[] testMethodArguments, 
            IMessageBus messageBus, 
            ExceptionAggregator aggregator, 
            CancellationTokenSource cancellationTokenSource) 
            
            : base(testCase, displayName, skipReason, constructorArguments, testMethodArguments, messageBus, aggregator, cancellationTokenSource)
        {
        }

        protected override XunitTestRunner CreateTestRunner(
            ITest test, 
            IMessageBus messageBus, 
            Type testClass, 
            object[] constructorArguments, 
            MethodInfo testMethod, 
            object[] testMethodArguments, 
            string skipReason, 
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, 
            ExceptionAggregator aggregator, 
            CancellationTokenSource cancellationTokenSource)
        {
            return new ScenarioXunitTestRunner(
                test, 
                messageBus, 
                testClass, 
                constructorArguments, 
                testMethod, 
                testMethodArguments, 
                skipReason, 
                beforeAfterAttributes, 
                aggregator, 
                cancellationTokenSource);
        }
    }
}
