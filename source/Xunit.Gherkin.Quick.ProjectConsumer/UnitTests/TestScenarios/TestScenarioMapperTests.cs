using Gherkin;
using Xunit.Gherkin.Quick.vNext.FeatureDocuments;
using Xunit.Gherkin.Quick.vNext.TestScenarios;

namespace Xunit.Gherkin.Quick.ProjectConsumer.UnitTests.TestScenarios;

public class TestScenarioMapperTests
{
    private readonly TestScenarioMapper _testScenarioMapper = new(new GherkinDialectProvider());

    [Fact]
    public void Map_MapsBasicInformationAndSteps()
    {
        var testScenario = _ParseTestScenario(@"
            Feature: this is a feature
            Scenario: this is a scenario
              Given a step
              And a subsequent step
              When an action step
              Then a final step
              But another final step
");

        Assert.Multiple(
            () => Assert.Equal("this is a feature", testScenario.FeatureName),
            () => Assert.Equal("this is a scenario", testScenario.ScenarioName),
            () => Assert.Equal("en", testScenario.Locale.Name),
            () => Assert.Empty(testScenario.Tags),
            () => Assert.Collection(
                testScenario.Steps,
                firstStep => Assert.Multiple(
                    () => Assert.Equal(TestStepType.Given, firstStep.Type),
                    () => Assert.Equal("a step", firstStep.Text),
                    () => Assert.Null(firstStep.DocStringArgument),
                    () => Assert.Null(firstStep.TableArgument)
                ),
                secondStep => Assert.Multiple(
                    () => Assert.Equal(TestStepType.And, secondStep.Type),
                    () => Assert.Equal("a subsequent step", secondStep.Text),
                    () => Assert.Null(secondStep.DocStringArgument),
                    () => Assert.Null(secondStep.TableArgument)
                ),
                thirdStep => Assert.Multiple(
                    () => Assert.Equal(TestStepType.When, thirdStep.Type),
                    () => Assert.Equal("an action step", thirdStep.Text),
                    () => Assert.Null(thirdStep.DocStringArgument),
                    () => Assert.Null(thirdStep.TableArgument)
                ),
                fourthStep => Assert.Multiple(
                    () => Assert.Equal(TestStepType.Then, fourthStep.Type),
                    () => Assert.Equal("a final step", fourthStep.Text),
                    () => Assert.Null(fourthStep.DocStringArgument),
                    () => Assert.Null(fourthStep.TableArgument)
                ),
                fifthStep => Assert.Multiple(
                    () => Assert.Equal(TestStepType.But, fifthStep.Type),
                    () => Assert.Equal("another final step", fifthStep.Text),
                    () => Assert.Null(fifthStep.DocStringArgument),
                    () => Assert.Null(fifthStep.TableArgument)
                )
            )
        );
    }

    [Fact]
    public void Map_MapsTags()
    {
        var testScenario = _ParseTestScenario(@"
            @tag-1 @tag-2
            Feature: this is another feature

            @tag-3 @tag-4
            Scenario: this is another scenario
              Given a step
");

        Assert.Multiple(
            () => Assert.Equal("this is another feature", testScenario.FeatureName),
            () => Assert.Equal("this is another scenario", testScenario.ScenarioName),
            () => Assert.Equal("en", testScenario.Locale.Name),
            () => Assert.Equal(["tag-1", "tag-2", "tag-3", "tag-4"], testScenario.Tags),
            () => Assert.Collection(
                testScenario.Steps,
                firstStep => Assert.Multiple(
                    () => Assert.Equal(TestStepType.Given, firstStep.Type),
                    () => Assert.Equal("a step", firstStep.Text),
                    () => Assert.Null(firstStep.DocStringArgument),
                    () => Assert.Null(firstStep.TableArgument)
                )
            )
        );
    }

    [Fact]
    public void Map_MapsLocale()
    {
        var testScenario = _ParseTestScenario(@"
            # language: sk
            Funkcia: this is another feature

            Scenár: this is another scenario
              Pokiaľ a step
");

        Assert.Multiple(
            () => Assert.Equal("this is another feature", testScenario.FeatureName),
            () => Assert.Equal("this is another scenario", testScenario.ScenarioName),
            () => Assert.Equal("sk", testScenario.Locale.Name),
            () => Assert.Empty(testScenario.Tags),
            () => Assert.Collection(
                testScenario.Steps,
                firstStep => Assert.Multiple(
                    () => Assert.Equal(TestStepType.Given, firstStep.Type),
                    () => Assert.Equal("a step", firstStep.Text),
                    () => Assert.Null(firstStep.DocStringArgument),
                    () => Assert.Null(firstStep.TableArgument)
                )
            )
        );
    }

    [Fact]
    public void Map_MapsDocString()
    {
        var testScenario = _ParseTestScenario(@"
            Feature: this is doc string feature

            Scenario: this is doc string scenario
              Given a step
              """"""plain-text
              doc string
              """"""
");

        Assert.Multiple(
            () => Assert.Equal("this is doc string feature", testScenario.FeatureName),
            () => Assert.Equal("this is doc string scenario", testScenario.ScenarioName),
            () => Assert.Equal("en", testScenario.Locale.Name),
            () => Assert.Empty(testScenario.Tags),
            () => Assert.Collection(
                testScenario.Steps,
                firstStep => Assert.Multiple(
                    () => Assert.Equal(TestStepType.Given, firstStep.Type),
                    () => Assert.Equal("a step", firstStep.Text),
                    () =>
                    {
                        Assert.NotNull(firstStep.DocStringArgument);
                        Assert.Equal("plain-text", firstStep.DocStringArgument.ContentType);
                        Assert.Equal("doc string", firstStep.DocStringArgument.Content);
                    },
                    () => Assert.Null(firstStep.TableArgument)
                )
            )
        );
    }

    [Fact]
    public void Map_MapsDataTable()
    {
        var testScenario = _ParseTestScenario(@"
            Feature: this is data table feature

            Scenario: this is data table scenario
              Given a step
              | row 1 - cell 1 | row 1 - cell 2 |
              | row 2 - cell 1 | row 2 - cell 2 |
              | row 3 - cell 1 | row 3 - cell 2 |
");

        Assert.Multiple(
            () => Assert.Equal("this is data table feature", testScenario.FeatureName),
            () => Assert.Equal("this is data table scenario", testScenario.ScenarioName),
            () => Assert.Equal("en", testScenario.Locale.Name),
            () => Assert.Empty(testScenario.Tags),
            () => Assert.Collection(
                testScenario.Steps,
                firstStep => Assert.Multiple(
                    () => Assert.Equal(TestStepType.Given, firstStep.Type),
                    () => Assert.Equal("a step", firstStep.Text),
                    () => Assert.Null(firstStep.DocStringArgument),
                    () =>
                    {
                        Assert.NotNull(firstStep.TableArgument);
                        Assert.Collection(
                            firstStep.TableArgument.Rows,
                            firstRow => Assert.Collection(
                                firstRow.Cells,
                                firstCell => Assert.Equal("row 1 - cell 1", firstCell.Value),
                                secondCell => Assert.Equal("row 1 - cell 2", secondCell.Value)
                            ),
                            secondRow => Assert.Collection(
                                secondRow.Cells,
                                firstCell => Assert.Equal("row 2 - cell 1", firstCell.Value),
                                secondCell => Assert.Equal("row 2 - cell 2", secondCell.Value)
                            ),
                            thirdRow => Assert.Collection(
                                thirdRow.Cells,
                                firstCell => Assert.Equal("row 3 - cell 1", firstCell.Value),
                                secondCell => Assert.Equal("row 3 - cell 2", secondCell.Value)
                            )
                        );
                    }
                )
            )
        );
    }

    private TestScenario _ParseTestScenario(string content)
    {
        var parser = new FeatureDocumentParser();
        var featureFile = new InlineFeatureFile("test.feature", "./test.feature", content);
        var featureDocument = parser.Parse(featureFile);

        Assert.Null(featureDocument.Error);
        Assert.NotNull(featureDocument.Feature);
        var scenarioDefinition = Assert.Single(featureDocument.Feature.Children, scenarioDefinition => scenarioDefinition is global::Gherkin.Ast.Scenario);
        var scenario = Assert.IsType<global::Gherkin.Ast.Scenario>(scenarioDefinition);

        return _testScenarioMapper.Map(featureDocument.Feature, scenario);
    }
}