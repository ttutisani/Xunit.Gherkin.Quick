using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Xunit.Gherkin.Quick
{
    public static class BeforeAfterAttributeHelper
    {
        public enum InvokeMethodType
        {
            None,
            BeforeFeature,
            AfterFeature,
            BeforeScenario,
            AfterScenario,
            BeforeStep,
            AfterStep,
        }

        private static Dictionary<InvokeMethodType, Type> attributeMapping = new Dictionary<InvokeMethodType, Type> {

            { InvokeMethodType.BeforeFeature, typeof(BeforeFeatureAttribute) },
            { InvokeMethodType.AfterFeature, typeof(AfterFeatureAttribute) },
            { InvokeMethodType.BeforeScenario, typeof(BeforeScenarioAttribute) },
            { InvokeMethodType.AfterScenario, typeof(AfterScenarioAttribute) },
            { InvokeMethodType.BeforeStep, typeof(BeforeStepAttribute) },
            { InvokeMethodType.AfterStep, typeof(AfterStepAttribute) },
        };        


        public static async Task InvokeAsync(this Feature featureInstance, InvokeMethodType attributetype)
        {
            Type invokeAttribute;
            if (attributeMapping.TryGetValue(attributetype, out invokeAttribute) == false)
                return;

            if (featureInstance == null)
                return;

            var methods = featureInstance.GetType().GetTypeInfo().GetMethods().Where(m => m.IsDefined(invokeAttribute));
            foreach (var f in methods)
            {
                var result = f.Invoke(featureInstance, null);
                if (result is Task resultAsTask)
                    await resultAsTask;
            }
        }
    }
}
