using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xunit.Gherkin.Quick
{
    internal abstract class FeatureFilePathInfo
    {
        internal sealed class FeatureAndPath
        {
            public string Path { get; }
            public global::Gherkin.Ast.Feature Feature { get; }

            internal FeatureAndPath(string path, global::Gherkin.Ast.Feature feature)
            {
                Path = path ?? throw new ArgumentNullException(nameof(path));
                Feature = feature ?? throw new ArgumentNullException(nameof(feature));
            }
        }

        public abstract bool MatchesPath(string path);

        public abstract List<FeatureAndPath> GetMatchingFeatures(IFeatureFileRepository featureFileRepository);
    }

    internal sealed class SimpleFeatureFilePathInfo : FeatureFilePathInfo
    {
        private readonly string _featureFilePath;

        public SimpleFeatureFilePathInfo(string featureFilePath)
        {
            _featureFilePath = featureFilePath;
        }

        public override List<FeatureAndPath> GetMatchingFeatures(IFeatureFileRepository featureFileRepository)
        {
            var featureFile = featureFileRepository.GetByFilePath(_featureFilePath);
            if (featureFile == null) return new List<FeatureAndPath>();

            var featureAndPath = new FeatureAndPath(_featureFilePath, featureFile.GherkinDocument.Feature);

            return new List<FeatureAndPath> { featureAndPath };
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

        public override List<FeatureAndPath> GetMatchingFeatures(IFeatureFileRepository featureFileRepository)
        {
            var allFeatureFilePaths = featureFileRepository
                .GetFeatureFilePaths().ToList();

            var result = new List<FeatureAndPath>();

            foreach (var featureFilePath in allFeatureFilePaths)
            {
                if (!Regex.IsMatch(featureFilePath, _pathRegex)) continue;

                var featureFile = featureFileRepository.GetByFilePath(featureFilePath);
                var featureAndPath = new FeatureAndPath(featureFilePath, featureFile.GherkinDocument.Feature);

                result.Add(featureAndPath);
            }

            return result;
        }

        public override bool MatchesPath(string path)
        {
            var match = Regex.IsMatch(path, _pathRegex);
            return match;
        }
    }
}
