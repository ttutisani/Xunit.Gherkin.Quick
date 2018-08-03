﻿using Gherkin;
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
            var feature = GetGherkinDocumentByType(testMethod.TestClass.Class.ToRuntimeType())
                .Feature;

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
