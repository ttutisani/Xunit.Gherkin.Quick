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
        [InlineData(typeof(MyFeature), "MyFeature*.feature")]
        [InlineData(typeof(MyFeatureWithPattern), "someother.pattern")]
        [InlineData(typeof(MyFeatureWithFileNameAndPattern), "someother.pattern")]
        public void Construct_InfoClass_With_Search_Pattern(
            Type classType,
            string pattern)
        {
            //act.
            var sut = FeatureClassInfo.FromFeatureClassType(classType);

            //assert.
            Assert.NotNull(sut);
            Assert.Equal(pattern, sut.FileNameSearchPattern);
        }

        private sealed class MyFeature : Feature { }

        [FeatureFileSearchPattern("someother.pattern")]
        private sealed class MyFeatureWithPattern : Feature { }

        [FeatureFile("some.file")]
        [FeatureFileSearchPattern("someother.pattern")]
        private sealed class MyFeatureWithFileNameAndPattern : Feature { }
    }
}
