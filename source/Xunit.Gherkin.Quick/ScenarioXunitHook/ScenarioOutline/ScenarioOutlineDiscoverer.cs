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
            var featureFiles = new FeatureDiscoveryModel(new FeatureFileRepository("*.feature")).Discover(testMethod.TestClass.Class.ToRuntimeType());

            foreach (var featureFile in featureFiles)
            {
                foreach (var scenarioOutline in featureFile.Feature.Children.OfType<ScenarioOutline>())
                {
                    foreach (var example in scenarioOutline.Examples)
                    {
                        var rowIndex = 0;
                        foreach (var row in example.TableBody)
                        {
                            var tags = featureFile.Feature.GetExamplesTags(scenarioOutline.Name, example.Name);
                            var skip = featureFile.Feature.IsExamplesIgnored(scenarioOutline.Name, example.Name);

                            yield return new ScenarioXunitTestCase(
                                _messageSink, 
                                testMethod, 
                                featureFile.Feature.Name,
                                !string.IsNullOrWhiteSpace(example.Name)
                                    ? $"{scenarioOutline.Name} :: {example.Name} :: #{rowIndex + 1}"
                                    : $"{scenarioOutline.Name} :: #{rowIndex + 1}",
                                tags,
                                skip,
                                new object[] { scenarioOutline.Name, example.Name, rowIndex, featureFile.Path });

                            rowIndex++;
                        }
                    }
                }
            }
        }
    }
}
