using System;
using System.ComponentModel;
using Gherkin.Ast;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.TestScenarios
{
    /// <summary>
    /// Represents a test step based on a <see cref="Step"/> normalized across translations.
    /// </summary>
    public class TestStep : IXunitSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestStep"/> class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStep"/> class.
        /// </summary>
        internal TestStep(TestStepType section, string text)
        {
            Type = section;
            Text = text;

            DocStringArgument = null;
            TableArgument = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStep"/> class.
        /// </summary>
        internal TestStep(TestStepType section, string text, TestStepDocStringArgument docStringArgument)
        {
            Type = section;
            Text = text;

            DocStringArgument = docStringArgument;
            TableArgument = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStep"/> class.
        /// </summary>
        internal TestStep(TestStepType section, string text, TestStepTableArgument tableArgument)
        {
            Type = section;
            Text = text;

            DocStringArgument = null;
            TableArgument = tableArgument;
        }

        /// <summary>
        /// Gets the test step type corresponding to the keyword in the feature file.
        /// </summary>
        public TestStepType Type { get; private set; }

        /// <summary>
        /// Gets the step text as specified in the feature file.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the doc string argument as specified in the feature file, if pressent; <c>null</c> otherwise.
        /// </summary>
        public TestStepDocStringArgument DocStringArgument { get; private set; }

        /// <summary>
        /// Gets the data table argument as specified in the feature file, if pressent; <c>null</c> otherwise.
        /// </summary>
        public TestStepTableArgument TableArgument { get; private set; }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Type), Type, typeof(TestStepType));
            info.AddValue(nameof(Text), Text, typeof(string));
            info.AddValue(nameof(DocStringArgument), DocStringArgument, typeof(TestStepDocStringArgument));
            info.AddValue(nameof(TableArgument), TableArgument, typeof(TestStepTableArgument));
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            Type = info.GetValue<TestStepType>(nameof(Type));
            Text = info.GetValue<string>(nameof(Text));
            DocStringArgument = info.GetValue<TestStepDocStringArgument>(nameof(DocStringArgument));
            TableArgument = info.GetValue<TestStepTableArgument>(nameof(TableArgument));
        }
    }
}