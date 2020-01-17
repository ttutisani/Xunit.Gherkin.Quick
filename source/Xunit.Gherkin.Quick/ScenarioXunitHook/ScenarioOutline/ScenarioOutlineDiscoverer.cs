using Gherkin;
using Gherkin.Ast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            var feature = new FeatureDiscoveryModel(new FeatureFileRepository()).Discover(testMethod.TestClass.Class.ToRuntimeType());

            foreach (var scenarioOutline in feature.Children.OfType<ScenarioOutline>())
            {
                foreach (var example in scenarioOutline.Examples)
                {
                    var rowIndex = 0;
                    foreach (var row in example.TableBody)
                    {
                        var tags = feature.GetExamplesTags(scenarioOutline.Name, example.Name);
                        var skip = feature.IsExamplesIgnored(scenarioOutline.Name, example.Name);

                        yield return new ScenarioXunitTestCase(
                            _messageSink, 
                            testMethod, 
                            feature.Name,
                            !string.IsNullOrWhiteSpace(example.Name)
                                ? $"{scenarioOutline.Name} :: {example.Name} :: #{rowIndex + 1}"
                                : $"{scenarioOutline.Name} :: #{rowIndex + 1}",
                            tags,
                            skip,
                            new object[] { scenarioOutline.Name, example.Name, rowIndex });

                        rowIndex++;
                    }
                }
            }
        }
    }
}
