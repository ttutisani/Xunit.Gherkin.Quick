using System.Linq;
using Xunit.Gherkin.Quick.ProjectConsumer.Addition.Async;
using Xunit.Gherkin.Quick.vNext;

namespace Xunit.Gherkin.Quick.ProjectConsumer.UnitTests;

public class FeatureDocumentMatcherTests
{
    private readonly FeatureDocumentMatcher _featureDocumentMatcher = new();

    [Fact]
    public void GetMatchingDocuments_WhenFeatureTypeIsAddTwoNumbersAsync_ReturnsMatchingFeatureDocument()
    {
        var matchingFeatureDocuments = _featureDocumentMatcher.GetMatchingDocuments(typeof(AddTwoNumbersAsync));

        var matchingFeatureDocument = Assert.Single(matchingFeatureDocuments);
        Assert.Multiple(
            () => Assert.Equal("AddTwoNumbersAsync.feature", matchingFeatureDocument.Name),
            () => Assert.Equal("./Async/AddTwoNumbersAsync.feature", matchingFeatureDocument.FullName),
            () =>
            {
                Assert.NotNull(matchingFeatureDocument.Feature);
                Assert.Equal("AddTwoNumbers Async", matchingFeatureDocument.Feature.Name);

                var scenarioDefinition = Assert.Single(matchingFeatureDocument.Feature.Children);
                Assert.Equal("Add two numbers", scenarioDefinition.Name);
            },
            () => Assert.Null(matchingFeatureDocument.Error)
        );
    }
}