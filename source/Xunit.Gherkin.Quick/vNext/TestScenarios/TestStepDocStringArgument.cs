using System;
using System.ComponentModel;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.vNext.TestScenarios
{
    internal class TestStepDocStringArgument : IXunitSerializable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestStepDocStringArgument()
        {
        }

        internal TestStepDocStringArgument(string content, string contentType, TestStepArgumentLocation location)
        {
            Content = content;
            ContentType = contentType;
            Location = location;
        }

        public string Content { get; private set; }
        public string ContentType { get; private set; }
        internal TestStepArgumentLocation Location { get; set; }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Content), Content, typeof(string));
            info.AddValue(nameof(ContentType), ContentType, typeof(string));
            info.AddValue(nameof(Location), Location, typeof(TestStepArgumentLocation));
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            Content = info.GetValue<string>(nameof(Content));
            ContentType = info.GetValue<string>(nameof(ContentType));
            Location = info.GetValue<TestStepArgumentLocation>(nameof(Location));
        }
    }
}