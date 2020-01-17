using System;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Xunit.Gherkin.Quick.MissingScenarioDiscoverer", "Xunit.Gherkin.Quick")]
    internal sealed class MissingScenarioAttribute : FactAttribute
    { }
}
