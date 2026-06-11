using System;
using System.Reflection;

namespace Xunit.Gherkin.Quick.Hooks
{
    internal sealed class TestAssemblyInfo
    {
        public string FeatureFileSearchPattern { get; }

        public TestAssemblyInfo(string featureFileSearchPattern)
        {
            FeatureFileSearchPattern = !string.IsNullOrWhiteSpace(featureFileSearchPattern)
                ? featureFileSearchPattern
                : throw new ArgumentNullException(nameof(featureFileSearchPattern));
        }

        [Obsolete("File lookups include all file extensions, this class will be removed with a future major release.")]
        internal static TestAssemblyInfo FromAssembly(Assembly assembly)
        {
            var patternAttribute = assembly.GetCustomAttribute<FeatureFileSearchPatternAttribute>();
            var searchPattern = patternAttribute?.Pattern ?? "*.feature";

            var assemblyInfo = new TestAssemblyInfo(searchPattern);
            return assemblyInfo;
        }
    }
}