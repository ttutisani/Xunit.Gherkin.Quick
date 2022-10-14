using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var featureClassType = testMethod.TestClass.Class.ToRuntimeType();
            TestAssemblyInfo testAssemblyInfo = TestAssemblyInfo.FromAssembly(featureClassType.GetTypeInfo().Assembly);

            var featureFiles = new FeatureDiscoveryModel(new FeatureFileRepository(testAssemblyInfo.FeatureFileSearchPattern)).Discover(featureClassType);

            foreach (var featureFile in featureFiles)
            {
                foreach (var scenario in featureFile.Feature.Children.OfType<global::Gherkin.Ast.Scenario>())
                {
                    var tags = featureFile.Feature.GetScenarioTags(scenario.Name);
                    bool skip = featureFile.Feature.IsScenarioIgnored(scenario.Name);

                    yield return new ScenarioXunitTestCase(
                        _messageSink, 
                        testMethod, 
                        featureFile.Feature.Name, 
                        scenario.Name, 
                        tags,
                        skip,
                        new object[] { scenario.Name, featureFile.Path });
                }
            }
        }

    }
}
