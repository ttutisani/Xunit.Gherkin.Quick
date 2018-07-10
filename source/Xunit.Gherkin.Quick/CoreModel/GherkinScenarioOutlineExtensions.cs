using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Xunit.Gherkin.Quick
{
    internal static class GherkinScenarioOutlineExtensions
    {
        private static readonly Regex _placeholderRegex = new Regex(@"<(.+)>");

        public static global::Gherkin.Ast.Scenario ApplyExampleRow(
            this global::Gherkin.Ast.ScenarioOutline @this,
            string exampleName,
            int exampleRowIndex)
        {
            var examples = @this.Examples.FirstOrDefault(e => e.Name == exampleName);
            if (examples == null)
                throw new InvalidOperationException($"Cannot find examples named `{exampleName}` in scenario outline `{@this.Name}`.");

            var exampleRow = GetExampleRow(@this, exampleRowIndex, examples);
            var rowValues = GetExampleRowValues(examples, exampleRow);

            var scenarioSteps = new List<global::Gherkin.Ast.Step>();

            foreach (var outlineStep in @this.Steps)
            {
                var scenarioStep = DigestExampleValuesIntoStep(
                    @this, 
                    exampleName, 
                    exampleRowIndex, 
                    rowValues, 
                    outlineStep);

                scenarioSteps.Add(scenarioStep);
            }

            return new global::Gherkin.Ast.Scenario(
                @this.Tags?.ToArray(),
                @this.Location,
                @this.Keyword,
                @this.Name,
                @this.Description,
                scenarioSteps.ToArray());
        }

        private static global::Gherkin.Ast.Step DigestExampleValuesIntoStep(global::Gherkin.Ast.ScenarioOutline @this, string exampleName, int exampleRowIndex, Dictionary<string, string> rowValues, global::Gherkin.Ast.Step outlineStep)
        {
            var stepText = _placeholderRegex.Replace(outlineStep.Text,
                                match =>
                                {
                                    var placeholderKey = match.Groups[1].Value;
                                    if (!rowValues.ContainsKey(placeholderKey))
                                        throw new InvalidOperationException($"Examples table did not provide value for `{placeholderKey}`. Scenario outline: `{@this.Name}`. Examples: `{exampleName}`. Row index: {exampleRowIndex}.");

                                    var placeholderValue = rowValues[placeholderKey];

                                    return placeholderValue;
                                });

            var scenarioStep = new global::Gherkin.Ast.Step(
                outlineStep.Location,
                outlineStep.Keyword,
                stepText,
                outlineStep.Argument);
            return scenarioStep;
        }

        private static Dictionary<string, string> GetExampleRowValues(global::Gherkin.Ast.Examples examples, List<global::Gherkin.Ast.TableCell> exampleRowCells)
        {
            var rowValues = new Dictionary<string, string>();

            var headerCells = examples.TableHeader.Cells.ToList();
            for (int index = 0; index < headerCells.Count; index++)
            {
                rowValues.Add(headerCells[index].Value, exampleRowCells[index].Value);
            }

            return rowValues;
        }

        private static List<global::Gherkin.Ast.TableCell> GetExampleRow(global::Gherkin.Ast.ScenarioOutline @this, int exampleRowIndex, global::Gherkin.Ast.Examples examples)
        {
            var exampleRows = examples.TableBody.ToList();
            if (!(exampleRowIndex < exampleRows.Count))
                throw new InvalidOperationException($"Index out of range. Cannot retrieve example row at index `{exampleRowIndex}`. Example: `{examples.Name}`. Scenario outline: `{@this.Name}`.");

            var exampleRow = exampleRows[exampleRowIndex];
            var exampleRowCells = exampleRow.Cells.ToList();
            return exampleRowCells;
        }
    }
}
