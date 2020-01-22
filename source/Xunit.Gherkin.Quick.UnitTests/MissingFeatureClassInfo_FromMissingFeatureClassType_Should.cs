using System;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class MissingFeatureClassInfo_FromMissingFeatureClassType_Should
    {
        [Fact]
        public void Require_ClassType()
        {
            //act / assert.
            Assert.Throws<ArgumentNullException>(() => MissingFeatureClassInfo.FromMissingFeatureClassType(null));
        }

        [Theory]
        [InlineData(typeof(MyFeature), "*.feature")]
        [InlineData(typeof(MyFeatureWithPattern), "someother.pattern")]
        public void Construct_InfoClass_With_Search_Pattern(
            Type classType,
            string pattern)
        {
            //act.
            var sut = MissingFeatureClassInfo.FromMissingFeatureClassType(classType);

            //assert.
            Assert.NotNull(sut);
            Assert.Equal(pattern, sut.FileNameSearchPattern);
        }

        private sealed class MyFeature : MissingFeature { }

        [FeatureFileSearchPattern("someother.pattern")]
        private sealed class MyFeatureWithPattern : MissingFeature { }
    }
}
