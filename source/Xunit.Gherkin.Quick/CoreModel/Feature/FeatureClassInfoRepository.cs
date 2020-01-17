using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xunit.Gherkin.Quick.CoreModel
{
    internal sealed class FeatureClassInfoRepository : IFeatureClassInfoRepository
    {
        public List<FeatureClassInfo> GetFeatureClassesInfo()
        {
            var allClasses = Assembly.GetEntryAssembly().GetExportedTypes();

            var featureClassType = typeof(Feature);

            var featureClasses = allClasses.Where(c => c.GetTypeInfo().IsSubclassOf(featureClassType))
                .ToList();

            var featureClassesInfo = featureClasses.Select(fc => FeatureClassInfo.FromFeatureClassType(fc))
                .ToList();

            return featureClassesInfo;
        }
    }
}
