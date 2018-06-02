using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class PrimitiveTypeArgumentTests
    {
        [Fact]
        public void Clone_Creates_Similar_Instance()
        {
            //arrange.
            var sut = new PrimitiveTypeArgument();

            //act.
            var clone = sut.Clone();

            //assert.
            Assert.True(clone.IsSameAs(sut));
        }
    }
}
