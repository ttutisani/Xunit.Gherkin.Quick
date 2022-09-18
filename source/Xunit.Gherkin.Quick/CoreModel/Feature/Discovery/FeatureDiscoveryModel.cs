using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureDiscoveryModel
    {
        private readonly IFeatureFileRepository _featureFileRepository;

        public FeatureDiscoveryModel(IFeatureFileRepository featureFileRepository)
        {
            _featureFileRepository = featureFileRepository ?? throw new ArgumentNullException(nameof(featureFileRepository));
        }

        public IEnumerable<Tuple<string,global::Gherkin.Ast.Feature>> Discover(Type featureClassType)
        {
            if (featureClassType == null)
                throw new ArgumentNullException(nameof(featureClassType));

            var featureClassInfo = FeatureClassInfo.FromFeatureClassType(featureClassType);
            var fileNameSearchPattern = featureClassInfo.FileNameSearchPattern;
            var fileName = featureClassInfo.FeatureFilePath;
            var featureFile = _featureFileRepository.GetByFilePath(fileName);
          
            var allFiles = _featureFileRepository.GetFeatureFilePaths();
            if (featureFile != null) 
                allFiles.Add(fileName);
                
            var newFeatures = allFiles
                .Select(f => Path.GetFullPath(f))
                .Distinct()
                .Select(f => Tuple.Create<string,global::Gherkin.Ast.Feature>(f, _featureFileRepository.GetByFilePath(f).GherkinDocument.Feature))
                .ToList() ?? new System.Collections.Generic.List<Tuple<string,global::Gherkin.Ast.Feature>>();

            if (newFeatures.Count == 0) {
                throw new System.IO.FileNotFoundException($"No features founds for ${fileName} o ${fileNameSearchPattern}");
            }

            return newFeatures;
        }
    }
}
