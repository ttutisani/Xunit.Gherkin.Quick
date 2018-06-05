using System;
using System.Collections.Generic;
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

            AssertPrimitiveTypeArg(args, 0);
            AssertPrimitiveTypeArg(args, 1);
            AssertPrimitiveTypeArg(args, 2);

            void AssertPrimitiveTypeArg(List<StepMethodArgument> arg, int index)
            {
                Assert.NotNull(arg[index]);
                Assert.IsType<PrimitiveTypeArgument>(arg[index]);
                Assert.Equal(index, (arg[index] as PrimitiveTypeArgument).Index);
            }
        }

        [Fact]
        public void IsSameAs_Identifies_Similar_Instances()
        {
            //arrange.
            var param0 = StepMethodArgument.ListFromParameters(new ParameterInfo[]
            {
                GetParameterAt(0)
            })[0];
            var param1 = StepMethodArgument.ListFromParameters(new ParameterInfo[]
            {
                GetParameterAt(0)
            })[0];

            //act.
            var same = param0.IsSameAs(param1) && param1.IsSameAs(param0);

            //assert.
            Assert.True(same);
        }

        [Fact]
        public void IsSameAs_Distinguishes_Different_Instances()
        {
            //arrange.
            var param0 = StepMethodArgument.ListFromParameters(new ParameterInfo[]
            {
                GetParameterAt(0)
            })[0];
            var param1 = StepMethodArgument.ListFromParameters(new ParameterInfo[]
            {
                GetParameterAt(1)
            })[0];

            //act.
            var same = param0.IsSameAs(param1) && param1.IsSameAs(param0);

            //assert.
            Assert.False(same);
        }

        [Fact]
        public void Clone_Creates_Similar_Instance()
        {
            //arrange.
            var sut = StepMethodArgument.ListFromParameters(new ParameterInfo[] { GetParameterAt(0) })[0];

            //act.
            var clone = sut.Clone();

            //assert.
            Assert.NotNull(clone);
            Assert.NotSame(sut, clone);
            Assert.True(clone.IsSameAs(sut));
        }

        private static ParameterInfo GetParameterAt(int index)
        {
            return typeof(StepMethodArgumentTests).GetMethod(nameof(MethodWithPrimitiveParams), BindingFlags.NonPublic | BindingFlags.Instance).GetParameters()[index];
        }

        private void MethodWithPrimitiveParams(int param1, string param2, DateTime param3)
        { }
    }
}
