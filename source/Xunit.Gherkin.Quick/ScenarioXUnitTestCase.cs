using System;
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
    }
}
