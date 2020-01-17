using System;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioDiscoveryModel
    {
        private readonly IFeatureFileRepository _featureFileRepository;

        public ScenarioDiscoveryModel(IFeatureFileRepository featureFileRepository)
        {
            _featureFileRepository = featureFileRepository ?? throw new ArgumentNullException(nameof(featureFileRepository));
        }

        public global::Gherkin.Ast.Feature Discover(Type featureClassType)
        {
            if (featureClassType == null)
                throw new ArgumentNullException(nameof(featureClassType));

            var fileName = FeatureClassInfo.FromFeatureClassType(featureClassType).FeatureFilePath;
            var featureFile = _featureFileRepository.GetByFilePath(fileName);
            if (featureFile == null)
                throw new System.IO.FileNotFoundException("Feature file not found.", fileName);

            var feature = featureFile.GherkinDocument.Feature;
            return feature;
        }
    }
}
