using Moq;
using System.Collections.Generic;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class MissingFeatureDiscoveryModel_Discover_Should
    {
        private readonly Mock<IFeatureFileRepository> _featureFileRepository = new Mock<IFeatureFileRepository>();
        private readonly Mock<IFeatureClassInfoRepository> _featureClassInfoRepository = new Mock<IFeatureClassInfoRepository>();
        private readonly MissingFeatureDiscoveryModel _sut;

        public MissingFeatureDiscoveryModel_Discover_Should()
        {
            _sut = new MissingFeatureDiscoveryModel(_featureFileRepository.Object, _featureClassInfoRepository.Object);
        }

        [Fact]
        public void Find_Features_That_Are_Not_Implemented()
        {
            //arrange.
            _featureFileRepository.Setup(r => r.GetFeatureFilePaths())
                .Returns(new List<string> { "path1", "path2" })
                .Verifiable();

            _featureClassInfoRepository.Setup(r => r.GetFeatureClassesInfo())
                .Returns(new List<FeatureClassInfo> { new FeatureClassInfo("path1") })
                .Verifiable();

            var expectedFeature = new GherkinFeatureBuilder().Build();
            _featureFileRepository.Setup(r => r.GetByFilePath("path2"))
                .Returns(new FeatureFile(new Gherkin.Ast.GherkinDocument(expectedFeature, null)))
                .Verifiable();

            //act.
            var features = _sut.Discover();

            //assert.
            _featureFileRepository.Verify();
            _featureClassInfoRepository.Verify();
            _featureFileRepository.Verify(r => r.GetByFilePath("path1"), Times.Never);

            Assert.NotNull(features);
            Assert.Single(features);
            Assert.Same(expectedFeature, features[0]);
        }
    }
}
