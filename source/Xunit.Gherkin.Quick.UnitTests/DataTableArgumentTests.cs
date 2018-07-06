using Moq;
using System;
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
            var dataTable = new Gherkin.Ast.DataTable(new Gherkin.Ast.TableRow[]
                {
                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                    {
                        new Gherkin.Ast.TableCell(null, "First argument"),
                        new Gherkin.Ast.TableCell(null, "Second argument"),
                        new Gherkin.Ast.TableCell(null, "Result"),
                    }),
                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                    {
                        new Gherkin.Ast.TableCell(null, "1"),
                        new Gherkin.Ast.TableCell(null, "2"),
                        new Gherkin.Ast.TableCell(null, "3"),
                    }),
                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                    {
                        new Gherkin.Ast.TableCell(null, "a"),
                        new Gherkin.Ast.TableCell(null, "b"),
                        new Gherkin.Ast.TableCell(null, "c"),
                    })
                });

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
            var dataTable = new Gherkin.Ast.DataTable(new Gherkin.Ast.TableRow[]
                {
                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                    {
                        new Gherkin.Ast.TableCell(null, "First argument"),
                        new Gherkin.Ast.TableCell(null, "Second argument"),
                        new Gherkin.Ast.TableCell(null, "Result"),
                    }),
                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                    {
                        new Gherkin.Ast.TableCell(null, "1"),
                        new Gherkin.Ast.TableCell(null, "2"),
                        new Gherkin.Ast.TableCell(null, "3"),
                    }),
                    new Gherkin.Ast.TableRow(null, new Gherkin.Ast.TableCell[]
                    {
                        new Gherkin.Ast.TableCell(null, "a"),
                        new Gherkin.Ast.TableCell(null, "b"),
                        new Gherkin.Ast.TableCell(null, "c"),
                    })
                });

            //act.
            sut.DigestScenarioStepValues(new string[] { "1", "2", "3" }, dataTable);

            //assert.
            Assert.Same(dataTable, sut.Value);
        }
        
        [Fact]
        public void IsSameAs_Identifies_Similar_Instances()
        {
            //arrange.
            var sut = new DataTableArgument();
            var other = new DataTableArgument();

            //act.
            var same = sut.IsSameAs(other);

            //assert.
            Assert.True(same);
        }

        [Fact]
        public void IsSameAs_Distinguishes_Different_Instances()
        {
            //arrange.
            var sut = new DataTableArgument();
            var other = new Mock<StepMethodArgument>().Object;

            //act.
            var same = sut.IsSameAs(other);

            //assert.
            Assert.False(same);
        }

        [Fact]
        public void Clone_Creates_Similar_Instance()
        {
            //arrange.
            var sut = new DataTableArgument();

            //act.
            var clone = sut.Clone();

            //assert.
            Assert.True(clone.IsSameAs(sut));
            Assert.NotSame(clone, sut);
        }
    }
}
