using System;
using System.IO;
using System.Linq;
using Xunit.Gherkin.Quick.vNext.FeatureFiles;

namespace Xunit.Gherkin.Quick.ProjectConsumer.UnitTests.FeatureFiles;

public class SystemFeatureFilesProviderTests
{
    [Fact]
    public void GetFeatureFiles_ListsSystemFeatureFiles()
    {
        var systemFeatureFileProvider = new SystemFeatureFilesProvider(new DirectoryInfo(Directory.GetCurrentDirectory()));

        var featureFiles = systemFeatureFileProvider.GetFeatureFiles();

        Assert.Equal(
            [
                ("AddNumbersTo5.feature", "./ScenarioBackground/AddNumbersTo5.feature"),
                ("AddTwoNumbers.feature", "./Addition_ForMultipleUseCases/AddTwoNumbers.feature"),
                ("AddTwoNumbersAsync.feature", "./Async/AddTwoNumbersAsync.feature"),
                ("AddTwoNumbersExtra.feature", "./Addition_ForMultipleUseCases/AddTwoNumbersExtra.feature"),
                ("BaseFolder.feature", "./FeatureFilePattern/BaseFolder.feature"),
                ("BeforeAfter.feature", "./BeforeAfterHooks/BeforeAfter.feature"),
                ("Concatenation.feature", "./ReuseStepsAcrossFeatures/Concatenation.feature"),
                ("EnsureOrderOfSteps.feature", "./GivenWhenThenTests/EnsureOrderOfSteps.feature"),
                ("InverseConcatenation.feature", "./ReuseStepsAcrossFeatures/InverseConcatenation.feature"),
                ("MathWithInfinity.feature", "./HandlingNotImplementedFeatures/MathWithInfinity.feature"),
                ("NestedFolder.feature", "./FeatureFilePattern/NestedFolder/NestedFolder.feature"),
                ("OutputMessages.feature", "./TestOutput/OutputMessages.feature"),
                ("PassParameters.feature", "./CucumberExpressions/PassParameters.feature"),
                ("Placeholders.feature", "./Placeholders/Placeholders.feature"),
                ("SimpleParameterTypes.feature", "./GivenWhenThenTests/SimpleParameterTypes.feature"),
                ("StarNotation.feature", "./StarNotation/StarNotation.feature"),
                ("TextBuilder.feature", "./DocString/TextBuilder.feature")
            ],
            featureFiles
                .Where(featureFile => featureFile.Name.EndsWith(".feature"))
                .Select(featureFile => (featureFile.Name, featureFile.FullName))
                .OrderBy(featureFileName => featureFileName.Name)
        );
    }
}