using System.Threading;
using System.Threading.Tasks;

namespace Xunit.Gherkin.Quick.ProjectConsumer.Addition.Async
{
    public sealed class AsyncCalculator
    {
        private int _firstNumber;
        private int _secondNumber;
        public int Result { get; private set; }

        public void SetFirstNumber(int firstNumber)
        {
            _firstNumber = firstNumber;
        }

        public void SetSecondNumber(int secondNumber)
        {
            _secondNumber = secondNumber;
        }

        public async Task AddNumbersAsync()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(1000); //intentional delay - to imitate truly async operation.
                Result = _firstNumber + _secondNumber;
            });
        }
    }
}
