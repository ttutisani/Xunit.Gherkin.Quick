using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Gherkin.Ast;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.TestScenarios
{
    /// <summary>
    /// Represents a test scenario based on a <see cref="Scenario"/> or <see cref="ScenarioOutline"/>
    /// normalized across translations.
    /// </summary>
    public class TestScenario : IXunitSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestScenario"/> class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestScenario()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestScenario"/> class.
        /// </summary>
        internal TestScenario(string featureName, string scenarioName, CultureInfo locale, string[] tags, TestStep[] steps)
        {
            FeatureName = featureName;
            ScenarioName = scenarioName;
            Locale = locale;
            Tags = tags ?? Array.Empty<string>();
            Steps = steps ?? Array.Empty<TestStep>();
        }

        /// <summary>
        /// Gets the feature name the scenario belongs to as specified in the feature file.
        /// </summary>
        public string FeatureName { get; private set; }

        /// <summary>
        /// Gets the scenario name as specified in the feature file.
        /// </summary>
        public string ScenarioName { get; private set; }

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> for the specified language in the feature file.
        /// </summary>
        public CultureInfo Locale { get; private set; }

        /// <summary>
        /// Gets the combined tags from the <see cref="Feature"/>, <see cref="Scenario"/>,
        /// <see cref="ScenarioOutline"/> and <see cref="Examples"/> to which the current
        /// instance corresponds to.
        /// </summary>
        /// <remarks>
        /// Duplicate tags are removed, the tags retain their order in which they first appear
        /// in. Uniqueness is case-insensitive.
        /// </remarks>
        public IReadOnlyCollection<string> Tags { get; private set; }

        /// <summary>
        /// Gets the <see cref="TestStep"/>s describing the scenario, including any preceeding
        /// steps part of a <see cref="Background"/> definition.
        /// </summary>
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