using System.Threading.Tasks;

namespace Xunit.Gherkin.Quick.ProjectConsumer.Addition.Async
{
    [FeatureFile("./Async/AddTwoNumbersAsync.feature")]
    public sealed class AddTwoNumbersAsync : Feature
    {
        private readonly AsyncCalculator _calculator = new AsyncCalculator();

        [Given(@"I chose (\d+) as first number")]
        public void I_chose_first_number(int firstNumber)
        {
            _calculator.SetFirstNumber(firstNumber);
        }

        [And(@"I chose (\d+) as second number")]
        public void I_chose_second_number(int secondNumber)
        {
            _calculator.SetSecondNumber(secondNumber);
        }

        [When(@"I press add")]
        public async Task I_press_add()
        {
            await _calculator.AddNumbersAsync();
        }

        [Then(@"the result should be (\d+) on the screen")]
        public void The_result_should_be_z_on_the_screen(int expectedResult)
        {
            var actualResult = _calculator.Result;

            Assert.Equal(expectedResult, actualResult);
        }
    }
}
