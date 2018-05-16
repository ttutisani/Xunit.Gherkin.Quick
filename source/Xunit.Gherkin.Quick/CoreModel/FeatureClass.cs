using System;
using System.Reflection;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureClass
    {
        public FeatureClass(string featureFilePath)
        {
            FeatureFilePath = !string.IsNullOrWhiteSpace(featureFilePath) 
                ? featureFilePath 
                : throw new ArgumentNullException(nameof(featureFilePath));
        }

        public string FeatureFilePath { get; }

        //TODO: maybe we only need a feature type and not the whole instance?
        public static FeatureClass FromFeatureInstance(Feature featureInstance)
        {
            if (featureInstance == null)
                throw new ArgumentNullException(nameof(featureInstance));

            Type featureType = featureInstance.GetType();

            var featureFileAttribute = featureType
                .GetTypeInfo()
                .GetCustomAttribute<FeatureFileAttribute>();
            var featureFilePath = featureFileAttribute?.Path ?? $"{featureType.Name}.feature";

            //TODO: also generate step methods on the feature class?

            return new FeatureClass(featureFilePath);
        }
    }
}
