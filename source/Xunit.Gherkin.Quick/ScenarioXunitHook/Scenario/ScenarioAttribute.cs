using System;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Xunit.Gherkin.Quick.ScenarioDiscoverer", "Xunit.Gherkin.Quick")]
    internal sealed class ScenarioAttribute : FactAttribute
    { }
}
