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

            var featurePathsAndFiles = new Dictionary<string,Gherkin.Quick.FeatureFile>();
                
            if (fileClassInfo.IsPattern) {
                _featureFileRepository
                    .GetFeatureFilePaths()
                    .FindAll(f => fileClassInfo.MatchesFilePathPattern(f))
                    .ForEach( f=> featurePathsAndFiles.Add(f, _featureFileRepository.GetByFilePath(f)));
                if (featurePathsAndFiles.Count == 0) {
                    throw new System.IO.FileNotFoundException($"No features found for pattern ${fileClassInfo.FeatureFilePath}");
                }
            } else {
                var fileName = fileClassInfo.FeatureFilePath;
                var featureFile = _featureFileRepository.GetByFilePath(fileName);
                if (featureFile == null)
                    throw new System.IO.FileNotFoundException("Feature file not found.", fileName);

                featurePathsAndFiles.Add(fileName, featureFile);
            }

            return featurePathsAndFiles
                .Select(f => new FeatureFile(f.Key, f.Value.GherkinDocument.Feature))
                .ToList();;
        }
    }
}
