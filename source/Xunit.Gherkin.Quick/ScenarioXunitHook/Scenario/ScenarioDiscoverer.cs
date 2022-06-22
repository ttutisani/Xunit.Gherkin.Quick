using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _messageSink;

        public ScenarioDiscoverer(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions, 
            ITestMethod testMethod, 
            IAttributeInfo factAttribute)
        {
            var feature = new FeatureDiscoveryModel(new FeatureFileRepository("*.feature")).Discover(testMethod.TestClass.Class.ToRuntimeType());

            foreach (var scenario in feature.Children.OfType<global::Gherkin.Ast.Scenario>())
            {
                if (scenario.Examples.Any())
                    continue;

                var tags = feature.GetScenarioTags(scenario.Name);
                bool skip = feature.IsScenarioIgnored(scenario.Name);

                yield return new ScenarioXunitTestCase(
                    _messageSink, 
                    testMethod, 
                    feature.Name, 
                    scenario.Name, 
                    tags,
                    skip,
                    new object[] { scenario.Name });
            }
        }

    }
}
