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
        public void Given_I_Have_Supplied_DataTable(DataTable data)
        {
            _tableData = data;
        }

        [When("I execute a scenario outline")]
        public void When_I_Execute_A_Scenario_Outline()
        {
        }

        [Then(@"the DataTables MealExtra column should contain (\w+)")]
        public void Then_the_DataTables_MealExtra_Column_Should_Contain(string fruit)
        {
            Assert.NotNull(_tableData);
            var dataRow = _tableData.Rows.Skip(1).FirstOrDefault();
            Assert.NotNull(dataRow);
            Assert.Equal(fruit, dataRow.Cells.FirstOrDefault()?.Value);
        }

        [Given("I have supplied a DocString with")]
        public void Given_I_Have_Supplied_A_DocString_With(DocString data)
        {
            _stringData = data;
        }

        [Then(@"the DocString should contain a {} plan for {} for {} plus {} portions and {} extra, containing {}, an optional hot drink {} {} and a description for a smoothie of a {} and {} combo")]
        public void Then_The_DocString_Should_Contain(
            string meal, string date, int myPortions, int otherPortions, int provision, string mainDish, string needHotDrink, string hotDrink, string fruit, string addition)
        {
            Assert.NotNull(_stringData);
            var dataContent = _stringData.Content;

            Assert.NotNull(dataContent);
            Assert.DoesNotContain('<', dataContent);
            Assert.DoesNotContain('>', dataContent);

            var expectedContent = $@"{date} Delicious {meal} plan:{Environment.NewLine}The main dish for {date} is: {mainDish}!{Environment.NewLine}Additionally prepare a {fruit}-based smoothie with some {addition}.{Environment.NewLine}Optionally add a cup of hot {hotDrink}? {needHotDrink}.{Environment.NewLine}Total portions needed: {myPortions}+{otherPortions}+{provision}!";

            Assert.Equal(expectedContent, dataContent);
        }
    }
}
