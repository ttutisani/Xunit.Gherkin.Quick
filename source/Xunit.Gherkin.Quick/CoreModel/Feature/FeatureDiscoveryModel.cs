using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using static Xunit.Gherkin.Quick.FeatureFilePathInfo;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureDiscoveryModel
    {
        private readonly IFeatureFileRepository _featureFileRepository;

        public FeatureDiscoveryModel(IFeatureFileRepository featureFileRepository)
        {
            _featureFileRepository = featureFileRepository ?? throw new ArgumentNullException(nameof(featureFileRepository));
        }

        public List<FeatureAndPath> Discover(Type featureClassType)
        {
            if (featureClassType == null)
                throw new ArgumentNullException(nameof(featureClassType));

            var featureClassInfo = FeatureClassInfo.FromFeatureClassType(featureClassType);

            var featurePathsAndFiles = featureClassInfo.PathInfo.GetMatchingFeatures(_featureFileRepository);

            if (featurePathsAndFiles.Count == 0) {
                throw new FileNotFoundException($"No feature file found for class `${featureClassType.Name}`.");
            }

            return featurePathsAndFiles;
        }
    }
}
