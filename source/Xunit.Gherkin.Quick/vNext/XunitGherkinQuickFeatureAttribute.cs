using System;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick.vNext
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    [XunitTestCaseDiscoverer("Xunit.Gherkin.Quick.vNext.XunitGherkinQuickTestCaseDiscoverer", "Xunit.Gherkin.Quick")]
    internal sealed class XunitGherkinQuickFeatureAttribute : FactAttribute
    {
    }
}