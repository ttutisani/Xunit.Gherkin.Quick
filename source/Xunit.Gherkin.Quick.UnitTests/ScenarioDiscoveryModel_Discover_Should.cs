using Moq;
using System;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class ScenarioDiscoveryModel_Discover_Should
    {
        private readonly Mock<IFileSystem> _fileSystem = new Mock<IFileSystem>();
        private readonly ScenarioDiscoveryModel _sut;

        public ScenarioDiscoveryModel_Discover_Should()
        {
            _sut = new ScenarioDiscoveryModel(_fileSystem.Object);
        }

        [Fact]
        public void Requires_FeatureClassType()
        {
            //act / assert.
            Assert.Throws<ArgumentNullException>(() => _sut.Discover(null));
        }

        [Theory]
        [InlineData(typeof(MyFeature), "MyFeature.feature")]
        [InlineData(typeof(MyFeatureWithAttribute), "/my/path/to/feature/file.feature")]
        public void Discover_Throws_When_Feature_File_Not_Found(
            Type featureClassType,
            string fileName)
        {
            //arrange.
            _fileSystem.Setup(fs => fs.FileExists(fileName))
                .Returns(false)
                .Verifiable();

            //act / assert.
            Assert.Throws<System.IO.FileNotFoundException>(() => _sut.Discover(featureClassType));

            //assert.
            _fileSystem.Verify();
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
