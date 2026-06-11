using System;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick.Hooks
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    [XunitTestCaseDiscoverer("Xunit.Gherkin.Quick.Hooks.TestCaseDiscoverer", "Xunit.Gherkin.Quick")]
    internal sealed class TestScenarioAttribute : FactAttribute
    {
    }
}