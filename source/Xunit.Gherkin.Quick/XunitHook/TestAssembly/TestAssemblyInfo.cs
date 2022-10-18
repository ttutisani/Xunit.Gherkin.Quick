using System;
using System.Reflection;

namespace Xunit.Gherkin.Quick
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

        internal static TestAssemblyInfo FromAssembly(Assembly assembly)
        {
            var patternAttribute = assembly.GetCustomAttribute<FeatureFileSearchPatternAttribute>();
            var searchPattern = patternAttribute?.Pattern ?? "*.feature";

            var assemblyInfo = new TestAssemblyInfo(searchPattern);
            return assemblyInfo;
        }
    }
}
