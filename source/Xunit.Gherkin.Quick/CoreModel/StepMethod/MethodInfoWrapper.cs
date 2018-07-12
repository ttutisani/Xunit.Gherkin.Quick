using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Xunit.Gherkin.Quick
{
    internal sealed class MethodInfoWrapper
    {
        private readonly MethodInfo _methodInfo;
        private readonly object _target;

        public MethodInfoWrapper(MethodInfo methodInfo, object target)
        {
            _methodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public async Task InvokeMethodAsync(object[] parameters)
        {
            var featureTarget = _target as Feature;

            await featureTarget.InvokeAsync(BeforeAfterAttributeHelper.InvokeMethodType.BeforeStep);
            var result = _methodInfo.Invoke(_target, parameters);
            if (result is Task resultAsTask)
                await resultAsTask;
            await featureTarget.InvokeAsync(BeforeAfterAttributeHelper.InvokeMethodType.AfterStep);
        }

        public bool IsSameAs(MethodInfoWrapper other)
        {
            if (this == other)
                return true;

            return other != null
                && other._methodInfo.Equals(_methodInfo)
                && other._target == _target;
        }

        public string GetMethodName()
        {
            return _methodInfo.Name;
        }
    }
}
