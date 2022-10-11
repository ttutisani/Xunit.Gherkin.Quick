namespace Xunit.Gherkin.Quick.ProjectConsumer.FeatureFilePattern
{
    [FeatureFile(@".*Pattern/Base.*\.feature", FeatureFilePathType.Regex)]
    public sealed class BaseFolderOnly : Feature
    {
        [Given("I have a base folder")]
        public void Given_I_have_base_folder()
        {

        }
    }
}
