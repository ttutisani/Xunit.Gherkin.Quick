[assembly:Xunit.Gherkin.Quick.FeatureFileSearchPattern("*.feature|*.txt|*.dat")]

namespace Xunit.Gherkin.Quick.ProjectConsumer
{
    [FeatureFileSearchPattern("*.feature|*.txt|*.dat")]
    public sealed class HandleNotImplemented : MissingFeature
    {
    }
}
