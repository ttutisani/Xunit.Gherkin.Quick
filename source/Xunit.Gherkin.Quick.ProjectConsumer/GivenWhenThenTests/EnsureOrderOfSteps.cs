namespace Xunit.Gherkin.Quick.UnitTests
{
    [FeatureFile("./GivenWhenThenTests/EnsureOrderOfSteps.feature")]
    public sealed class EnsureOrderOfSteps : Feature
    {
        private int _order = 0;

        [Given(@"Sample text for Given")]
        public void Sample_text_for_Given()
        {
            Assert.Equal(0, _order++);
        }

        [And(@"Sample text for And after Given")]
        public void Sample_text_for_And_after_Given()
        {
            Assert.Equal(1, _order++);
        }

        [But(@"Sample text for But after Given")]
        public void Sample_text_for_But_after_Given()
        {
            Assert.Equal(2, _order++);
        }

        [When(@"Sample text for When")]
        public void Sample_text_for_When()
        {
            Assert.Equal(3, _order++);
        }

        [And(@"Sample text for And after When")]
        public void Sample_text_for_And_after_When()
        {
            Assert.Equal(4, _order++);
        }

        [But(@"Sample text for But after When")]
        public void Sample_text_for_But_after_When()
        {
            Assert.Equal(5, _order++);
        }

        [Then(@"Sample text for Then")]
        public void Sample_text_for_Then()
        {
            Assert.Equal(6, _order++);
        }

        [And(@"Sample text for And after Then")]
        public void Sample_text_for_And_after_Then()
        {
            Assert.Equal(7, _order++);
        }

        [But(@"Sample text for But after Then")]
        public void Sample_text_for_But_after_Then()
        {
            Assert.Equal(8, _order++);
        }
    }
}
