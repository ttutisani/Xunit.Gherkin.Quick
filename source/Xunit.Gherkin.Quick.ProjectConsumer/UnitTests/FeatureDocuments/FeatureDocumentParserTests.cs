using Xunit.Gherkin.Quick.vNext.FeatureDocuments;

namespace Xunit.Gherkin.Quick.ProjectConsumer.UnitTests.FeatureDocuments;

public class FeatureDocumentParserTests
{
    private readonly FeatureDocumentParser _parser = new();

    [Theory]
    [InlineData("Feature file does not contain a feature definition.", "")]
    [InlineData("Parser errors:\n(1:1): expected: #EOF, #Language, #TagLine, #FeatureLine, #Comment, #Empty, got 'feature:'", "feature:")]
    public void Parse_InvalidDocument_HasErrorSet(string errorMessage, string featureFileContents)
    {
        var document = _parser.Parse(new InlineFeatureFile("test.feature", "./directory/test.feature", featureFileContents));

        Assert.Multiple(
            () => Assert.Equal("test.feature", document.Name),
            () => Assert.Equal("./directory/test.feature", document.FullName),
            () => Assert.Null(document.Content),
            () =>
            {
                Assert.NotNull(document.Error);
                Assert.Equal(errorMessage, document.Error.Message);
            }
        );
    }

    [Fact]
    public void Parse_ValidDocument_SetsParsedContents()
    {
        var document = _parser.Parse(new InlineFeatureFile("test.feature", "./directory/test.feature", @"
Feature: This is a test

Scenario: First scenario
  Given a test
  When test runs
  Then I get results
"));

        Assert.Multiple(
            () => Assert.Equal("test.feature", document.Name),
            () => Assert.Equal("./directory/test.feature", document.FullName),
            () =>
            {
                Assert.NotNull(document.Content);

                Assert.Multiple(
                    () => Assert.Equal("Feature", document.Content.Feature.Keyword),
                    () => Assert.Equal("This is a test", document.Content.Feature.Name),
                    () => Assert.Empty(document.Content.Feature.Tags),
                    () => Assert.Empty(document.Content.Comments),
                    () => Assert.Null(document.Content.Feature.Description),
                    () =>
                    {
                        var scenarioDefinition = Assert.Single(document.Content.Feature.Children);
                        var scenario = Assert.IsType<global::Gherkin.Ast.Scenario>(scenarioDefinition);

                        Assert.Multiple(
                            () => Assert.Equal("Scenario", scenario.Keyword),
                            () => Assert.Equal("First scenario", scenario.Name),
                            () => Assert.Empty(scenario.Tags),
                            () => Assert.Null(scenario.Description),
                            () => Assert.Collection(
                                scenario.Steps,
                                firstStep => Assert.Multiple(
                                    () => Assert.Equal("Given ", firstStep.Keyword),
                                    () => Assert.Equal("a test", firstStep.Text)
                                ),
                                secondStep => Assert.Multiple(
                                    () => Assert.Equal("When ", secondStep.Keyword),
                                    () => Assert.Equal("test runs", secondStep.Text)
                                ),
                                thirdStep => Assert.Multiple(
                                    () => Assert.Equal("Then ", thirdStep.Keyword),
                                    () => Assert.Equal("I get results", thirdStep.Text)
                                )
                            )
                        );
                    });

            },
            () => Assert.Null(document.Error)
        );
    }
}