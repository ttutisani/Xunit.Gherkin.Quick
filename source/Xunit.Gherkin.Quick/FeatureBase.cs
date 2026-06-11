using System;

namespace Xunit.Gherkin.Quick
{
    /// <summary>
    /// Base class for the <see cref="Feature"/> and <see cref="MissingFeature"/> classes. This class is used for internal infrastructure of the framework.
    /// If you are trying to create a feature class, you shoud derive from either of the mentioned classes, but not from this class directly.
    /// </summary>
    [Obsolete("The Feature class now handles all feature look ups and execution, this class will be removed with a future major release.")]
    public abstract class FeatureBase
    {
    }
}