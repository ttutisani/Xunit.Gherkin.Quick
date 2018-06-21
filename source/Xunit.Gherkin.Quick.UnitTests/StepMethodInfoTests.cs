using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class StepMethodInfoTests
    {
        [Fact]
        public void Ctor_Initializes_Properties()
        {
            //arrange.
            var featureInstance = new FeatureForCtorTest();

            //act.
            var sut = StepMethodInfo.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForCtorTest.When_Something)), featureInstance);

            //assert.
            Assert.Equal(StepMethodKind.When, sut.Kind);
            Assert.Equal(FeatureForCtorTest.WhenStepText, sut.Pattern);
        }

        private sealed class FeatureForCtorTest : Feature
        {
            public const string WhenStepText = "some text 123";

            [When(WhenStepText)]
            public void When_Something()
            {

            }
        }

        [Fact]
        public void Clone_Creates_Similar_Instance()
        {
            //arrange.
            var featureInstance = new FeatureForCtorTest();
            var sut = StepMethodInfo.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForCtorTest.When_Something)), featureInstance);

            //act.
            var clone = sut.Clone();

            //assert.
            Assert.NotNull(clone);
            Assert.True(clone.IsSameAs(sut));
        }

        [Fact]
        public void IsSameAs_Identifies_Similar_Instances()
        {
            //arrange.
            var featureInstance = new FeatureForCtorTest();
            var sut = StepMethodInfo.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForCtorTest.When_Something)), featureInstance);
            var clone = StepMethodInfo.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForCtorTest.When_Something)), featureInstance);

            //act.
            var same = sut.IsSameAs(clone) && clone.IsSameAs(sut);

            //assert.
            Assert.True(same);
        }

        [Fact]
        public void DigestScenarioStepValues_Expects_Exact_Number_Of_Groups_Not_Less()
        {
            //arrange.
            var featureInstance = new FeatureForApplyArgumentValues_LessThanNeededGroups();
            var sut = StepMethodInfo.FromMethodInfo(
                featureInstance.GetType().GetMethod(nameof(FeatureForApplyArgumentValues_LessThanNeededGroups.Method_With_Arguments)),
                featureInstance);

            var number = 123;
            var text = "Ana";
            var date = new DateTime(2018, 5, 23);
            var stepText = FeatureForApplyArgumentValues_LessThanNeededGroups.StepMethodText
                .Replace(@"(\d+)", $"{number}")
                .Replace(@"(\w+)", $"{text}");

            var step = CreateGherkinDocument("some scenario", "Then " + stepText)
                .Feature.Children.First().Steps.First();

            //act / assert.
            Assert.Throws<InvalidOperationException>(() => sut.DigestScenarioStepValues(step));
        }

        private sealed class FeatureForApplyArgumentValues_LessThanNeededGroups : Feature
        {
            public const string StepMethodText = @"I should have (\d+) apples from (\w+) by 5/23/2018";

            [Then(StepMethodText)]
            public void Method_With_Arguments(int number, string text, DateTime date)
            { }
        }

        [Fact]
        public void DigestScenarioStepValues_Sets_Primitive_Values()
        {
            //arrange.
            var featureInstance = new FeatureForApplyArgumentValues();
            var sut = StepMethodInfo.FromMethodInfo(
                featureInstance.GetType().GetMethod(nameof(FeatureForApplyArgumentValues.Method_With_Arguments)),
                featureInstance);

            var number = 123;
            var text = "Ana";
            var date = new DateTime(2018, 5, 23);
            var stepText = FeatureForApplyArgumentValues.StepMethodText
                .Replace(@"(\d+)", $"{number}")
                .Replace(@"(\w+)", $"{text}")
                .Replace(@"([\d/]+)", $"{date.Month}/{date.Day}/{date.Year}");

            var step = CreateGherkinDocument("some scenario", "Then " + stepText)
                .Feature.Children.First().Steps.First();

            //act.
            sut.DigestScenarioStepValues(step);

            //assert.
            var digestedText = sut.GetDigestedStepText();
            Assert.Equal(stepText, digestedText);
        }

        private static Gherkin.Ast.GherkinDocument CreateGherkinDocument(string scenario, params string[] steps)
        {
            var gherkinText =
@"Feature: Some Sample Feature
    In order to learn Math
    As a regular human
    I want to add two numbers using Calculator

Scenario: " + scenario + @"
" + string.Join(Environment.NewLine, steps)
;
            using (var gherkinStream = new MemoryStream(Encoding.UTF8.GetBytes(gherkinText)))
            using (var gherkinReader = new StreamReader(gherkinStream))
            {
                var parser = new Gherkin.Parser();
                return parser.Parse(gherkinReader);
            }
        }

        private sealed class FeatureForApplyArgumentValues : Feature
        {
            public const string StepMethodText = @"I should have (\d+) apples from (\w+) by ([\d/]+)";

            [Then(StepMethodText)]
            public void Method_With_Arguments(int number, string text, DateTime date)
            { }
        }

        [Fact]
        public async Task Execute_Invokes_StepMethod()
        {
            //arrange.
            var featureInstance = new FeatureForExecuteTest();
            var sut = StepMethodInfo.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForExecuteTest.Call_This_Method)), featureInstance);

            //act.
            await sut.ExecuteAsync();

            //assert.
            Assert.True(featureInstance.Called);
        }

        private sealed class FeatureForExecuteTest : Feature
        {
            public bool Called { get; private set; } = false;

            [And("Call this")]
            public void Call_This_Method()
            {
                Called = true;
            }
        }

        [Fact]
        public void GetDigestedStepText_Throws_Error_If_Not_Yet_Digested()
        {
            //arrange.
            var featureInstance = new Feature_For_GetDigestedStepTextTest();
            var sut = StepMethodInfo.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(Feature_For_GetDigestedStepTextTest.When_Something_Method)), featureInstance);

            //act / assert.
            Assert.Throws<InvalidOperationException>(() => sut.GetDigestedStepText());
        }

        private sealed class Feature_For_GetDigestedStepTextTest : Feature
        {
            [When("something")]
            public void When_Something_Method() { }
        }

        [Fact]
        public void FromMethodInfo_Creates_StepMethodInfo_With_DataTable()
        {
            //arrange.
            var featureInstance = new FeatureWithDataTableScenarioStep();

            //act.
            var sut = StepMethodInfo.FromMethodInfo(
                featureInstance.GetType().GetMethod(nameof(FeatureWithDataTableScenarioStep.When_DataTable_Is_Expected)),
                featureInstance
                );

            //assert.
            Assert.NotNull(sut);
        }

        [Fact]
        public void DigestScenarioStepValues_Sets_DataTable_Value()
        {
            //arrange.
            var featureInstance = new FeatureWithDataTableScenarioStep();
            var sut = StepMethodInfo.FromMethodInfo(
                featureInstance.GetType().GetMethod(nameof(FeatureWithDataTableScenarioStep.When_DataTable_Is_Expected)),
                featureInstance
                );

            var step = CreateGherkinDocument("some scenario 1212",
                    "When " + FeatureWithDataTableScenarioStep.Steptext + Environment.NewLine +
@"  | First argument | Second argument | Result |
    | 1              |       2         |       3|
    | a              |   b             | c      |
"
                    ).Feature.Children.First().Steps.First();

            //act.
            sut.DigestScenarioStepValues(step);

            //assert.
            var digestedText = sut.GetDigestedStepText();
            Assert.Equal(FeatureWithDataTableScenarioStep.Steptext, digestedText);
        }

        private sealed class FeatureWithDataTableScenarioStep : Feature
        {
            public Gherkin.Ast.DataTable ReceivedDataTable { get; private set; }

            public const string Steptext = "Some step text";

            [When(Steptext)]
            public void When_DataTable_Is_Expected(Gherkin.Ast.DataTable dataTable)
            {
                ReceivedDataTable = dataTable;
            }
        }

        [Fact]
        public void FromMethodInfo_Creates_StepMethodInfo_With_DocString()
        {
            //arrange.
            var featureInstance = new FeatureWithDocStringScenarioStep();

            //act.
            var sut = StepMethodInfo.FromMethodInfo(
                featureInstance.GetType().GetMethod(nameof(FeatureWithDocStringScenarioStep.Step_With_DocString_Argument)),
                featureInstance
                );

            //assert.
            Assert.NotNull(sut);
        }

        private sealed class FeatureWithDocStringScenarioStep : Feature
        {
            public Gherkin.Ast.DocString ReceivedDocString { get; private set; }

            public const string StepWithDocStringText = "Step with docstirng";

            [Given(StepWithDocStringText)]
            public void Step_With_DocString_Argument(Gherkin.Ast.DocString docString)
            {
                ReceivedDocString = docString;
            }
        }

        [Fact]
        public void DigestScenarioStepValues_Sets_DocString_Value()
        {
            //arrange.
            var featureInstance = new FeatureWithDocStringScenarioStep();
            var sut = StepMethodInfo.FromMethodInfo(
                featureInstance.GetType().GetMethod(nameof(FeatureWithDocStringScenarioStep.Step_With_DocString_Argument)),
                featureInstance
                );

            var scenarioName = "scenario ajshas a";
            var docStringContent = @"some content
+++
with multi lines
---
in it";

            var step = CreateGherkinDocument(scenarioName,
                "Given " + FeatureWithDocStringScenarioStep.StepWithDocStringText + @"
" + @"""""""
" + docStringContent + @"
""""""").Feature.Children.First().Steps.First();

            //act.
            sut.DigestScenarioStepValues(step);

            //assert.
            var digestedText = sut.GetDigestedStepText();
            Assert.Equal(FeatureWithDocStringScenarioStep.StepWithDocStringText, digestedText);
        }
    }
}
