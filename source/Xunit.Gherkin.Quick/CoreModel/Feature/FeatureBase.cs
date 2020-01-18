using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick
{
    public abstract class FeatureBase
    {
        internal ITestOutputHelper InternalOutput { get; set; }
    }
}
