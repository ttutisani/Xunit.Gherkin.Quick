using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class StepMethodTests
    {
        [Fact]
        public void Ctor_Initializes_Properties()
        {
            //arrange.
            var featureInstance = new FeatureForCtorTest();

            //act.
            var sut = StepMethod.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForCtorTest.When_Something)), featureInstance);

            //assert.
            Assert.Equal(StepMethodKind.When, sut.Kind);
            Assert.Equal(FeatureForCtorTest.WhenStepText, sut.Pattern);
            Assert.NotNull(sut.Arguments);
            Assert.Empty(sut.Arguments);
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
            var sut = StepMethod.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForCtorTest.When_Something)), featureInstance);

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
            var sut = StepMethod.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForCtorTest.When_Something)), featureInstance);
            var clone = StepMethod.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForCtorTest.When_Something)), featureInstance);

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
            var sut = StepMethod.FromMethodInfo(
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
        public void DigestScenarioStepValues_Expects_Exact_Number_Of_Groups_Not_More()
        {
            //arrange.
            var featureInstance = new FeatureForApplyArgumentValues_MoreThanNeededGroups();
            var sut = StepMethod.FromMethodInfo(
                featureInstance.GetType().GetMethod(nameof(FeatureForApplyArgumentValues_MoreThanNeededGroups.Method_With_Arguments)),
                featureInstance);

            var number = 123;
            var text = "Ana";
            var date = new DateTime(2018, 5, 23);
            var stepText = FeatureForApplyArgumentValues_MoreThanNeededGroups.StepMethodText
                .Replace(@"(\d+)", $"{number}")
                .Replace(@"(\w+)", $"{text}")
                .Replace(@"([\d/]+)", $"{date.Month}/{date.Day}/{date.Year}");

            var step = CreateGherkinDocument("some scenario", "Then " + stepText)
                .Feature.Children.First().Steps.First();

            //act / assert.
            Assert.Throws<InvalidOperationException>(() => sut.DigestScenarioStepValues(step));
        }

        private sealed class FeatureForApplyArgumentValues_MoreThanNeededGroups : Feature
        {
            public const string StepMethodText = @"I should have (\d+) apples from (\w+) by ([\d/]+)";

            [Then(StepMethodText)]
            public void Method_With_Arguments(int number, string text)
            { }
        }

        [Fact]
        public void DigestScenarioStepValues_Sets_Primitive_Values()
        {
            //arrange.
            var featureInstance = new FeatureForApplyArgumentValues();
            var sut = StepMethod.FromMethodInfo(
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
            Assert.NotNull(sut.Arguments);
            Assert.Equal(3, sut.Arguments.Count);
            Assert.Equal(number, sut.Arguments[0].Value);
            Assert.Equal(text, sut.Arguments[1].Value);
            Assert.Equal(date, sut.Arguments[2].Value);
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
        public void Execute_Invokes_StepMethod()
        {
            //arrange.
            var featureInstance = new FeatureForExecuteTest();
            var sut = StepMethod.FromMethodInfo(featureInstance.GetType().GetMethod(nameof(FeatureForExecuteTest.Call_This_Method)), featureInstance);

            //act.
            sut.Execute();

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
    }
}
