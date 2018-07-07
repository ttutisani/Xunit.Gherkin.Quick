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
            var example = @this.Examples.FirstOrDefault(e => e.Name == exampleName);
            if (example == null)
                throw new InvalidOperationException($"Cannot find example named `{exampleName}` in scenario outline `{@this.Name}`.");

            var exampleRows = example.TableBody.ToList();
            if (!(exampleRowIndex < exampleRows.Count))
                throw new InvalidOperationException($"Index out of range. Cannot retrieve example row at index `{exampleRowIndex}`. Example: `{example.Name}`. Scenario outline: `{@this.Name}`.");

            var exampleRow = exampleRows[exampleRowIndex];
            var rowCells = exampleRow.Cells.ToList();

            var rowValues = new Dictionary<string, string>();

            var headerCells = example.TableHeader.Cells.ToList();
            for (int index = 0; index < headerCells.Count; index++)
            {
                rowValues.Add(headerCells[index].Value, rowCells[index].Value);
            }

            var scenarioSteps = new List<global::Gherkin.Ast.Step>();

            foreach (var outlineStep in @this.Steps)
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
    }
}
