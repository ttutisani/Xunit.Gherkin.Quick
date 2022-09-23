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
        [InlineData(typeof(MyFeature), false)]
        [InlineData(typeof(MyFeatureWithPattern), true)]
        [InlineData(typeof(MyFeatureWithFileNameAndPattern), true)]
        public void Construct_InfoClass_With_Search_Pattern(
            Type classType,
            bool isPattern)
        {
            //act.
            var sut = FeatureClassInfo.FromFeatureClassType(classType);

            //assert.
            Assert.NotNull(sut);
            Assert.Equal(isPattern, sut.IsPattern);
        }

        private sealed class MyFeature : Feature { }

        [FeatureFile("someother*.pattern")]
        private sealed class MyFeatureWithPattern : Feature { }

        [FeatureFile("some*.file")]
        private sealed class MyFeatureWithFileNameAndPattern : Feature { }
    }
}
