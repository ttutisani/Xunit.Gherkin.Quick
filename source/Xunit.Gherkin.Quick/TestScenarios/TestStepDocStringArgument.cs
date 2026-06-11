using System;
using System.ComponentModel;
using Gherkin.Ast;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.TestScenarios
{
    /// <summary>
    /// Represents a multi-line string step argument based on a <see cref="DocString"/>.
    /// </summary>
    public class TestStepDocStringArgument : IXunitSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepDocStringArgument"/> class.
        /// </summary>
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

        /// <summary>
        /// Gets the doc string content as specified in the feature file.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Gets the content type as specified in the feature file.
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Used internally to accurately recreate <see cref="DocString"/>s for scenario step methods.
        /// </summary>
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