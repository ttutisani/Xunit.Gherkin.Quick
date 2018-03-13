namespace Xunit.Gherkin.Quick.ProjectConsumer.Addition
{
    [FeatureFile("./Addition/AddTwoNumbers.feature")]
    public sealed class AddTwoNumbers : Feature
    {
        private readonly Calculator _calculator = new Calculator();

        [Given(@"I have entered (\d+) into the calculator")]
        public void I_have_entered_x_into_the_calculator(int firstNumber)
        {
            _calculator.SetFirstNumber(firstNumber);
        }

        [And(@"I have entered (\d+) into the calculator")]
        public void I_have_entered_y_into_the_calculator(int secondNumber)
        {
            _calculator.SetSecondNumber(secondNumber);
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
