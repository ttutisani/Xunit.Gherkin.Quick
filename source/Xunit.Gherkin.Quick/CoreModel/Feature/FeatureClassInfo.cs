using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureClassInfo
    {
        public FeatureFilePathInfo PathInfo { get; }
        
        private FeatureClassInfo(FeatureFilePathInfo pathInfo)
        {
            PathInfo = pathInfo ?? throw new ArgumentNullException(nameof(pathInfo));
        }

        public static FeatureClassInfo FromFeatureClassType(Type featureClassType)
        {
            var featureFileAttribute = featureClassType
                .GetTypeInfo()
                .GetCustomAttribute<FeatureFileAttribute>();
            
            var featureFilePath = featureFileAttribute?.Path ?? $"{featureClassType.Name}.feature";
            var pathType = featureFileAttribute?.PathType ?? FeatureFilePathType.Simple;

            var pathInfo = pathType == FeatureFilePathType.Simple
                ? new SimpleFeatureFilePathInfo(featureFilePath) as FeatureFilePathInfo
                : pathType == FeatureFilePathType.Regex
                ? new RegexFeatureFilePathInfo(featureFilePath)
                : throw new NotSupportedException($"Path type `{pathType}` is not supported.");

            return new FeatureClassInfo(pathInfo);
        }

    }
}
