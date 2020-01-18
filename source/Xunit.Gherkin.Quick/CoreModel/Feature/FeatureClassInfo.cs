using System;
using System.Reflection;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureClassInfo
    {
        public string FeatureFilePath { get; }

        private FeatureClassInfo(string featureFilePath)
        {
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

            return new FeatureClassInfo(featureFilePath);
        }
    }
}
