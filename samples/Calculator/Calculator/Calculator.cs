namespace NetCoreTest
{
    public class Calculator
    {
        public int Result { get; private set; }
        private int firstNumber;
        private int secondNumber;
        
        public void SetFirstNumber(int firstNumber)
        {
            this.firstNumber = firstNumber;
        }

        public void SetSecondNumber(int secondNumber)
        {
            this.secondNumber = secondNumber;
        }

        public void AddNumbers()
        {
            this.Result = this.firstNumber + this.secondNumber;
        }

    }
}