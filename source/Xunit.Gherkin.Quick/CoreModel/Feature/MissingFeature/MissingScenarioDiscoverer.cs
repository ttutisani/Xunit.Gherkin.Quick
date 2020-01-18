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
            var testAssembly = testMethod.TestClass.Class.ToRuntimeType().GetTypeInfo().Assembly;
            var features = new MissingFeatureDiscoveryModel(new FeatureFileRepository(), new FeatureClassInfoRepository(testAssembly)).Discover();

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

        private static GherkinDocument GetGherkinDocumentByType(Type classType)
        {

            var fileName = classType.FullName;
            fileName = fileName.Substring(fileName.LastIndexOf('.') + 1) + ".feature";

            if (!File.Exists(fileName))
            {
                var path = (classType.GetTypeInfo().GetCustomAttributes(typeof(FeatureFileAttribute))
                    .FirstOrDefault() as FeatureFileAttribute)
                    ?.Path;

                if (path == null || !File.Exists(path))
                {
                    throw new TypeLoadException($"Cannot find feature file `{fileName}` in the output root directory. If it's somewhere else, use {nameof(FeatureFileAttribute)} to specify file path.");
                }

                fileName = path;
            }

            var parser = new Parser();
            using (var gherkinFile = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var gherkinReader = new StreamReader(gherkinFile))
                {
                    var gherkinDocument = parser.Parse(gherkinReader);
                    return gherkinDocument;
                }
            }
        }
    }
}
