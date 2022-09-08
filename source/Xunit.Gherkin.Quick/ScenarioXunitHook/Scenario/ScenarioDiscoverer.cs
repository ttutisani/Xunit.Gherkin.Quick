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
            var features = new FeatureDiscoveryModel(new FeatureFileRepository("*.feature")).Discover(testMethod.TestClass.Class.ToRuntimeType());
            foreach (var feature in features)
            {
                foreach (var scenario in feature.Item2.Children.OfType<global::Gherkin.Ast.Scenario>())
                {
                    var tags = feature.Item2.GetScenarioTags(scenario.Name);
                    bool skip = feature.Item2.IsScenarioIgnored(scenario.Name);

                    yield return new ScenarioXunitTestCase(
                        _messageSink, 
                        testMethod, 
                        feature.Item2.Name, 
                        scenario.Name, 
                        tags,
                        skip,
                        new object[] { scenario.Name, feature.Item1 });
                }
            }
        }

    }
}
