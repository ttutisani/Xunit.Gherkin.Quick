using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class PrimitiveTypeArgumentTests
    {
        public PrimitiveTypeArgumentTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

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
            var step = new Gherkin.Ast.Step(
                null,
                "Then",
                $@"I should have {argumentsAsString[0]} apples from {argumentsAsString[1]} by {argumentsAsString[2]}",
                null);

            //act.
            sut.DigestScenarioStepValues(argumentsAsString, step.Argument);

            //assert.
            Assert.Equal(arguments[index], sut.Value);
        }
    }
}
