using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    internal sealed class MissingScenarioDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _messageSink;

        public MissingScenarioDiscoverer(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions, 
            ITestMethod testMethod, 
            IAttributeInfo factAttribute)
        {
            var missingFeatureClass = testMethod.TestClass.Class.ToRuntimeType();
            var missingFeatureClassInfo = MissingFeatureClassInfo.FromMissingFeatureClassType(missingFeatureClass);
            var testAssembly = missingFeatureClass.GetTypeInfo().Assembly;
            var features = new MissingFeatureDiscoveryModel(new FeatureFileRepository(missingFeatureClassInfo.FileNameSearchPattern), new FeatureClassInfoRepository(testAssembly)).Discover();

            foreach (var feature in features)
            {
                foreach (var scenario in feature.Children.OfType<global::Gherkin.Ast.Scenario>())
                {
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
}
