using System;
using System.Reflection;

namespace Xunit.Gherkin.Quick
{
    //value object.
    internal sealed class MethodInfoWrapper
    {
        private readonly MethodInfo _methodInfo;
        private readonly object _target;

        public MethodInfoWrapper(MethodInfo methodInfo, object target)
        {
            _methodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public void InvokeMethod(object[] parameters)
        {
            _methodInfo.Invoke(_target, parameters);
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
