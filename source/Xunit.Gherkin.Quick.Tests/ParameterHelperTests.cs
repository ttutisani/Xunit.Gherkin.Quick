using FluentAssertions;
using Gherkin.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xunit.Gherkin.Quick.Tests
{
    public class ParameterHelperTests
    {
        [Fact]
        public void GetParamValues_NoTableData()
        {
            var method = typeof(SimpleTestClass).GetMethod("NoTableData1");

            var step = new Step(null, "Given", "some data", null);

            var specParams = new List<string>
            {
                "String Value 1",
                "250"
            };

            var values = ParameterHelper.GetParamValues(method, step, specParams);

            values.Should().NotBeNull()
                .And.HaveCount(2);
            values[0].Should().Be("String Value 1");
            values[1].Should().Be(250);
        }

        [Fact]
        public void GetParamValues_TableDataAtStart()
        {
            var method = typeof(SimpleTestClass).GetMethod("TableDataAtStart1");

            var step = new Step(null,
                "Given",
                "some data",
                new DataTable(new[] { new TableRow(null, new[] { new TableCell(null, "A value 1") }) }));

            var specParams = new List<string>
            {
                "String Value 2",
                "251"
            };

            var values = ParameterHelper.GetParamValues(method, step, specParams);

            values.Should().NotBeNull()
                .And.HaveCount(3);
            values[0].Should().BeOfType<DataTable>();
            values[0].As<DataTable>().Rows.First().Cells.First().Value.Should().Be("A value 1");
            values[1].Should().Be("String Value 2");
            values[2].Should().Be(251);
        }

        [Fact]
        public void GetParamValues_TableDataAtEnd()
        {
            var method = typeof(SimpleTestClass).GetMethod("TableDataAtEnd1");

            var step = new Step(null, "Given", "some data", new DataTable(new[] { new TableRow(null, new[] { new TableCell(null, "A value 2") }) }));

            var specParams = new List<string>
            {
                "String Value 3",
                "252"
            };

            var values = ParameterHelper.GetParamValues(method, step, specParams);

            values.Should().NotBeNull()
                .And.HaveCount(3);
            values[0].Should().Be("String Value 3");
            values[1].Should().Be(252);
            values[2].Should().BeOfType<DataTable>();
            values[2].As<DataTable>().Rows.First().Cells.First().Value.Should().Be("A value 2");
        }

        [Fact]
        public void GetParamValues_TableParamExpectedButNotSupplied()
        {
            var method = typeof(SimpleTestClass).GetMethod("TableDataAtEnd1");

            var step = new Step(null, "Given", "some data", null); // We don't send any table data

            var specParams = new List<string>
            {
                "String Value 4",
                "253"
            };

            Action getParamValues = () => ParameterHelper.GetParamValues(method, step, specParams);

            getParamValues.Should().Throw<Exception>()
                .WithMessage("Method `TableDataAtEnd1` for step `Givensome data` is expecting a table parameter, but none was supplied.");
        }

        private class SimpleTestClass
        {
            public void NoTableData1(string value1, int value2) { }
            public void TableDataAtStart1(DataTable table, string value1, int value2) { }
            public void TableDataAtEnd1(string value1, int value2, DataTable table) { }
        }
    }
}
