using System;

namespace Xunit.Gherkin.Quick
{
    /// <summary>
    /// Specifies feature file name search pattern. 
    /// This pattern will be used to find feature files which don't have corresponding feature classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class FeatureFileSearchPatternAttribute : Attribute
    {
        /// <summary>
        /// Initializes new instance of <see cref="FeatureFileSearchPatternAttribute"/>.
        /// </summary>
        /// <param name="pattern">Feature file name search pattern that will be used to 
        /// find feature files that don't have corresponding feature classes.</param>
        public FeatureFileSearchPatternAttribute(string pattern)
        {
            Pattern = !string.IsNullOrWhiteSpace(pattern)
                ? pattern
                : throw new ArgumentNullException(nameof(pattern));
        }

        internal string Pattern { get; }
    }
}
