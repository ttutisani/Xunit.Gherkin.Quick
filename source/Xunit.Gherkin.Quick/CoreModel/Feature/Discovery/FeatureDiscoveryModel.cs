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

            var repo = new FeatureFileRepository(fileNameSearchPattern);
            
            var allFiles = repo.GetFeatureFilePaths();

            var newFeatures = allFiles
                .FindAll(f => ! f.Equals(fileName)) // if pattern contains filename, this one is filtered
                .Select(f => Tuple.Create<string,global::Gherkin.Ast.Feature>(f, _featureFileRepository.GetByFilePath(f).GherkinDocument.Feature))
                .ToList();

            if (featureFile != null) {
                var feature = featureFile.GherkinDocument.Feature;
                newFeatures.Add(Tuple.Create(fileName, featureFile.GherkinDocument.Feature));
            } 
            
            if (newFeatures.Count == 0) {
                throw new System.IO.FileNotFoundException($"No features founds for ${fileName} o ${fileNameSearchPattern}");
            }
            return newFeatures;
        }
    }
}
