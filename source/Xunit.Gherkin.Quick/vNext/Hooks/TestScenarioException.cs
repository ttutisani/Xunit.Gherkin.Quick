using System;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick.vNext.Hooks
{
    internal class TestScenarioException : XunitException
    {
        public TestScenarioException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}