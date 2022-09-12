using Gherkin.Ast;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioOutlineDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _messageSink;

        public ScenarioOutlineDiscoverer(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo factAttribute)
        {
            var featureClass = testMethod.TestClass.Class.ToRuntimeType();
            var featureClassInfo = FeatureClassInfo.FromFeatureClassType(featureClass);

            var features = new FeatureDiscoveryModel(new FeatureFileRepository(featureClassInfo.FileNameSearchPattern)).Discover(featureClass);

            foreach (var feature in features)
            {
                foreach (var scenarioOutline in feature.Item2.Children.OfType<ScenarioOutline>())
                {
                    foreach (var example in scenarioOutline.Examples)
                    {
                        var rowIndex = 0;
                        foreach (var row in example.TableBody)
                        {
                            var tags = feature.Item2.GetExamplesTags(scenarioOutline.Name, example.Name);
                            var skip = feature.Item2.IsExamplesIgnored(scenarioOutline.Name, example.Name);

                            yield return new ScenarioXunitTestCase(
                                _messageSink, 
                                testMethod, 
                                feature.Item2.Name,
                                !string.IsNullOrWhiteSpace(example.Name)
                                    ? $"{scenarioOutline.Name} :: {example.Name} :: #{rowIndex + 1}"
                                    : $"{scenarioOutline.Name} :: #{rowIndex + 1}",
                                tags,
                                skip,
                                new object[] { scenarioOutline.Name, example.Name, rowIndex, feature.Item1 });

                            rowIndex++;
                        }
                    }
                }
            }
        }
    }
}
