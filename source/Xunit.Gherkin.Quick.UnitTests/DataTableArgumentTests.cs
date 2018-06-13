using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class DataTableArgumentTests
    {
        [Fact]
        public void DigestScenarioStepValues_Throws_Error_If_No_Arguments_And_No_DataTable()
        {
            //arrange.
            var sut = new DataTableArgument();

            //act / assert.
            Assert.Throws<InvalidOperationException>(() => sut.DigestScenarioStepValues(new string[0], null));
        }

        [Fact]
        public void DigestScenarioStepValues_Throws_Error_If_Arguments_Present_But_No_DataTable()
        {
            //arrange.
            var sut = new DataTableArgument();

            //act / assert.
            Assert.Throws<InvalidOperationException>(() => sut.DigestScenarioStepValues(new string[] { "1", "2", "3" }, null));
        }

        [Fact]
        public void DigestScenarioStepValues_Sets_Value_As_DataTable_When_Only_DataTable()
        {
            //arrange.
            var sut = new DataTableArgument();
            var dataTable = CreateGherkinDocument("scenario123",
                    "When some step text" + Environment.NewLine +
@"  | First argument | Second argument | Result |
    | 1              |       2         |       3|
    | a              |   b             | c      |
"
                    ).Feature.Children.ElementAt(0).Steps.ElementAt(0).Argument as Gherkin.Ast.DataTable;

            //act.
            sut.DigestScenarioStepValues(new string[0], dataTable);

            //assert.
            Assert.Same(dataTable, sut.Value);
        }

        [Fact]
        public void DigestScenarioStepValues_Sets_Value_As_DataTable_When_DataTable_And_Other_Args_Present()
        {
            //arrange.
            var sut = new DataTableArgument();
            var dataTable = CreateGherkinDocument("scenario123",
                    "When some step text" + Environment.NewLine +
@"  | First argument | Second argument | Result |
    | 1              |       2         |       3|
    | a              |   b             | c      |
"
                    ).Feature.Children.ElementAt(0).Steps.ElementAt(0).Argument as Gherkin.Ast.DataTable;

            //act.
            sut.DigestScenarioStepValues(new string[] { "1", "2", "3" }, dataTable);

            //assert.
            Assert.Same(dataTable, sut.Value);
        }

        private static Gherkin.Ast.GherkinDocument CreateGherkinDocument(string scenario, params string[] steps)
        {
            var gherkinText =
@"Feature: Some Sample Feature
    In order to learn Math
    As a regular human
    I want to add two numbers using Calculator

Scenario: " + scenario + @"
" + string.Join(Environment.NewLine, steps)
;
            using (var gherkinStream = new MemoryStream(Encoding.UTF8.GetBytes(gherkinText)))
            using (var gherkinReader = new StreamReader(gherkinStream))
            {
                var parser = new Gherkin.Parser();
                return parser.Parse(gherkinReader);
            }
        }

    }
}
