using System;
using System.Reflection;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureClassInfo
    {
        public string FeatureFilePath { get; }

        public string FileNameSearchPattern { get; }

        private FeatureClassInfo(string featureFilePath, string fileNameSearchPattern)
        {
            FileNameSearchPattern = !string.IsNullOrWhiteSpace(fileNameSearchPattern)
                ? fileNameSearchPattern 
                : throw new System.ArgumentNullException(nameof(fileNameSearchPattern));

            FeatureFilePath = !string.IsNullOrWhiteSpace(featureFilePath)
                ? featureFilePath
                : throw new ArgumentNullException(nameof(featureFilePath));
        }

        public static FeatureClassInfo FromFeatureClassType(Type featureClassType)
        {
            var featureFileAttribute = featureClassType
                .GetTypeInfo()
                .GetCustomAttribute<FeatureFileAttribute>();
            var featureFilePath = featureFileAttribute?.Path ?? $"{featureClassType.Name}.feature";

            var featureFileNameSearchPatternAttribute = featureClassType
                .GetTypeInfo()
                .GetCustomAttribute<FeatureFileSearchPatternAttribute>();
            var featureFileNameSearchPattern = featureFileNameSearchPatternAttribute?.Pattern ?? $"{featureClassType.Name}*.feature";

            return new FeatureClassInfo(featureFilePath, featureFileNameSearchPattern);
        }
    }
}
