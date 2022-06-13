namespace Xunit.Gherkin.Quick.ProjectConsumer
{
    [FeatureFile("./GivenWhenThenTests/InAnotherLanguage.feature")]
    public sealed class InAnotherLanguage : Feature
    {
        private int _order = 0;



        [Given(@"Monster tekst voor Stel")]
        public void Sample_text_for_Given()
        {
            Assert.Equal(0, _order++);
        }

        [And(@"Monster tekst voor En na Stel")]
        public void Sample_text_for_And_after_Given()
        {
            Assert.Equal(1, _order++);
        }

        [But(@"Monster tekst voor Maar na Stel")]
        public void Sample_text_for_But_after_Given()
        {
            Assert.Equal(2, _order++);
        }

        [When(@"Monster tekst voor Als")]
        public void Sample_text_for_When()
        {
            Assert.Equal(3, _order++);
        }

        [And(@"Monster tekst voor En na Als")]
        public void Sample_text_for_And_after_When()
        {
            Assert.Equal(4, _order++);
        }

        [But(@"Monster tekst voor Maar na Als")]
        public void Sample_text_for_But_after_When()
        {
            Assert.Equal(5, _order++);
        }

        [Then(@"Monster tekst voor Dan")]
        public void Sample_text_for_Then()
        {
            Assert.Equal(6, _order++);
        }

        [And(@"Monster tekst voor En na Dan")]
        public void Sample_text_for_And_after_Then()
        {
            Assert.Equal(7, _order++);
        }

        [But(@"Monster tekst voor Maar na Dan")]
        public void Sample_text_for_But_after_Then()
        {
            Assert.Equal(8, _order++);
        }
    }
}