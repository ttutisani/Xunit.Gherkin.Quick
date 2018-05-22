using System.Collections.Generic;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class ScenarioTests
    {
        [Fact]
        public void Ctor_Initializes_Properties()
        {
            //arrange.
            var stepMethods = new List<StepMethod>
            {
                new StepMethod(StepMethodKind.Then, "some text 1", new List<StepMethodArgument>()),
                new StepMethod(StepMethodKind.When, "some text 2", new List<StepMethodArgument>()),
                new StepMethod(StepMethodKind.Given, "some text 3", new List<StepMethodArgument>())
            };

            //act.
            var sut = new Scenario(stepMethods);

            //assert.
            Assert.NotNull(sut.Steps);
            Assert.Equal(stepMethods, sut.Steps);
        }
    }
}
