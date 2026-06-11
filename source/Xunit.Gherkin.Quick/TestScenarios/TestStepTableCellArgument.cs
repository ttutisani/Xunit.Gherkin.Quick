using System;
using System.ComponentModel;
using Gherkin.Ast;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.TestScenarios
{
    /// <summary>
    /// Represents a cell for a table step argument based on a <see cref="TableCell"/>.
    /// </summary>
    public class TestStepTableCellArgument : IXunitSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepTableCellArgument"/> class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestStepTableCellArgument()
        {
        }

        internal TestStepTableCellArgument(string value, TestStepArgumentLocation location)
        {
            Value = value;
            Location = location;
        }

        /// <summary>
        /// Gets the cell value as specified in the feature file.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Used internally to accurately recreate <see cref="TableCell"/>s for scenario step methods.
        /// </summary>
        internal TestStepArgumentLocation Location { get; private set; }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Value), Value, typeof(string));
            info.AddValue(nameof(Location), Location, typeof(TestStepArgumentLocation));
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            Value = info.GetValue<string>(nameof(Value));
            Location = info.GetValue<TestStepArgumentLocation>(nameof(Location));
        }
    }
}