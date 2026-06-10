using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Gherkin;

namespace Xunit.Gherkin.Quick.vNext.TestScenarios
{
    internal class TestScenarioMapper
    {
        private readonly IDictionary<string, CultureInfo> _cultureInfosByName = new Dictionary<string, CultureInfo>();
        private readonly IGherkinDialectProvider _gherkinDialectProvider;

        internal TestScenarioMapper(global::Gherkin.IGherkinDialectProvider gherkinDialectProvider)
            => _gherkinDialectProvider = gherkinDialectProvider;

        internal TestScenario Map(global::Gherkin.Ast.Feature feature, global::Gherkin.Ast.Scenario scenario)
        {
            var gherkinDialect = _gherkinDialectProvider.GetDialect(feature.Language, null);
            var tags = (feature.Tags ?? Enumerable.Empty<global::Gherkin.Ast.Tag>())
                .Concat(scenario.Tags ?? Enumerable.Empty<global::Gherkin.Ast.Tag>())
                .Where(tag => !string.IsNullOrWhiteSpace(tag.Name))
                .Select(tag => tag.Name.StartsWith("@") ? tag.Name.Substring(1) : tag.Name)
                .ToArray();

            var testSteps = new TestStep[scenario.Steps.Count()];
            var testStepIndex = 0;
            foreach (var step in scenario.Steps)
            {
                var testStepType = TestStepType.Unknown;
                if (step.Keyword.IndexOf("*", StringComparison.OrdinalIgnoreCase) >= 0)
                    testStepType = TestStepType.All;
                else if (gherkinDialect.GivenStepKeywords.Contains(step.Keyword, StringComparer.OrdinalIgnoreCase))
                    testStepType = TestStepType.Given;
                else if (gherkinDialect.WhenStepKeywords.Contains(step.Keyword, StringComparer.OrdinalIgnoreCase))
                    testStepType = TestStepType.When;
                else if (gherkinDialect.ThenStepKeywords.Contains(step.Keyword, StringComparer.OrdinalIgnoreCase))
                    testStepType = TestStepType.Then;
                else if (gherkinDialect.AndStepKeywords.Contains(step.Keyword, StringComparer.OrdinalIgnoreCase))
                    testStepType = TestStepType.And;
                else if (gherkinDialect.ButStepKeywords.Contains(step.Keyword, StringComparer.OrdinalIgnoreCase))
                    testStepType = TestStepType.But;

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

        private CultureInfo _GetCultureInfo(string language)
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
    }
}