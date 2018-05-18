using System.Collections.Generic;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class FeatureClassTests
    {
        [Fact]
        public void Ctor_Initializes_Properties()
        {
            //arrange.
            var featureFilePath = "some path 123";
            var stepMethods = new List<StepMethod>
            {
                new StepMethod(StepMethodKind.Given, "some text 123")
            };

            //act.
            var sut = new FeatureClass(featureFilePath, stepMethods.AsReadOnly());

            //assert.
            Assert.Equal(featureFilePath, sut.FeatureFilePath);
            Assert.NotNull(sut.StepMethods);
            Assert.Single(sut.StepMethods);
            Assert.Same(stepMethods[0], sut.StepMethods[0]);
        }

        [Fact]
        public void FromFeatureInstance_Creates_FeatureClass_With_Default_FilePath_If_No_Attribute()
        {
            //arrange.
            var featureInstance = new FeatureWithoutFilePath();

            //act.
            var sut = FeatureClass.FromFeatureInstance(featureInstance);

            //assert.
            Assert.NotNull(sut);
            Assert.Equal($"{nameof(FeatureWithoutFilePath)}.feature", sut.FeatureFilePath);
            Assert.Empty(sut.StepMethods);
        }

        private sealed class FeatureWithoutFilePath : Feature
        {
        }

        [Fact]
        public void FromFeatureInstance_Creates_FeatureClass_With_FilePath_From_Attribute()
        {
            //arrange.
            var featureInstance = new FeatureWithFilePath();

            //act.
            var sut = FeatureClass.FromFeatureInstance(featureInstance);

            //assert.
            Assert.NotNull(sut);
            Assert.Equal(FeatureWithFilePath.PathFor_FeatureWithFilePath, sut.FeatureFilePath);
            Assert.Empty(sut.StepMethods);
        }

        [FeatureFile(PathFor_FeatureWithFilePath)]
        private sealed class FeatureWithFilePath : Feature
        {
            public const string PathFor_FeatureWithFilePath = "some file path const 123";
        }

        [Fact]
        public void FromFeatureInstance_Creates_FeatureClass_With_StepMethods()
        {
            //arrange.
            var featureInstance = new FeatureWithStepMethods();

            //act.
            var sut = FeatureClass.FromFeatureInstance(featureInstance);

            //assert.
            Assert.NotNull(sut);
            Assert.NotNull(sut.StepMethods);
            Assert.Equal(5, sut.StepMethods.Count);

            Assert.Contains(sut.StepMethods, sm => sm.Kind == StepMethodKind.Given && sm.Text == FeatureWithStepMethods.GivenStepText);
            Assert.Contains(sut.StepMethods, sm => sm.Kind == StepMethodKind.When && sm.Text == FeatureWithStepMethods.WhenStepText);
            Assert.Contains(sut.StepMethods, sm => sm.Kind == StepMethodKind.Then && sm.Text == FeatureWithStepMethods.ThenStepText);
            Assert.Contains(sut.StepMethods, sm => sm.Kind == StepMethodKind.And && sm.Text == FeatureWithStepMethods.AndStepText);
            Assert.Contains(sut.StepMethods, sm => sm.Kind == StepMethodKind.But && sm.Text == FeatureWithStepMethods.ButStepText);
        }

        private sealed class FeatureWithStepMethods : Feature
        {
            public const string GivenStepText = "step1";

            [Given(GivenStepText)]
            public void GivenStepMethod111()
            {

            }

            public const string WhenStepText = "step2 when";

            [When(WhenStepText)]
            public void WhenStepMethod123()
            {

            }

            public const string ThenStepText = "then step3";

            [Then(ThenStepText)]
            public void ThenStepMethod432()
            {

            }

            public const string AndStepText = "and and 123";

            [And(AndStepText)]
            public void AndStepMethod512()
            {

            }

            public const string ButStepText = "but 123 but";

            [But(ButStepText)]
            public void ButStepMethod333()
            {

            }
        }
    }
}
