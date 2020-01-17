using System;
using System.Collections.Generic;

namespace Xunit.Gherkin.Quick
{
    internal sealed class ScenarioDiscoveryModel
    {
        private readonly IFileSystem _fileSystem;

        public ScenarioDiscoveryModel(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public List<ScenarioInfo> Discover(Type featureClassType)
        {
            if (featureClassType == null)
                throw new ArgumentNullException(nameof(featureClassType));

            var fileName = FeatureClassInfo.FromFeatureClassType(featureClassType).FeatureFilePath;
            if (!_fileSystem.FileExists(fileName))
                throw new System.IO.FileNotFoundException("Feature file not found.", fileName);

            throw new NotImplementedException();
        }
    }
}
