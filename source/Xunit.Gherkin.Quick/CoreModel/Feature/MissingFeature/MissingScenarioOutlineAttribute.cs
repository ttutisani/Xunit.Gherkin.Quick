using System;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Xunit.Gherkin.Quick.MissingScenarioOutlineDiscoverer", "Xunit.Gherkin.Quick")]
    internal sealed class MissingScenarioOutlineAttribute : FactAttribute
    { }
}
