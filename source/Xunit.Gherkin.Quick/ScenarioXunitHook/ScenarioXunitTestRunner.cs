using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioXUnitTestRunner : XunitTestRunner
    {
        public ScenarioXUnitTestRunner(
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
            
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes, aggregator, cancellationTokenSource)
        {
        }

        private TestOutputHelper _testOutputHelper;

        protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            _testOutputHelper = FindOutputHelperInArguments(ConstructorArguments);
            var shouldOwnTestOutputHelper = _testOutputHelper == null;
            if (shouldOwnTestOutputHelper)
            {
                _testOutputHelper = new TestOutputHelper();
                _testOutputHelper.Initialize(MessageBus, Test);
            }

            var executionResult = await base.InvokeTestAsync(aggregator);

            if (shouldOwnTestOutputHelper)
            {
                executionResult = new Tuple<decimal, string>(executionResult.Item1, _testOutputHelper.Output);
                _testOutputHelper.Uninitialize();
            }

            return executionResult;
        }

        protected override async Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
        {
            return await new ScenarioXUnitTestInvoker(
                Test,
                MessageBus,
                TestClass,
                ConstructorArguments,
                TestMethod,
                TestMethodArguments,
                BeforeAfterAttributes,
                aggregator,
                CancellationTokenSource,

                _testOutputHelper
                ).RunAsync();
        }

        private static TestOutputHelper FindOutputHelperInArguments(object[] args)
        {
            return args.FirstOrDefault(argObject => argObject is TestOutputHelper)
                as TestOutputHelper;
        }
    }
}
