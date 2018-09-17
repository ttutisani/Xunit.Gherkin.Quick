using System;
using System.Collections.Generic;
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

        public async Task InvokeMethodAsync(Dictionary<string, object> scenarioContext, object[] parameters)
        {
			if(_target is StepContainer contextualTarget)
			{
				contextualTarget.ScenarioContext = scenarioContext;
			}

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
