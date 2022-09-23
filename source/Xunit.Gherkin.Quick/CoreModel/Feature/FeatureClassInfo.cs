using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureClassInfo
    {
        public string FeatureFilePath { get; }
        private Regex _matcher;
        public bool IsPattern { get { return _matcher != null; } }

        private FeatureClassInfo(string featureFilePath)
        {
            FeatureFilePath = !string.IsNullOrWhiteSpace(featureFilePath)
                ? featureFilePath
                : throw new ArgumentNullException(nameof(featureFilePath));

            if (FeatureFilePath.Contains("*")) {
                var regex = FeatureFilePath.Replace("*", @"(\w|\/\s)*");
                this._matcher = new Regex(regex,
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }


        }

        public static FeatureClassInfo FromFeatureClassType(Type featureClassType)
        {
            var featureFileAttribute = featureClassType
                .GetTypeInfo()
                .GetCustomAttribute<FeatureFileAttribute>();
            var featureFilePath = featureFileAttribute?.Path ?? $"{featureClassType.Name}.feature";

            return new FeatureClassInfo(featureFilePath);
        }

        public bool MatchesFilePathPattern(string file) {
            if (IsPattern) 
                return _matcher.IsMatch(file);
            return FeatureFilePath.Equals(file);
        }
    }
}
