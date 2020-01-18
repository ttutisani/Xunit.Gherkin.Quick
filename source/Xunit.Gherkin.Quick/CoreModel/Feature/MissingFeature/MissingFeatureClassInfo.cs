using System;
using System.Reflection;

namespace Xunit.Gherkin.Quick
{
    internal sealed class MissingFeatureClassInfo
    {
        private MissingFeatureClassInfo(string fileNameSearchPattern)
        {
            FileNameSearchPattern = !string.IsNullOrWhiteSpace(fileNameSearchPattern)
                ? fileNameSearchPattern 
                : throw new System.ArgumentNullException(nameof(fileNameSearchPattern));
        }

        public string FileNameSearchPattern { get; }

        public static MissingFeatureClassInfo FromMissingFeatureClassType(Type missingFeatureClassType)
        {
            if (missingFeatureClassType is null)
                throw new ArgumentNullException(nameof(missingFeatureClassType));

            var featureFileNameSearchPatternAttribute = missingFeatureClassType
                .GetTypeInfo()
                .GetCustomAttribute<FeatureFileSearchPatternAttribute>();
            var featureFileNameSearchPattern = featureFileNameSearchPatternAttribute?.Pattern ?? $"*.feature";

            return new MissingFeatureClassInfo(featureFileNameSearchPattern);
        }
    }
}
