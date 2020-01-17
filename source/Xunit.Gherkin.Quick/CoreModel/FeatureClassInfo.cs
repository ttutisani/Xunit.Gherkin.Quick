using System;
using System.Reflection;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureClassInfo
    {
        public string FeatureFilePath { get; }

        public FeatureClassInfo(string featureFilePath)
        {
            FeatureFilePath = featureFilePath;
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
