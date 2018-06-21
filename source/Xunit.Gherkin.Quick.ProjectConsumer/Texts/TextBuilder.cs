using Gherkin.Ast;
using System.Text;

namespace Xunit.Gherkin.Quick.ProjectConsumer.Texts
{
    [FeatureFile("./Texts/TextBuilder.feature")]
    public sealed class TextBuilder : Feature
    {
        private StringBuilder _textBuilder;

        [Given("I have a text like this:")]
        public void I_have_a_text_like_this(DocString text)
        {
            _textBuilder = new StringBuilder(text.Content);
        }

        [When(@"I replace '(\w+)' with '(\w+)'")]
        public void I_replace_x_with_y(string oldValue, string newValue)
        {
            _textBuilder.Replace(oldValue, newValue);
        }

        [Then("I should have text like this:")]
        public void I_should_have_text_like_this(DocString text)
        {
            Assert.Equal(text.Content, _textBuilder.ToString());
        }
    }
}
