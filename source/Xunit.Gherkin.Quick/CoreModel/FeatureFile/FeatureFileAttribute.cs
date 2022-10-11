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
        /// <param name="pathType">Indicates the kind of the provided file path.
        /// Default is Simple (i.e., the path represents a concrete file path).</param>
        public FeatureFileAttribute(string path, FeatureFilePathType pathType = FeatureFilePathType.Simple)
        {
            Path = !string.IsNullOrWhiteSpace(path) 
                ? path 
                : throw new ArgumentNullException(nameof(path));

            PathType = pathType;
        }

        internal string Path { get; }

        internal FeatureFilePathType PathType { get; }
    }

    /// <summary>
    /// All possible kinds of feature file path.
    /// </summary>
    public enum FeatureFilePathType
    {
        /// <summary>
        /// Denotes the default, simple path type, that is the path to a single file.
        /// </summary>
        Simple,

        /// <summary>
        /// Denotes the path that contains regex, and thus can match one or more files based on the provided pattern.
        /// </summary>
        Regex
    }
}
