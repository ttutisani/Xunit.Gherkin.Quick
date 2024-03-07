using Gherkin.Ast;
using System;
using System.Linq;

namespace Xunit.Gherkin.Quick.ProjectConsumer.Placeholders
{
    [FeatureFile("./Placeholders/Placeholders.feature")]
    public sealed class Placeholder : Feature
    {
        private DataTable _tableData;
        private DocString _stringData;

        [Given("I have supplied a DataTable with")]
        public void I_Have_Supplied_DataTable(DataTable data)
        {
            _tableData = data;
        }

        [When("I execute a scenario outline")]
        public void I_Execute_A_Scenario_Outline()
        {
        }

        [Then(@"the DataTables MealExtra column should contain (\w+)")]
        public void The_DataTables_MealExtra_Column_Should_Contain(string fruit)
        {
            Assert.NotNull(_tableData);
            var dataRow = _tableData.Rows.Skip(1).FirstOrDefault();
            Assert.NotNull(dataRow);
            Assert.Equal(fruit, dataRow.Cells.FirstOrDefault()?.Value);
        }

        [Given("I have supplied a DocString with")]
        public void I_have_supplied_a_docstring_with(DocString data)
        {
            _stringData = data;
        }

        [Then(@"the DocString should contain a {} plan for {} for {} plus {} portions and {} extra, containing {}, an optional hot drink {} {} and a description for a smoothie of a {} and {} combo")]
        public void The_docstring_should_contain(
            string meal, string date, int myPortions, int otherPortions, int provision, string mainDish, string needHotDrink, string hotDrink, string fruit, string addition)
        {
            Assert.NotNull(_stringData);
            var dataContent = _stringData.Content;

            Assert.NotNull(dataContent);
            Assert.DoesNotContain('<', dataContent);
            Assert.DoesNotContain('>', dataContent);

            var expectedContent = $@"{date} Delicious {meal} plan:
The main dish for {date} is: {mainDish}!
Additionally prepare a {fruit}-based smoothie with some {addition}.
Optionally add a cup of hot {hotDrink}? {needHotDrink}.
Total portions needed: {myPortions}+{otherPortions}+{provision}! ";

            Assert.Equal(expectedContent, dataContent);
        }
    }
}
