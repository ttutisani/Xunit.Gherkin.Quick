using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureDiscoveryModel
    {
        internal sealed class FeatureFile {
            public string Path { get; }
            public global::Gherkin.Ast.Feature Feature { get; }

            internal FeatureFile(string path ,global::Gherkin.Ast.Feature feature) {
                this.Path = path;
                this.Feature = feature;                
            }
        }

        private readonly IFeatureFileRepository _featureFileRepository;

        public FeatureDiscoveryModel(IFeatureFileRepository featureFileRepository)
        {
            _featureFileRepository = featureFileRepository ?? throw new ArgumentNullException(nameof(featureFileRepository));
        }

        public List<FeatureFile> Discover(Type featureClassType)
        {
            if (featureClassType == null)
                throw new ArgumentNullException(nameof(featureClassType));

            var fileClassInfo = FeatureClassInfo.FromFeatureClassType(featureClassType);

            var featurePathsAndFiles = new Dictionary<string, Quick.FeatureFile>();

            var allFeatureFilePaths = _featureFileRepository
                .GetFeatureFilePaths().ToList();

            foreach (var featureFilePath in allFeatureFilePaths)
            {
                if (!fileClassInfo.PathInfo.MatchesPath(featureFilePath)) continue;

                featurePathsAndFiles.Add(featureFilePath, _featureFileRepository.GetByFilePath(featureFilePath));
            }

            if (featurePathsAndFiles.Count == 0) {
                throw new FileNotFoundException($"No feature file found for class `${featureClassType.Name}`.");
            }

            return featurePathsAndFiles
                .Select(f => new FeatureFile(f.Key, f.Value.GherkinDocument.Feature))
                .ToList();;
        }
    }
}
