using System;
using System.Threading.Tasks;

namespace Xunit.Gherkin.Quick
{
    internal sealed class StepMethod
    {
        public StepMethodInfo StepMethodInfo { get; }

        public string StepText { get; }

        public StepMethod(StepMethodInfo stepMethodInfo, string stepText)
        {
            StepMethodInfo = stepMethodInfo ?? throw new ArgumentNullException(nameof(stepMethodInfo));
            StepText = !string.IsNullOrWhiteSpace(stepText)
                ? stepText
                : throw new ArgumentNullException(nameof(stepText));
        }

        public async Task ExecuteAsync()
        {
            await StepMethodInfo.ExecuteAsync();
        }
    }
}
