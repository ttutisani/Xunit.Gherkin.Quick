using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.TestScenarios
{
    internal class TestScenario : IXunitSerializable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestScenario()
        {
        }

        internal TestScenario(string featureName, string scenarioName, CultureInfo locale, string[] tags, TestStep[] steps)
        {
            FeatureName = featureName;
            ScenarioName = scenarioName;
            Locale = locale;
            Tags = tags ?? Array.Empty<string>();
            Steps = steps ?? Array.Empty<TestStep>();
        }

        public string FeatureName { get; private set; }
        public string ScenarioName { get; private set; }
        public CultureInfo Locale { get; private set; }
        public IReadOnlyCollection<string> Tags { get; private set; }
        public IReadOnlyList<TestStep> Steps { get; private set; }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(FeatureName), FeatureName, typeof(string));
            info.AddValue(nameof(ScenarioName), ScenarioName, typeof(string));
            info.AddValue(nameof(Locale), Locale.Name, typeof(string));
            info.AddValue(nameof(Tags), Tags as string[] ?? Tags.ToArray(), typeof(string[]));
            info.AddValue(nameof(Steps), Steps as TestStep[] ?? Steps.ToArray(), typeof(TestStep[]));
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            FeatureName = info.GetValue<string>(nameof(FeatureName));
            ScenarioName = info.GetValue<string>(nameof(ScenarioName));
            Locale = new CultureInfo(info.GetValue<string>(nameof(Locale)));
            Tags = info.GetValue<string[]>(nameof(Tags)) ?? Array.Empty<string>();
            Steps = info.GetValue<TestStep[]>(nameof(Steps)) ?? Array.Empty<TestStep>();
        }
    }
}