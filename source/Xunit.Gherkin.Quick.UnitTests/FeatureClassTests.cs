namespace Xunit.Gherkin.Quick.UnitTests
{
    public class FeatureClassTests
    {
        [Fact]
        public void Ctor_Initializes_Properties()
        {
            //arrange.
            var featureFilePath = "some path 123";

            //act.
            var sut = new FeatureClass(featureFilePath);

            //assert.
            Assert.Equal(featureFilePath, sut.FeatureFilePath);
        }

        [Fact]
        public void FromFeatureInstance_Creates_FeatureClass_With_Default_FilePath_If_No_Attribute()
        {
            //arrange.
            var featureInstance = new FeatureWithoutFilePath();

            //act.
            var sut = FeatureClass.FromFeatureInstance(featureInstance);

            //assert.
            Assert.NotNull(sut);
            Assert.Equal($"{nameof(FeatureWithoutFilePath)}.feature", sut.FeatureFilePath);
        }

        [Fact]
        public void FromFeatureInstance_Creates_FeatureClass_With_FilePath_From_Attribute()
        {
            //arrange.
            var featureInstance = new FeatureWithFilePath();

            //act.
            var sut = FeatureClass.FromFeatureInstance(featureInstance);

            //assert.
            Assert.NotNull(sut);
            Assert.Equal(PathFor_FeatureWithFilePath, sut.FeatureFilePath);
        }

        private sealed class FeatureWithoutFilePath : Feature
        {
        }

        private const string PathFor_FeatureWithFilePath = "some file path const 123";

        [FeatureFile(PathFor_FeatureWithFilePath)]
        private sealed class FeatureWithFilePath : Feature
        {
        }
    }
}
