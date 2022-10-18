using System.Collections.Generic;

namespace Xunit.Gherkin.Quick
{
    internal interface IFeatureClassInfoRepository
    {
        List<FeatureClassInfo> GetFeatureClassesInfo();
    }
}
