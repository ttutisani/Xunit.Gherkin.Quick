using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Xunit.Gherkin.Quick
{
    internal abstract class FeatureFilePathInfo
    {
        public abstract bool MatchesPath(string path);
    }

    internal sealed class SimpleFeatureFilePathInfo : FeatureFilePathInfo
    {
        private readonly string _featureFilePath;

        public SimpleFeatureFilePathInfo(string featureFilePath)
        {
            _featureFilePath = featureFilePath;
        }

        public override bool MatchesPath(string path)
        {
            var match = string.Equals(_featureFilePath, path, StringComparison.OrdinalIgnoreCase);
            return match;
        }
    }

    internal sealed class RegexFeatureFilePathInfo : FeatureFilePathInfo
    {
        private readonly string _pathRegex;

        public RegexFeatureFilePathInfo(string pathRegex)
        {
            _pathRegex = pathRegex;
        }

        public override bool MatchesPath(string path)
        {
            var match = Regex.IsMatch(path, _pathRegex);
            return match;
        }
    }
}
