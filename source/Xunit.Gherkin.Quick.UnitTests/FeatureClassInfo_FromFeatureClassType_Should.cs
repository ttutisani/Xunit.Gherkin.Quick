using System;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class FeatureClassInfo_FromFeatureClassType_Should
    {
        [Fact]
        public void Require_ClassType()
        {
            //act / assert.
            Assert.Throws<ArgumentNullException>(() => MissingFeatureClassInfo.FromMissingFeatureClassType(null));
        }

        [Theory]
        [InlineData(typeof(MyFeature), typeof(SimpleFeatureFilePathInfo))]
        [InlineData(typeof(MyFeatureWithPattern), typeof(RegexFeatureFilePathInfo))]
        [InlineData(typeof(MyFeatureWithFileNameAndPattern), typeof(RegexFeatureFilePathInfo))]
        public void Construct_InfoClass_With_Search_Pattern(
            Type classType,
            Type pathInfoType)
        {
            //act.
            var sut = FeatureClassInfo.FromFeatureClassType(classType);

            //assert.
            Assert.NotNull(sut);
            Assert.IsType(pathInfoType, sut.PathInfo);
        }

        private sealed class MyFeature : Feature { }

        [FeatureFile(@"someother.*\.pattern", FeatureFilePathType.Regex)]
        private sealed class MyFeatureWithPattern : Feature { }

        [FeatureFile(@"some.*\.file", FeatureFilePathType.Regex)]
        private sealed class MyFeatureWithFileNameAndPattern : Feature { }
    }
}
