using Gherkin.Ast;
using System.Linq;

namespace Xunit.Gherkin.Quick.ProjectConsumer.Addition
{
    public partial class AddTwoNumbers
    {
        [Given(@"following table of (\d+) inputs and outputs:")]
        public void Following_table_of_inputs_and_outputs(int inputCount, DataTable dataTable)
        {
            Assert.Equal(inputCount, dataTable.Rows.Count() - 1);

            foreach (var row in dataTable.Rows.Skip(1))
            {
                //arrange.
                var calculator = new Calculator();
                calculator.SetFirstNumber(int.Parse(row.Cells.ElementAt(0).Value));
                calculator.SetSecondNumber(int.Parse(row.Cells.ElementAt(1).Value));

                //act.
                calculator.AddNumbers();

                //assert.
                Assert.Equal(int.Parse(row.Cells.ElementAt(2).Value), calculator.Result);
            }
        }
    }
}
