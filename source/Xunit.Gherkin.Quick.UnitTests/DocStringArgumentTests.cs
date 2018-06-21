using Moq;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class DocStringArgumentTests
    {
        [Fact]
        public void DigestScenarioStepValues_Throws_Error_If_No_Arguments_And_No_DocString()
        {
            //arrange.
            var sut = new DocStringArgument();

            //act / assert.
            Assert.Throws<InvalidOperationException>(() => sut.DigestScenarioStepValues(new string[0], null));
        }

        [Fact]
        public void DigestScenarioStepValues_Throws_Error_If_Arguments_Present_But_No_DocString()
        {
            //arrange.
            var sut = new DocStringArgument();

            //act / assert.
            Assert.Throws<InvalidOperationException>(() => sut.DigestScenarioStepValues(new string[] { "1", "2", "3" }, null));
        }

        [Fact]
        public void DigestScenarioStepValues_Sets_Value_As_DocString_When_Only_DocString()
        {
            //arrange.
            var sut = new DocStringArgument();
            var scenarioName = "scenario-121kh2";
            var docStringContent = @"some content
+++
with multi lines
---
in it";
            var docString = CreateGherkinDocument(scenarioName,
                @"Given some step text
" + @"""""""
" + docStringContent + @"
""""""").Feature.Children.ElementAt(0).Steps.ElementAt(0).Argument as Gherkin.Ast.DocString;

            //act.
            sut.DigestScenarioStepValues(new string[0], docString);

            //assert.
            Assert.Same(docString, sut.Value);
        }

        [Fact]
        public void DigestScenarioStepValues_Sets_Value_As_DataTable_When_DataTable_And_Other_Args_Present()
        {
            //arrange.
            var sut = new DocStringArgument();
            var scenarioName = "scenario-121kh2";
            var docStringContent = @"some content
+++
with multi lines
---
in it";
            var docString = CreateGherkinDocument(scenarioName,
                @"Given some step text
" + @"""""""
" + docStringContent + @"
""""""").Feature.Children.ElementAt(0).Steps.ElementAt(0).Argument as Gherkin.Ast.DocString;

            //act.
            sut.DigestScenarioStepValues(new string[] { "1", "2", "3" }, docString);

            //assert.
            Assert.Same(docString, sut.Value);
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

        [Fact]
        public void IsSameAs_Identifies_Similar_Instances()
        {
            //arrange.
            var sut = new DocStringArgument();
            var other = new DocStringArgument();

            //act.
            var same = sut.IsSameAs(other);

            //assert.
            Assert.True(same);
        }

        [Fact]
        public void IsSameAs_Distinguishes_Different_Instances()
        {
            //arrange.
            var sut = new DocStringArgument();
            var other = new Mock<StepMethodArgument>().Object;

            //act.
            var same = sut.IsSameAs(other);

            //assert.
            Assert.False(same);
        }

        [Fact]
        public void Clone_Creates_Similar_Instance()
        {
            //arrange.
            var sut = new DocStringArgument();

            //act.
            var clone = sut.Clone();

            //assert.
            Assert.True(clone.IsSameAs(sut));
            Assert.NotSame(clone, sut);
        }
    }
}
