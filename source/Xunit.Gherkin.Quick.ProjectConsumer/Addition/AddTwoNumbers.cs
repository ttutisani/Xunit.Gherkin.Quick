using System;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.ProjectConsumer.Addition
{
    [FeatureFile("./Addition/AddTwoNumbers.feature")]
    public sealed class AddTwoNumbers : Feature
    {
        private readonly Calculator _calculator = new Calculator();

        public AddTwoNumbers(ITestOutputHelper output) : base(output)
        {}

        [GivenAttribute(@"I chose (\d+) as first number")]
        public void I_chose_first_number(int firstNumber)
        {
            _calculator.SetFirstNumber(firstNumber);
        }

        [AndAttribute(@"I chose (\d+) as second number")]
        public void I_chose_second_number(int secondNumber)
        {
            _calculator.SetSecondNumber(secondNumber);
        }

        [WhenAttribute(@"I press add")]
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
