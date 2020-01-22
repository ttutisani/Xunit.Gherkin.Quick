using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureClassInfoRepository : IFeatureClassInfoRepository
    {
        private readonly Assembly _testAssembly;

        public FeatureClassInfoRepository(Assembly testAssembly)
        {
            _testAssembly = testAssembly ?? throw new System.ArgumentNullException(nameof(testAssembly));
        }

        public List<FeatureClassInfo> GetFeatureClassesInfo()
        {
            var allClasses = _testAssembly.GetExportedTypes();

            var featureClassType = typeof(Feature);

            var featureClasses = allClasses.Where(c => c.GetTypeInfo().IsSubclassOf(featureClassType))
                .ToList();

            var featureClassesInfo = featureClasses.Select(fc => FeatureClassInfo.FromFeatureClassType(fc))
                .ToList();

            return featureClassesInfo;
        }
    }
}
