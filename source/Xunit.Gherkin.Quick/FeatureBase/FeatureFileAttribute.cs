using System;

namespace Xunit.Gherkin.Quick
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class FeatureFileAttribute : Attribute
    {
        /// <summary>
        /// Specify path to feature file (can be relative to output folder).
        /// </summary>
        /// <param name="path">
        /// Path to feature file. Can be relative to output folder,
        /// e.g. "./SampleFolder/Sample.feature"
        /// </param>
        public FeatureFileAttribute(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public string Path { get; }
    }
}
