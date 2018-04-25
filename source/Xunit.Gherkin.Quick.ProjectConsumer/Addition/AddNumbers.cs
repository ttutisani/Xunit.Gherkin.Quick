using System.Linq;

namespace Xunit.Gherkin.Quick.ProjectConsumer.Addition
{
    [FeatureFile("./Addition/AddNumbers.feature")]
    public sealed class AddTwoNumbers : Feature
    {
        private readonly Calculator _calculator = new Calculator();

        [Given(@"I chose (\d+) as first number")]
        public void I_chose_first_number(int firstNumber)
        {
            _calculator.Numbers.Add(firstNumber);
        }

        [Given(@"I have chosen the following table of numbers")]
        public void GivenIHaveChosenTheFollowingTableOfNumbers(Table table)
        {
            var numbers = table.Rows.ToList()[1].Cells.ToList();
            numbers.ForEach(num =>
            {
                _calculator.Numbers.Add(int.Parse(num.Value));
            });
        }

        [Given(@"I have chosen the following list of numbers")]
        public void GivenIHaveChosenTheFollowingListOfNumbers(string multilineText)
        {
            var numbers = multilineText.Split("\r\n").ToList();
            numbers.ForEach(num =>
            {
                _calculator.Numbers.Add(int.Parse(num));
            });
        }

[And(@"I chose (\d+) as second number")]
        public void I_chose_second_number(int secondNumber)
        {
            _calculator.Numbers.Add(secondNumber);
        }

        [When(@"I press add")]
        public void I_press_add()
        {
            _calculator.AddNumbers();
        }

        [Then(@"the result should be (\d+) on the screen")]
        public void The_result_should_be_z_on_the_screen(int expectedResult)
        {
            var actualResult = _calculator.Result;

            Assert.Equal(expectedResult, actualResult);
        }
    }
}
