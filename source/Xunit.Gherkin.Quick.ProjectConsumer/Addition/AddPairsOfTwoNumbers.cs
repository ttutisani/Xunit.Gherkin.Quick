using Gherkin.Ast;
using System.Linq;

namespace Xunit.Gherkin.Quick.ProjectConsumer.Addition
{
    public sealed partial class AddTwoNumbers
    {
        [Given("following table of inputs and outputs:")]
        public void Following_table_of_inputs_and_outputs(DataTable dataTable)
        {
            foreach (var row in dataTable.Rows)
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
