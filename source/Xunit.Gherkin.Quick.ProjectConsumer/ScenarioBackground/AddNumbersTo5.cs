namespace Xunit.Gherkin.Quick.ProjectConsumer.Emoji
{
    [FeatureFile("./ScenarioBackground/AddNumbersTo5.em.feature")]
    public class AddNumbersTo5 : Addition.AddNumbersTo5 { }

}

namespace Xunit.Gherkin.Quick.ProjectConsumer.Addition
{
    [FeatureFile("./ScenarioBackground/AddNumbersTo5.feature")]
    public class AddNumbersTo5 : Feature
    {
        private readonly Calculator _calculator = new Calculator();

        [Given(@"I chose (-?\d+) as first number")]
        public void Given_I_chose_x_as_first_number(int firstNumber)
        {
            _calculator.SetFirstNumber(firstNumber);
        }

        [And(@"I chose (-?\d+) as second number")]
        public void And_I_chose_y_as_second_number(int secondNumber)
        {
            _calculator.SetSecondNumber(secondNumber);
        }

        [When(@"I press add")]
        public void When_I_press_add()
        {
            _calculator.AddNumbers();
        }

        [Then(@"the result should be (-?\d+) on the screen")]
        public void Then_the_result_should_be_z_on_the_screen(int expectedResult)
        {
            var actualResult = _calculator.Result;

            Assert.Equal(expectedResult, actualResult);
        }
    }
}
