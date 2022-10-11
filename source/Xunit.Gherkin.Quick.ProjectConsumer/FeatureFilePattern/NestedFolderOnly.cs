namespace Xunit.Gherkin.Quick.ProjectConsumer.FeatureFilePattern
{
    [FeatureFile(@".*Pattern/Nested.*/.*\.feature", FeatureFilePathType.Regex)]
    public sealed class NestedFolderOnly : Feature
    {
        [Given("I have a nested folder")]
        public void Given_I_have_nested_folder()
        {

        }
    }
}
