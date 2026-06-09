using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.vNext.TestScenarios
{
    internal class TestScenario : IXunitSerializable
    {
        private static readonly IDictionary<string, CultureInfo> _cultureInfosByName = new Dictionary<string, CultureInfo>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestScenario()
        {
        }

        internal TestScenario(string featureName, string scenarioName, CultureInfo locale, IReadOnlyList<string> tags, IReadOnlyList<TestStep> steps)
        {
            FeatureName = featureName;
            ScenarioName = scenarioName;
            Locale = locale;
            Tags = tags ?? Array.Empty<string>();
            Steps = steps ?? Array.Empty<TestStep>();
        }

        public string FeatureName { get; private set; }
        public string ScenarioName { get; private set; }
        public CultureInfo Locale { get; private set; }
        public IReadOnlyCollection<string> Tags { get; private set; }
        public IReadOnlyList<TestStep> Steps { get; private set; }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(FeatureName), FeatureName, typeof(string));
            info.AddValue(nameof(ScenarioName), ScenarioName, typeof(string));
            info.AddValue(nameof(Locale), Locale.Name, typeof(string));
            info.AddValue(nameof(Tags), Tags as string[] ?? Tags.ToArray(), typeof(string[]));
            info.AddValue(nameof(Steps), Steps as TestStep[] ?? Steps.ToArray(), typeof(TestStep[]));
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            FeatureName = info.GetValue<string>(nameof(FeatureName));
            ScenarioName = info.GetValue<string>(nameof(ScenarioName));
            Locale = new CultureInfo(info.GetValue<string>(nameof(Locale)));
            Tags = info.GetValue<string[]>(nameof(Tags)) ?? Array.Empty<string>();
            Steps = info.GetValue<TestStep[]>(nameof(Steps)) ?? Array.Empty<TestStep>();
        }

        internal static TestScenario From(global::Gherkin.Ast.Feature feature, global::Gherkin.Ast.Scenario scenario)
        {
            var tags = (feature.Tags ?? Enumerable.Empty<global::Gherkin.Ast.Tag>())
                .Concat(scenario.Tags ?? Enumerable.Empty<global::Gherkin.Ast.Tag>())
                .Where(tag => !string.IsNullOrWhiteSpace(tag.Name))
                .Select(tag => tag.Name.StartsWith("@") ? tag.Name.Substring(1) : tag.Name)
                .ToArray();

            var testSteps = new TestStep[scenario.Steps.Count()];
            var testStepIndex = 0;
            foreach (var step in scenario.Steps)
            {
                TestStepType testStepType;
                if (step.Keyword.IndexOf("given", StringComparison.OrdinalIgnoreCase) >= 0 || step.Keyword.IndexOf("*", StringComparison.OrdinalIgnoreCase) >= 0)
                    testStepType = TestStepType.Given;
                else if (step.Keyword.IndexOf("when", StringComparison.OrdinalIgnoreCase) >= 0)
                    testStepType = TestStepType.When;
                else if (step.Keyword.IndexOf("then", StringComparison.OrdinalIgnoreCase) >= 0)
                    testStepType = TestStepType.Then;
                else
                    testStepType = TestStepType.And;

                TestStep testStep;
                if (step.Argument is global::Gherkin.Ast.DocString docStringArgument)
                    testStep = new TestStep(testStepType, step.Text, _GetDocStringArgument(docStringArgument));
                else if (step.Argument is global::Gherkin.Ast.DataTable dataTableArgument)
                    testStep = new TestStep(testStepType, step.Text, _GetTableArgument(dataTableArgument));
                else
                    testStep = new TestStep(testStepType, step.Text);


                testSteps[testStepIndex] = testStep;
                testStepIndex++;
            }

            return new TestScenario(
                feature.Name,
                scenario.Name,
                _GetCultureInfo(feature.Language),
                tags,
                testSteps
            );
        }

        private static TestStepDocStringArgument _GetDocStringArgument(global::Gherkin.Ast.DocString docStringArgument)
            => new TestStepDocStringArgument(docStringArgument.Content, docStringArgument.ContentType, _GetLocation(docStringArgument.Location));

        private static TestStepTableArgument _GetTableArgument(global::Gherkin.Ast.DataTable dataTableArgument)
        {
            var rows = new TestStepTableRowArgument[dataTableArgument.Rows.Count()];
            var rowIndex = 0;
            foreach (var row in dataTableArgument.Rows)
                rows[rowIndex++] = _GetTableRow(row);

            return new TestStepTableArgument(rows, _GetLocation(dataTableArgument.Location));
        }

        private static TestStepTableRowArgument _GetTableRow(global::Gherkin.Ast.TableRow row)
        {
            var cells = new TestStepTableRowCellArgument[row.Cells.Count()];
            var cellIndex = 0;
            foreach (var cell in row.Cells)
            {
                cells[cellIndex] = new TestStepTableRowCellArgument(cell.Value, new TestStepArgumentLocation(cell.Location.Line, cell.Location.Column));
                cellIndex++;
            }

            return new TestStepTableRowArgument(cells, _GetLocation(row.Location));
        }

        private static TestStepArgumentLocation _GetLocation(global::Gherkin.Ast.Location location)
            => new TestStepArgumentLocation(location.Line, location.Column);

        private static CultureInfo _GetCultureInfo(string language)
        {
            CultureInfo cultureInfo;
            try
            {
                if (!_cultureInfosByName.TryGetValue(language, out cultureInfo))
                {
                    cultureInfo = new CultureInfo(language);
                    _cultureInfosByName.Add(language, cultureInfo);
                }
            }
            catch (CultureNotFoundException)
            {
                cultureInfo = new CultureInfo("en");
                _cultureInfosByName.Add(language, cultureInfo);
            }

            return cultureInfo;
        }
    }
}