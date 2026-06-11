using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Gherkin;
using Gherkin.Ast;

namespace Xunit.Gherkin.Quick.TestScenarios
{
    internal class TestScenarioMapper
    {
        private readonly IDictionary<string, CultureInfo> _cultureInfosByName = new Dictionary<string, CultureInfo>();
        private readonly IGherkinDialectProvider _gherkinDialectProvider;

        internal TestScenarioMapper(global::Gherkin.IGherkinDialectProvider gherkinDialectProvider)
            => _gherkinDialectProvider = gherkinDialectProvider;

        internal TestScenario Map(GherkinDocument document, Scenario scenario, IReadOnlyDictionary<string, string> arguments = null)
        {
            arguments = arguments ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var parameterReplacePattern = new Regex($"<(?<parameterName>{string.Join("|", arguments.Keys.Select(Regex.Escape))})>", RegexOptions.IgnoreCase);
            string _ReplaceParameters(string value)
            {
                if (value is null)
                    return null;
                else
                    return parameterReplacePattern.Replace(
                        value,
                        match => arguments.TryGetValue(match.Groups["parameterName"].Value, out var argument)
                            ? argument
                            : match.Value
                    );
            }

            var gherkinDialect = _gherkinDialectProvider.GetDialect(document.Feature.Language, null);
            var tags = _GetTags(document.Feature, scenario);

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
                if (step.Argument is DocString docStringArgument)
                    testStep = new TestStep(testStepType, _ReplaceParameters(step.Text), _GetDocStringArgument(docStringArgument, _ReplaceParameters));
                else if (step.Argument is DataTable dataTableArgument)
                    testStep = new TestStep(testStepType, _ReplaceParameters(step.Text), _GetTableArgument(dataTableArgument, _ReplaceParameters));
                else
                    testStep = new TestStep(testStepType, _ReplaceParameters(step.Text));

                testSteps[testStepIndex] = testStep;
                testStepIndex++;
            }

            return new TestScenario(
                _ReplaceParameters(document.Feature.Name),
                _ReplaceParameters(scenario.Name),
                (
                    _TryGetCultureInfo(document.Feature.Language)
                    ?? _TryGetCultureInfo(_gherkinDialectProvider.DefaultDialect.Language)
                    ?? CultureInfo.InvariantCulture
                ),
                tags.ToArray(),
                testSteps
            );
        }

        private CultureInfo _TryGetCultureInfo(string language)
        {
            CultureInfo cultureInfo = null;
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
            }

            return cultureInfo;
        }

        private static TestStepDocStringArgument _GetDocStringArgument(DocString docStringArgument, Func<string, string> parameterReplacer)
            => new TestStepDocStringArgument(parameterReplacer(docStringArgument.Content), parameterReplacer(docStringArgument.ContentType), _GetLocation(docStringArgument.Location));

        private static TestStepTableArgument _GetTableArgument(DataTable dataTableArgument, Func< string, string> parameterReplacer)
        {
            var rows = new TestStepTableRowArgument[dataTableArgument.Rows.Count()];
            var rowIndex = 0;
            foreach (var row in dataTableArgument.Rows)
                rows[rowIndex++] = _GetTableRow(row, parameterReplacer);

            return new TestStepTableArgument(rows);
        }

        private static TestStepTableRowArgument _GetTableRow(TableRow row, Func<string, string> parameterReplacer)
        {
            var cells = new TestStepTableCellArgument[row.Cells.Count()];
            var cellIndex = 0;
            foreach (var cell in row.Cells)
            {
                cells[cellIndex] = new TestStepTableCellArgument(parameterReplacer(cell.Value), new TestStepArgumentLocation(cell.Location.Line, cell.Location.Column));
                cellIndex++;
            }

            return new TestStepTableRowArgument(cells, _GetLocation(row.Location));
        }

        private static IEnumerable<string> _GetTags(params IHasTags[] hasTagsCollection)
            => hasTagsCollection
                .SelectMany(hasTags => hasTags.Tags ?? Enumerable.Empty<Tag>())
                .Where(tag => !string.IsNullOrWhiteSpace(tag.Name))
                .Select(tag => tag.Name.StartsWith("@") ? tag.Name.Substring(1) : tag.Name)
                .GroupBy(tag => tag, (uniqueTag, tags) => uniqueTag, StringComparer.OrdinalIgnoreCase)
                .AsEnumerable();

        private static TestStepArgumentLocation _GetLocation(Location location)
            => new TestStepArgumentLocation(location.Line, location.Column);
    }
}