using System.Linq;
using Xunit.Gherkin.Quick.vNext.FeatureFiles;

namespace Xunit.Gherkin.Quick.ProjectConsumer.UnitTests.FeatureFiles;

public class EmbeddedFeatureFilesProviderTests
{
    [Fact]
    public void GetFeatureFiles_ListsEmbeddedFeatureFiles()
    {
        var embeddedFeatureFileProvider = new EmbeddedFeatureFilesProvider(typeof(EmbeddedFeatureFilesProviderTests).Assembly);

        var featureFiles = embeddedFeatureFileProvider.GetFeatureFiles();

        Assert.Equal(
            [
                ("AddNumbersTo5.feature", "Xunit.Gherkin.Quick.ProjectConsumer.ScenarioBackground.AddNumbersTo5.feature"),
                ("AddTwoNumbers.feature", "Xunit.Gherkin.Quick.ProjectConsumer.Addition_ForMultipleUseCases.AddTwoNumbers.feature"),
                ("AddTwoNumbersAsync.feature", "Xunit.Gherkin.Quick.ProjectConsumer.Async.AddTwoNumbersAsync.feature"),
                ("AddTwoNumbersExtra.feature", "Xunit.Gherkin.Quick.ProjectConsumer.Addition_ForMultipleUseCases.AddTwoNumbersExtra.feature"),
                ("BaseFolder.feature", "Xunit.Gherkin.Quick.ProjectConsumer.FeatureFilePattern.BaseFolder.feature"),
                ("BeforeAfter.feature", "Xunit.Gherkin.Quick.ProjectConsumer.BeforeAfterHooks.BeforeAfter.feature"),
                ("Concatenation.feature", "Xunit.Gherkin.Quick.ProjectConsumer.ReuseStepsAcrossFeatures.Concatenation.feature"),
                ("EnsureOrderOfSteps.feature", "Xunit.Gherkin.Quick.ProjectConsumer.GivenWhenThenTests.EnsureOrderOfSteps.feature"),
                ("InverseConcatenation.feature", "Xunit.Gherkin.Quick.ProjectConsumer.ReuseStepsAcrossFeatures.InverseConcatenation.feature"),
                ("MathWithInfinity.feature", "Xunit.Gherkin.Quick.ProjectConsumer.HandlingNotImplementedFeatures.MathWithInfinity.feature"),
                ("NestedFolder.feature", "Xunit.Gherkin.Quick.ProjectConsumer.FeatureFilePattern.NestedFolder.NestedFolder.feature"),
                ("OutputMessages.feature", "Xunit.Gherkin.Quick.ProjectConsumer.TestOutput.OutputMessages.feature"),
                ("PassParameters.feature", "Xunit.Gherkin.Quick.ProjectConsumer.CucumberExpressions.PassParameters.feature"),
                ("Placeholders.feature", "Xunit.Gherkin.Quick.ProjectConsumer.Placeholders.Placeholders.feature"),
                ("SimpleParameterTypes.feature", "Xunit.Gherkin.Quick.ProjectConsumer.GivenWhenThenTests.SimpleParameterTypes.feature"),
                ("StarNotation.feature", "Xunit.Gherkin.Quick.ProjectConsumer.StarNotation.StarNotation.feature"),
                ("TextBuilder.feature", "Xunit.Gherkin.Quick.ProjectConsumer.DocString.TextBuilder.feature")
            ],
            featureFiles
                .Select(featureFile => (featureFile.Name, featureFile.FullName))
                .OrderBy(featureFileName => featureFileName.Name)
        );
    }
}