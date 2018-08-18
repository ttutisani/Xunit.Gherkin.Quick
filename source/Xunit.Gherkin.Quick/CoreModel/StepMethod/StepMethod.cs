using System;
using System.Threading.Tasks;

namespace Xunit.Gherkin.Quick
{
    internal sealed class StepMethod
    {
        private readonly StepMethodInfo _stepMethodInfo;

        public string StepText { get; }

        public StepMethodKind Kind { get; }

        public StepMethod(StepMethodInfo stepMethodInfo, string stepText)
        {
            _stepMethodInfo = stepMethodInfo ?? throw new ArgumentNullException(nameof(stepMethodInfo));
            StepText = !string.IsNullOrWhiteSpace(stepText)
                ? stepText
                : throw new ArgumentNullException(nameof(stepText));

            //Kind = stepMethodInfo.Kind;
        }

        public async Task ExecuteAsync()
        {
            await _stepMethodInfo.ExecuteAsync();
        }
    }
}
