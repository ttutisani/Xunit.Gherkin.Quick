using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{

    public sealed class IgnoreMethodDiscoverer : IXunitTestCaseDiscoverer
    {
        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            yield break;
        }
    }
}
