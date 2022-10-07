namespace Xunit.Gherkin.Quick.ProjectConsumer.FeatureFilePattern
{
    [FeatureFile("*Pattern/(Nested*/)?*.feature")]
    public sealed class BaseAndNestedFolders : Feature
    {
        [Given("I have a base folder")]
        public void Given_I_have_base_folder()
        {

        }

        [Given("I have a nested folder")]
        public void Given_I_have_nested_folder()
        {

        }
    }
}
