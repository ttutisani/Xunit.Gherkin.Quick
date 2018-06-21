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
        public void ListFromMethodInfo_Creates_Empty_From_Empty()
        {
            //arrange.
            var method = GetPrivateMethod(nameof(MethodWithoutParams));

            //act.
            var args = StepMethodArgument.ListFromMethodInfo(method);

            //assert.
            Assert.NotNull(args);
            Assert.Empty(args);
        }

        private static MethodInfo GetPrivateMethod(string name)
        {
            return typeof(StepMethodArgumentTests).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private void MethodWithoutParams()
        { }

        [Fact]
        public void ListFromParameters_Creates_PrimitiveTypeArguments()
        {
            //arrange.
            var method = GetPrivateMethod(nameof(MethodWithPrimitiveParams));

            //act.
            var args = StepMethodArgument.ListFromMethodInfo(method);

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
            }
        }

        [Fact]
        public void IsSameAs_Identifies_Similar_Instances()
        {
            //arrange.
            var method = GetPrivateMethod(nameof(MethodWithPrimitiveParams));
            var param0 = StepMethodArgument.ListFromMethodInfo(method)[0];
            var param1 = StepMethodArgument.ListFromMethodInfo(method)[0];

            //act.
            var same = param0.IsSameAs(param1) && param1.IsSameAs(param0);

            //assert.
            Assert.True(same);
        }

        [Fact]
        public void IsSameAs_Distinguishes_Different_Instances()
        {
            //arrange.
            var method = GetPrivateMethod(nameof(MethodWithPrimitiveParams));
            var param0 = StepMethodArgument.ListFromMethodInfo(method)[0];
            var param1 = StepMethodArgument.ListFromMethodInfo(method)[1];

            //act.
            var same = param0.IsSameAs(param1) && param1.IsSameAs(param0);

            //assert.
            Assert.False(same);
        }

        [Fact]
        public void Clone_Creates_Similar_Instance()
        {
            //arrange.
            var method = GetPrivateMethod(nameof(MethodWithPrimitiveParams));
            var sut = StepMethodArgument.ListFromMethodInfo(method)[0];

            //act.
            var clone = sut.Clone();

            //assert.
            Assert.NotNull(clone);
            Assert.NotSame(sut, clone);
            Assert.True(clone.IsSameAs(sut));
        }

        private void MethodWithPrimitiveParams(int param1, string param2, DateTime param3)
        { }

        [Fact]
        public void ListFromParameters_Creates_DataTableArgument()
        {
            //arrange / act.
            var args = StepMethodArgument.ListFromMethodInfo(GetPrivateMethod(nameof(MethodWithDataTableArgumentOnly)));

            //assert.
            Assert.NotNull(args);
            Assert.Single(args);
            Assert.NotNull(args[0]);
            Assert.IsType<DataTableArgument>(args[0]);
        }

        private void MethodWithDataTableArgumentOnly(Gherkin.Ast.DataTable dataTable)
        { }

        [Fact]
        public void ListFromParameters_Creates_DataTableArgument_And_Others()
        {
            //arrange / act.
            var args = StepMethodArgument.ListFromMethodInfo(GetPrivateMethod(nameof(MethodWithDataTableAndOtherArguments)));

            //assert.
            Assert.NotNull(args);
            Assert.Equal(4, args.Count);

            AssertPrimitiveTypeArg(args, 0);
            AssertPrimitiveTypeArg(args, 1);
            AssertPrimitiveTypeArg(args, 2);
            Assert.IsType<DataTableArgument>(args[3]);

            void AssertPrimitiveTypeArg(List<StepMethodArgument> arg, int index)
            {
                Assert.NotNull(arg[index]);
                Assert.IsType<PrimitiveTypeArgument>(arg[index]);
            }

        }

        private void MethodWithDataTableAndOtherArguments(int param1, string param2, DateTime param3, Gherkin.Ast.DataTable dataTable)
        { }

        [Fact]
        public void ListFromParameters_Creates_DocStringArgument()
        {
            //arrange / act.
            var args = StepMethodArgument.ListFromMethodInfo(GetPrivateMethod(nameof(MethodWithDocStringArgumentOnly)));

            //assert.
            Assert.NotNull(args);
            Assert.Single(args);
            Assert.NotNull(args[0]);
            Assert.IsType<DocStringArgument>(args[0]);
        }

        private void MethodWithDocStringArgumentOnly(Gherkin.Ast.DocString docString)
        { }

        [Fact]
        public void ListFromParameters_Creates_DocStringArgument_And_Others()
        {
            //arrange / act.
            var args = StepMethodArgument.ListFromMethodInfo(GetPrivateMethod(nameof(MethodWithDocStringAndOtherArguments)));

            //assert.
            Assert.NotNull(args);
            Assert.Equal(4, args.Count);

            AssertPrimitiveTypeArg(args, 0);
            AssertPrimitiveTypeArg(args, 1);
            AssertPrimitiveTypeArg(args, 2);
            Assert.IsType<DocStringArgument>(args[3]);

            void AssertPrimitiveTypeArg(List<StepMethodArgument> arg, int index)
            {
                Assert.NotNull(arg[index]);
                Assert.IsType<PrimitiveTypeArgument>(arg[index]);
            }

        }

        private void MethodWithDocStringAndOtherArguments(int param1, string param2, DateTime param3, Gherkin.Ast.DocString docString)
        { }
    }
}
