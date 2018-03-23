using Gherkin;
using Gherkin.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Gherkin.Quick
{
    public abstract class Feature
    {
        /// <summary>Allows you to log extra data to the result of the test.</summary>
        protected ITestOutputHelper Output { get; set; }

        [Obsolete("To recieve step-level output from your tests, use the constructor passing an ITestOutputHelper")]
        protected Feature() {}

        /// <summary>Create a new Feature.</summary>
        /// <param name="output">
        /// Allows you to log extra data to the result of the test. Xunit will provide you with an implementation
        /// in your constructor which you can pass straight through e.g. public MyFeatureClass(ITestOutputHelper output) : base(output)
        /// </param>
        protected Feature(ITestOutputHelper output) => Output = output;

        [Scenario]
        internal void Scenario(string scenarioName)
        {
            var gherkinDocument = ScenarioDiscoverer.GetGherkinDocumentByType(GetType());

            var parsedScenario = gherkinDocument.Feature.Children.FirstOrDefault(scenario => scenario.Name == scenarioName);
            if (parsedScenario == null)
                throw new Exception($"Cannot find scenario `{scenarioName}`.");

            var stepMethods = GetType().GetTypeInfo().GetMethods()
                .Where(m => m.IsDefined(typeof(KeyWordAttributeBase)))
                .Select(m => new { method = m, keywordAttribute = m.GetCustomAttribute<KeyWordAttributeBase>() });

            foreach (var parsedStep in parsedScenario.Steps)
            {
                var matchingStepMethod = stepMethods.FirstOrDefault(stepMethod => 
                    stepMethod.keywordAttribute.MatchesStep(parsedStep.Keyword, parsedStep.Text));
                if (matchingStepMethod == null)
                    throw new Exception($"Cannot find scenario step `{parsedStep.Keyword}{parsedStep.Text}` for scenario `{scenarioName}`.");

                var stepRegexMatch = matchingStepMethod.keywordAttribute.MatchRegex(parsedStep.Text);
                if (!string.Equals(stepRegexMatch.Value, parsedStep.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    throw new Exception($"Step method partially matched but not selected. Step `{parsedStep.Text.Trim()}`, Method pattern `{matchingStepMethod.keywordAttribute.Pattern}`.");

                var methodParams = matchingStepMethod.method.GetParameters();
                object[] methodParamValues = null;
                if (methodParams.Length > 0)
                {
                    var methodParamStringValues = stepRegexMatch.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToList();

                    if (methodParamStringValues.Count < methodParams.Length)
                        throw new Exception($"Method `{matchingStepMethod.method.Name}` for step `{parsedStep.Keyword}{parsedStep.Text}` is expecting {methodParams.Length} params, but only {methodParamStringValues.Count} param values were supplied.");

                    methodParamValues = methodParams.Select((p, i) => Convert.ChangeType(methodParamStringValues[i], p.ParameterType))
                        .ToArray();
                }

                try
                {
                    matchingStepMethod.method.Invoke(this, methodParamValues);
                    Output?.WriteLine($"{parsedStep.Keyword} {parsedStep.Text}: PASSED");
                }
                catch
                {
                    Output?.WriteLine($"{parsedStep.Keyword} {parsedStep.Text}: FAILED");
                    throw;
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Xunit.Gherkin.Quick.ScenarioDiscoverer", "Xunit.Gherkin.Quick")]
    internal sealed class ScenarioAttribute : FactAttribute
    { }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class FeatureFileAttribute : Attribute
    {
        /// <summary>
        /// Specify path to feature file (can be relative to output folder).
        /// </summary>
        /// <param name="path">
        /// Path to feature file. Can be relative to output folder,
        /// e.g. "./SampleFolder/Sample.feature"
        /// </param>
        public FeatureFileAttribute(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public string Path { get; }
    }

    public sealed class ScenarioDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _messageSink;

        public ScenarioDiscoverer(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            var gherkinDocument = GetGherkinDocumentByType(testMethod.TestClass.Class.ToRuntimeType());

            foreach (var scenario in gherkinDocument.Feature.Children)
            {
                yield return new ScenarioXUnitTestCase(_messageSink, testMethod, $"{gherkinDocument.Feature.Name} :: {scenario.Name}", new object[] { scenario.Name });
            }
        }

        public static GherkinDocument GetGherkinDocumentByType(Type classType)
        {
            var fileName = classType.FullName;
            fileName = fileName.Substring(fileName.LastIndexOf('.') + 1) + ".feature";

            if (!System.IO.File.Exists(fileName))
            {
                var path = (classType.GetTypeInfo().GetCustomAttributes(typeof(FeatureFileAttribute))
                    .FirstOrDefault() as FeatureFileAttribute)
                    ?.Path;

                if (path == null || !System.IO.File.Exists(path))
                {
                    throw new Exception($"Cannot find feature file `{fileName}` in the output root directory. If it's somewhere else, use {nameof(FeatureFileAttribute)} to specify file path.");
                }

                fileName = path;
            }

            var parser = new Parser();
            var gherkinDocument = parser.Parse(fileName);
            return gherkinDocument;
        }
    }

    public sealed class ScenarioXUnitTestCase : XunitTestCase
    {
        [Obsolete]
        public ScenarioXUnitTestCase()
        {
        }

        public ScenarioXUnitTestCase(IMessageSink messageSink, ITestMethod testMethod, string scenarioName, object[] testMethodArguments = null)
            : base(messageSink, TestMethodDisplay.Method, testMethod, testMethodArguments)
        {
            DisplayName = scenarioName;
        }
    }
}
