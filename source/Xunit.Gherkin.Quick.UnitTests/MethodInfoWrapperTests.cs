using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class MethodInfoWrapperTests
    {
        [Fact]
        public void IsSameAs_Identifies_Similar_Instances()
        {
            //arrange.
            var target = new ClassWithMethod();
            var sut = new MethodInfoWrapper(target.GetType().GetMethod(nameof(ClassWithMethod.MethodToCall)), target);
            var other = new MethodInfoWrapper(target.GetType().GetMethod(nameof(ClassWithMethod.MethodToCall)), target);

            //act.
            var same = sut.IsSameAs(other) && other.IsSameAs(sut);

            //assert.
            Assert.True(same);
        }
        
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
