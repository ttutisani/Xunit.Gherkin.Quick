using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class StepMethodTests
    {
        [Theory]
        [InlineData(StepMethodKind.Given)]
        [InlineData(StepMethodKind.When)]
        [InlineData(StepMethodKind.Then)]
        [InlineData(StepMethodKind.And)]
        [InlineData(StepMethodKind.But)]
        internal void Ctor_Initializes_Properties(StepMethodKind kind)
        {
            //arrange.
            var text = "stpe method text 123";

            //act.
            var sut = new StepMethod(kind, text);

            //assert.
            Assert.Equal(kind, sut.Kind);
            Assert.Equal(text, sut.Text);
        }
    }
}
