using Moq;
using System;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class FeatureDiscoveryModel_Discover_Should
    {
        private readonly Mock<IFeatureFileRepository> _featureFileRepository = new Mock<IFeatureFileRepository>();
        private readonly FeatureDiscoveryModel _sut;

        public FeatureDiscoveryModel_Discover_Should()
        {
            _sut = new FeatureDiscoveryModel(_featureFileRepository.Object);
        }

        [Fact]
        public void Require_FeatureClassType()
        {
            //act / assert.
            Assert.Throws<ArgumentNullException>(() => _sut.Discover(null));
        }

        [Theory]
        [InlineData(typeof(MyFeature), "MyFeature.feature")]
        [InlineData(typeof(MyFeatureWithAttribute), "/my/path/to/feature/file.feature")]
        public void Throw_When_Feature_File_Not_Found(
            Type featureClassType,
            string fileName)
        {
            //arrange.
            _featureFileRepository.Setup(r => r.GetByFilePath(fileName))
                .Returns<FeatureFile>(null)
                .Verifiable();

            //act / assert.
            Assert.Throws<System.IO.FileNotFoundException>(() => _sut.Discover(featureClassType));

            //assert.
            _featureFileRepository.Verify();
        }

        [Theory]
        [InlineData(typeof(MyFeature), "MyFeature.feature")]
        [InlineData(typeof(MyFeatureWithAttribute), "/my/path/to/feature/file.feature")]
        public void Find_Scenarios_In_Feature_File(
            Type featureClassType,
            string fileName)
        {
            //arrange.
            var gherkinFeature = new GherkinFeatureBuilder().WithScenario("scenario1", steps =>
                steps.Given("step 1", null))
                .Build();
            _featureFileRepository.Setup(r => r.GetByFilePath(fileName))
                .Returns(new FeatureFile(new Gherkin.Ast.GherkinDocument(gherkinFeature, null)))
                .Verifiable();

            //act.
            var feature = _sut.Discover(featureClassType).GetEnumerator().Current;

            //assert.
            _featureFileRepository.Verify();

            Assert.NotNull(feature.Item2);
            Assert.Same(gherkinFeature, feature.Item2);
        }

        private sealed class MyFeature : Feature
        {

        }

        [FeatureFile(path: "/my/path/to/feature/file.feature")]
        private sealed class MyFeatureWithAttribute : Feature
        {

        }
    }
}
