using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick
{
    /// <summary>
    /// Base class for the <see cref="Feature"/> and <see cref="MissingFeature"/> classes. This class is used for internal infrastructure of the framework.
    /// If you are trying to create a feature class, you shoud derive from either of the mentioned classes, but not from this class directly.
    /// </summary>
    public abstract class FeatureBase
    {
        internal ITestOutputHelper InternalOutput { get; set; }
    }
}
