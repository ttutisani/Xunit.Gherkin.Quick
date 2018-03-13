namespace Xunit.Gherkin.Quick.ProjectConsumer
{
    public sealed class Calculator
    {
        public int FirstNumber { get; private set; }
        public int SecondNumber { get; private set; }
        public int Result { get; private set; }

        public void SetFirstNumber(int number) => FirstNumber = number;

        public void SetSecondNumber(int number) => SecondNumber = number;

        public void AddNumbers() => Result = FirstNumber + SecondNumber;
    }
}
