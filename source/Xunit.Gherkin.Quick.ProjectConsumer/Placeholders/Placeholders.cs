using Gherkin.Ast;
using System.Linq;

namespace Xunit.Gherkin.Quick.ProjectConsumer.Placeholders
{
    [FeatureFile("./Placeholders/Placeholders.feature")]
    public sealed class Placeholder : Feature
    {
        private DataTable _data;

        [Given("I have supplied a DataTable with")]
        public void Given_I_Have_Supplied_DataTable(DataTable data)
        {
            _data = data;
        }

        [When("I execute a scenario outline")]
        public void When_I_Execute_A_Scenario_Outline()
        {
        }

        [Then(@"the DataTables MealExtra column should contain (\w+)")]
        public void Then_The_DataTables_MealExtra_Column_Should_Contain(string fruit)
        {
            Assert.NotNull(_data);
            var dataRow = _data.Rows.Skip(1).FirstOrDefault();
            Assert.NotNull(dataRow);
            Assert.Equal(fruit, dataRow.Cells.FirstOrDefault()?.Value);
        }
    }
}
