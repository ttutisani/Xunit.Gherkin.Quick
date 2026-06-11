
using Xunit.Gherkin.Quick.vNext.FeatureDocuments;

namespace Xunit.Gherkin.Quick.ProjectConsumer.UnitTests.FeatureDocuments;

public class FeatureDocumentTests
{
    [Fact]
    public void Initialize_SetsFileName()
    {
        var featureDocument = new FeatureDocument(
            new InlineFeatureFile("document.feature", "./tests/document.feature", @""),
            new global::Gherkin.Parser()
        );

        Assert.Multiple(
            () => Assert.Equal("document.feature", featureDocument.Name),
            () => Assert.Equal("./tests/document.feature", featureDocument.FullName)
        );
    }

    [Fact]
    public void Document_ProvidesParsedGherkinFeature()
    {
        var featureDocument = new FeatureDocument(
            new InlineFeatureFile("document.feature", "./tests/document.feature", @"
Feature: this is a test
Scenario: still a test
  Given a step
  When another step
  Then yet another step
"),
            new global::Gherkin.Parser()
        );

        Assert.Multiple(
            () => Assert.Equal("document.feature", featureDocument.Name),
            () => Assert.Equal("./tests/document.feature", featureDocument.FullName),
            () =>
            {
                Assert.NotNull(featureDocument.Content);
                Assert.Equal("this is a test", featureDocument.Content.Feature.Name);
                Assert.Null(featureDocument.Content.Feature.Description);

                var scenarioDefinition = Assert.Single(featureDocument.Content.Feature.Children);
                var scenario = Assert.IsType<global::Gherkin.Ast.Scenario>(scenarioDefinition);
                Assert.Equal("still a test", scenario.Name);
                Assert.Null(scenario.Description);
                Assert.Collection(
                    scenario.Steps,
                    firstStep => Assert.Multiple(
                        () => Assert.Equal("Given ", firstStep.Keyword),
                        () => Assert.Equal("a step", firstStep.Text)
                    ),
                    secondStep => Assert.Multiple(
                        () => Assert.Equal("When ", secondStep.Keyword),
                        () => Assert.Equal("another step", secondStep.Text)
                    ),
                    thirdStep => Assert.Multiple(
                        () => Assert.Equal("Then ", thirdStep.Keyword),
                        () => Assert.Equal("yet another step", thirdStep.Text)
                    )
                );
            },
            () => Assert.Null(featureDocument.Error)
        );
    }

    [Theory]
    [InlineData("", "Feature file does not contain a feature definition.")]
    [InlineData("feature: this is a test", "Parser errors:\n(1:1): expected: #EOF, #Language, #TagLine, #FeatureLine, #Comment, #Empty, got 'feature: this is a test'")]
    public void Document_ProvidesErrorWhenInvalid(string content, string expectedError)
    {
        var featureDocument = new FeatureDocument(
            new InlineFeatureFile("document.feature", "./tests/document.feature", content),
            new global::Gherkin.Parser()
        );

        Assert.Multiple(
            () => Assert.Equal("document.feature", featureDocument.Name),
            () => Assert.Equal("./tests/document.feature", featureDocument.FullName),
            () => Assert.Null(featureDocument.Content),
            () =>
            {
                Assert.NotNull(featureDocument.Error);
                Assert.Equal(expectedError, featureDocument.Error.Message);
            }
        );
    }
}