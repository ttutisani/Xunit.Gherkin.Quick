using System.Collections.Generic;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class ScenarioStepPatternTests
    {
        [Fact]
        public void ListFromStepAttributes_Creates_List_From_Attributes()
        {
            //arrange.
            var attributes = new List<BaseStepDefinitionAttribute>
            {
                new GivenAttribute("pattern"),
                new WhenAttribute("other pattern"),
                new ThenAttribute("yet another pattern")
            };

            //act.
            var patterns = ScenarioStepPattern.ListFromStepAttributes(attributes);

            //assert.
            Assert.NotNull(patterns);
            Assert.Equal(attributes.Count, patterns.Count);
        }
    }
}
