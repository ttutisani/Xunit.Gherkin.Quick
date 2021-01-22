using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Xunit.Gherkin.Quick
{
    internal sealed class MethodInfoWrapper
    {
        private readonly MethodInfo _methodInfo;
        private readonly object _target;

        private MethodInfoWrapper(MethodInfo methodInfo, object target)
        {
            _methodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public static MethodInfoWrapper FromMethodInfo(MethodInfo methodInfo, object target)
        {
            if (IsAsyncMethod(methodInfo) && methodInfo.ReturnType == typeof(void))
            {
                throw new InvalidOperationException($"Method `{methodInfo.Name}` of `{methodInfo.DeclaringType.Name}` class is async and void, which looks like a mistake. Use either async with Task or void without async.");
            }

            return new MethodInfoWrapper(methodInfo, target);
        }

        private static bool IsAsyncMethod(MethodInfo method)
        {
            Type attType = typeof(AsyncStateMachineAttribute);
            var attrib = (AsyncStateMachineAttribute)method.GetCustomAttribute(attType);

            return (attrib != null);
        }

        public async Task InvokeMethodAsync(object[] parameters)
        {
            var result = _methodInfo.Invoke(_target, parameters);
            if (result is Task resultAsTask)
                await resultAsTask;
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
