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
        public void Find_Features_with_exact_path_That_Are_Not_Implemented()
        {
            //arrange.
            _featureFileRepository.Setup(r => r.GetFeatureFilePaths())
                .Returns(new List<string> { "path1/feature1.feature", "path2/feature2.feature" })
                .Verifiable();

            _featureClassInfoRepository.Setup(r => r.GetFeatureClassesInfo())
                .Returns(new List<FeatureClassInfo> { FeatureClassInfo.FromFeatureClassType(typeof(FeatureWithExactPath)) })
                .Verifiable();

            var expectedFeature = new GherkinFeatureBuilder().Build();
            _featureFileRepository.Setup(r => r.GetByFilePath("path2/feature2.feature"))
                .Returns(new FeatureFile(new Gherkin.Ast.GherkinDocument(expectedFeature, null)))
                .Verifiable();

            //act.
            var features = _sut.Discover();

            //assert.
            _featureFileRepository.Verify();
            _featureClassInfoRepository.Verify();
            _featureFileRepository.Verify(r => r.GetByFilePath("path1/feature1.feature"), Times.Never);

            Assert.NotNull(features);
            Assert.Single(features);
            Assert.Same(expectedFeature, features[0]);
        }

        [FeatureFile(path: "path1/feature1.feature")]
        private sealed class FeatureWithExactPath : Feature { }

        [Fact]
        public void Find_features_with_pattern_name_that_are_not_implemented()
        {
            //arrange.
            _featureFileRepository.Setup(r => r.GetFeatureFilePaths())
                .Returns(new List<string> { "path1/feature1.feature", "path2/feature2.feature" })
                .Verifiable();

            _featureClassInfoRepository.Setup(r => r.GetFeatureClassesInfo())
                .Returns(new List<FeatureClassInfo> { FeatureClassInfo.FromFeatureClassType(typeof(FeatureWithPatternName)) })
                .Verifiable();

            var expectedFeature = new GherkinFeatureBuilder().Build();
            _featureFileRepository.Setup(r => r.GetByFilePath("path2/feature2.feature"))
                .Returns(new FeatureFile(new Gherkin.Ast.GherkinDocument(expectedFeature, null)))
                .Verifiable();

            //act.
            var features = _sut.Discover();

            //assert.
            _featureFileRepository.Verify();
            _featureClassInfoRepository.Verify();
            _featureFileRepository.Verify(r => r.GetByFilePath("path1/feature1.feature"), Times.Never);

            Assert.NotNull(features);
            Assert.Single(features);
            Assert.Same(expectedFeature, features[0]);
        }

        [FeatureFile(path: "*1.feature")]
        private sealed class FeatureWithPatternName : Feature { }

        [Fact]
        public void Find_features_with_pattern_folder_that_are_not_implemented()
        {
            //arrange.
            _featureFileRepository.Setup(r => r.GetFeatureFilePaths())
                .Returns(new List<string> { "path1/feature1.feature", "path2/feature2.feature" })
                .Verifiable();

            _featureClassInfoRepository.Setup(r => r.GetFeatureClassesInfo())
                .Returns(new List<FeatureClassInfo> { FeatureClassInfo.FromFeatureClassType(typeof(FeatureWithPatternFolder)) })
                .Verifiable();

            var expectedFeatures = new List<Gherkin.Ast.Feature> { new GherkinFeatureBuilder().Build(), new GherkinFeatureBuilder().Build() };
            _featureFileRepository.Setup(r => r.GetByFilePath("path1/feature1.feature"))
                .Returns(new FeatureFile(new Gherkin.Ast.GherkinDocument(expectedFeatures[0], null)))
                .Verifiable();
            _featureFileRepository.Setup(r => r.GetByFilePath("path2/feature2.feature"))
                .Returns(new FeatureFile(new Gherkin.Ast.GherkinDocument(expectedFeatures[1], null)))
                .Verifiable();

            //act.
            var features = _sut.Discover();

            //assert.
            _featureFileRepository.Verify();
            _featureClassInfoRepository.Verify();
            
            Assert.NotNull(features);
            Assert.Equal(2, features.Count);
            Assert.Same(expectedFeatures[0], features[0]);
            Assert.Same(expectedFeatures[1], features[1]);
        }

        [FeatureFile(path: "path-other/*.feature")]
        private sealed class FeatureWithPatternFolder : Feature { }
    }
}
