using System;
using System.Threading;
using System.Threading.Tasks;
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
            var sut = MethodInfoWrapper.FromMethodInfo(target.GetType().GetMethod(nameof(ClassWithMethod.MethodToCall)), target);
            var other = MethodInfoWrapper.FromMethodInfo(target.GetType().GetMethod(nameof(ClassWithMethod.MethodToCall)), target);

            //act.
            var same = sut.IsSameAs(other) && other.IsSameAs(sut);

            //assert.
            Assert.True(same);
        }

        [Fact]
        public void GetMethodName_Returns_Wrapped_Method_Name()
        {
            //arrange.
            var target = new ClassWithMethod();
            var sut = MethodInfoWrapper.FromMethodInfo(target.GetType().GetMethod(nameof(ClassWithMethod.MethodToCall)), target);

            //act.
            var name = sut.GetMethodName();

            //assert.
            Assert.Equal(nameof(ClassWithMethod.MethodToCall), name);
        }
        
        [Fact]
        public async Task InvokeMethod_Invokes_Underlying_Method()
        {
            //arrange.
            var target = new ClassWithMethod();
            var sut = MethodInfoWrapper.FromMethodInfo(target.GetType().GetMethod(nameof(ClassWithMethod.MethodToCall)), target);

            //act.
            await sut.InvokeMethodAsync(null);

            //assert.
            Assert.True(target.Called);
        }

        [Fact]
        public async Task InvokeMethod_Invokes_Underlying_Async_Method()
        {
            //arrange.
            var target = new ClassWithMethod();
            var sut = MethodInfoWrapper.FromMethodInfo(target.GetType().GetMethod(nameof(ClassWithMethod.MethodToCallAsync)), target);

            //act.
            await sut.InvokeMethodAsync(null);

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

            public async Task MethodToCallAsync()
            {
                await Task.Run(() =>
                {
                    Thread.Sleep(100); //intentional delay - to imitate truly async operation.
                    Called = true;
                });
            }
        }

        [Fact]
        public void FromMethodInfo_DoesNotAllow_AsyncVoid_Steps()
        {
            //arrange.
            var featureInstance = new FeatureWithAsyncVoidStep();
            var methodInfoWithAsyncVoid = typeof(FeatureWithAsyncVoidStep).GetMethod(nameof(FeatureWithAsyncVoidStep.StepWithAsyncVoid));

            //act / assert.
            Assert.Throws<InvalidOperationException>(() => MethodInfoWrapper.FromMethodInfo(methodInfoWithAsyncVoid, featureInstance));
        }

        private sealed class FeatureWithAsyncVoidStep : Feature
        {
            [Given("Any step text")]
            public async void StepWithAsyncVoid()
            {
                await Task.CompletedTask;
            }
        }
    }
}
