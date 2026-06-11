using System.Linq;
using Xunit.Gherkin.Quick.FeatureFiles;

namespace Xunit.Gherkin.Quick.Tests.Units.FeatureFiles;

public class EmbeddedFeatureFilesProviderTests
{
    [Fact]
    public void GetFeatureFiles_ListsEmbeddedFeatureFiles()
    {
        var embeddedFeatureFileProvider = new EmbeddedFeatureFilesProvider(typeof(EmbeddedFeatureFilesProviderTests).Assembly);

        var featureFiles = embeddedFeatureFileProvider.GetFeatureFiles();

        Assert.Collection(
            featureFiles
                .Select(featureFile => (featureFile.Name, featureFile.FullName))
                .OrderBy(featureFileName => featureFileName.Name),
            featureFile => Assert.Multiple(
                () => Assert.Equal("AddNumbersTo5.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.ScenarioBackground.AddNumbersTo5.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("AddTwoNumbers.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.Addition_ForMultipleUseCases.AddTwoNumbers.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("AddTwoNumbersAsync.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.Async.AddTwoNumbersAsync.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("AddTwoNumbersExtra.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.Addition_ForMultipleUseCases.AddTwoNumbersExtra.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("BaseFolder.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.FeatureFilePattern.BaseFolder.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("BeforeAfter.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.BeforeAfterHooks.BeforeAfter.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("Concatenation.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.ReuseStepsAcrossFeatures.Concatenation.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("EnsureOrderOfSteps.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.GivenWhenThenTests.EnsureOrderOfSteps.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("InverseConcatenation.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.ReuseStepsAcrossFeatures.InverseConcatenation.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("MathWithInfinity.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.HandlingNotImplementedFeatures.MathWithInfinity.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("NestedFolder.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.FeatureFilePattern.NestedFolder.NestedFolder.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("OutputMessages.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.TestOutput.OutputMessages.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("PassParameters.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.CucumberExpressions.PassParameters.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("Placeholders.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.Placeholders.Placeholders.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("SimpleParameterTypes.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.GivenWhenThenTests.SimpleParameterTypes.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("StarNotation.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.StarNotation.StarNotation.feature", featureFile.FullName)
            ),
            featureFile => Assert.Multiple(
                () => Assert.Equal("TextBuilder.feature", featureFile.Name),
                () => Assert.Equal("Xunit.Gherkin.Quick.Tests.Features.DocStrings.TextBuilder.feature", featureFile.FullName)
            )
        );
    }
}