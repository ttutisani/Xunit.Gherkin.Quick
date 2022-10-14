[assembly:Xunit.Gherkin.Quick.FeatureFileSearchPattern("*.feature")]

namespace Xunit.Gherkin.Quick.ProjectConsumer
{
    [FeatureFileSearchPattern("*.feature|*.txt|*.dat")]
    public sealed class HandleNotImplemented : MissingFeature
    {
    }
}
