using System;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick.vNext.Hooks
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    [XunitTestCaseDiscoverer("Xunit.Gherkin.Quick.vNext.Hooks.TestCaseDiscoverer", "Xunit.Gherkin.Quick")]
    internal sealed class TestScenarioAttribute : FactAttribute
    {
    }
}