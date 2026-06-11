using System;
using System.ComponentModel;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.TestScenarios
{
    internal class TestStep : IXunitSerializable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestStep()
        {
        }

        internal TestStep(TestStepType section, string text)
        {
            Type = section;
            Text = text;

            DocStringArgument = null;
            TableArgument = null;
        }

        internal TestStep(TestStepType section, string text, TestStepDocStringArgument docStringArgument)
        {
            Type = section;
            Text = text;

            DocStringArgument = docStringArgument;
            TableArgument = null;
        }

        internal TestStep(TestStepType section, string text, TestStepTableArgument tableArgument)
        {
            Type = section;
            Text = text;

            DocStringArgument = null;
            TableArgument = tableArgument;
        }

        public TestStepType Type { get; private set; }
        public string Text { get; private set; }
        public TestStepDocStringArgument DocStringArgument { get; private set; }
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