using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class MethodWrapperTests
    {
        [Fact]
        public void InvokeMethod_Invokes_Underlying_Method()
        {
            //arrange.
            var target = new ClassWithMethod();
            var sut = new MethodInfoWrapper(target.GetType().GetMethod(nameof(ClassWithMethod.MethodToCall)), target);

            //act.
            sut.InvokeMethod(null);

            //assert.
            Assert.True(target.Called);
        }

        private sealed class ClassWithMethod
        {
            public bool Called { get; private set; } = false;

            public void MethodToCall()
            {
                Called = true;
            }
        }
    }
}
