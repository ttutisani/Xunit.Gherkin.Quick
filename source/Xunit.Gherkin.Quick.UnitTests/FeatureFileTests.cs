using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class FeatureFileTests
    {
        [Fact]
        public void Ctor_Initializes_Properties()
        {
            //arrange.
            var gherkinDocument = new Gherkin.Ast.GherkinDocument(null, null);

            //act.
            var sut = new FeatureFile(gherkinDocument);

            //assert.
            Assert.Same(gherkinDocument, sut.GherkinDocument);
        }
    }
}
