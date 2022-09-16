using System;
using System.Collections.Generic;
using System.Linq;

namespace Xunit.Gherkin.Quick
{
    internal sealed class MissingFeatureDiscoveryModel
    {
        private readonly IFeatureFileRepository _featureFileRepository;
        private readonly IFeatureClassInfoRepository _featureClassInfoRepository;

        public MissingFeatureDiscoveryModel(
            IFeatureFileRepository featureFileRepository,
            IFeatureClassInfoRepository featureClassInfoRepository)
        {
            _featureFileRepository = featureFileRepository ?? throw new ArgumentNullException(nameof(featureFileRepository));
            _featureClassInfoRepository = featureClassInfoRepository ?? throw new ArgumentNullException(nameof(featureClassInfoRepository));
        }

        public List<global::Gherkin.Ast.Feature> Discover()
        {
            var allFiles = _featureFileRepository.GetFeatureFilePaths();
            var referencedFiles = _featureClassInfoRepository.GetFeatureClassesInfo()
                .Select(fc => fc.FeatureFilePath)
                .ToList();
                
            var referencedFilesByPattern = _featureClassInfoRepository.GetFeatureClassesInfo()
                .SelectMany(fc => new FeatureFileRepository(fc.FileNameSearchPattern).GetFeatureFilePaths())
                .ToList();

            var newFiles = allFiles
                .Where(f => !referencedFilesByPattern.Contains(f))
                .Where(f => !referencedFiles.Contains(f))
                .ToList();            

            var newFeatures = newFiles.Select(f => _featureFileRepository.GetByFilePath(f))
                .Select(ff => ff.GherkinDocument.Feature)
                .ToList();

            return newFeatures;
        }
    }
}
