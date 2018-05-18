using System;
using System.Reflection;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class StepMethodArgumentTests
    {
        [Fact]
        public void ListFromParameters_Creates_Empty_From_Empty()
        {
            //arrange.
            var @params = new ParameterInfo[0];

            //act.
            var args = StepMethodArgument.ListFromParameters(@params);

            //assert.
            Assert.NotNull(args);
            Assert.Empty(args);
        }

        [Fact]
        public void ListFromParameters_Creates_PrimitiveTypeArguments()
        {
            //arrange.
            var @params = new ParameterInfo[] 
            {
                GetParameterAt(0),
                GetParameterAt(1),
                GetParameterAt(2)
            };

            //act.
            var args = StepMethodArgument.ListFromParameters(@params);

            //assert.
            Assert.NotNull(args);
            Assert.Equal(3, args.Count);

            AssertPrimitiveTypeArg(args[0]);
            AssertPrimitiveTypeArg(args[1]);
            AssertPrimitiveTypeArg(args[2]);

            void AssertPrimitiveTypeArg(StepMethodArgument arg)
            {
                Assert.NotNull(arg);
                Assert.IsType<PrimitiveTypeArgument>(arg);
            }
        }

        private static ParameterInfo GetParameterAt(int index)
        {
            return typeof(StepMethodArgumentTests).GetMethod(nameof(MethodWithPrimitiveParams), BindingFlags.NonPublic | BindingFlags.Instance).GetParameters()[index];
        }

        private void MethodWithPrimitiveParams(int param1, string param2, DateTime param3)
        { }
    }
}
