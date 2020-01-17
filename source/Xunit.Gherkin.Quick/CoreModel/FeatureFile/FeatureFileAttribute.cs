using System;

namespace Xunit.Gherkin.Quick
{
    /// <summary>
    /// Allows associating a feature text file with feature class.
    /// If the feature text file is located in the root folder and 
    /// it's named as &lt;FeatureClass&gt;.feature, then you can
    /// omit this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class FeatureFileAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FeatureFileAttribute"/> class.
        /// </summary>
        /// <param name="path">
        /// Path to feature file. Must be relative to output folder,
        /// e.g. "./SampleFolder/Sample.feature"
        /// </param>
        public FeatureFileAttribute(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        /// <summary>
        /// Gets path to feature file.
        /// </summary>
        public string Path { get; }
    }
}
