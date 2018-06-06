using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class PrimitiveTypeArgumentTests
    {
        private static ParameterInfo GetParamAt(int index)
        {
            return typeof(PrimitiveTypeArgumentTests).GetMethod(nameof(MethodWithParameters), BindingFlags.NonPublic | BindingFlags.Instance)
                .GetParameters()[index];
        }

        private void MethodWithParameters(int numberParam, string literalParam, DateTime dateParam) { }

        [Fact]
        public void Clone_Creates_Similar_Instance()
        {
            //arrange.
            var sut = new PrimitiveTypeArgument(GetParamAt(0), 123);

            //act.
            var clone = sut.Clone();

            //assert.
            Assert.True(clone.IsSameAs(sut));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void DigestScenarioStepValues_Takes_Value_By_Index(int index)
        {
            //arrange.
            var sut = new PrimitiveTypeArgument(GetParamAt(index), index);

            var arguments = new dynamic[] { 123, "Ana", new DateTime(2018, 5, 23) };
            var argumentsAsString = new string[] { $"{arguments[0]}", $"{arguments[1]}", $"{arguments[2].Month}/{arguments[2].Day}/{arguments[2].Year}" };
            var step = CreateGherkinDocument("some scenario", $@"Then I should have {argumentsAsString[0]} apples from {argumentsAsString[1]} by {argumentsAsString[2]}")
                .Feature.Children.First().Steps.First();

            //act.
            sut.DigestScenarioStepValues(argumentsAsString, step.Argument);

            //assert.
            Assert.Equal(arguments[index], sut.Value);
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
    }
}
