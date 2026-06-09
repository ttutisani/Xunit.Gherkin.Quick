using System;
using System.ComponentModel;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.vNext.TestScenarios
{
    internal class TestStepTableRowCellArgument : IXunitSerializable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestStepTableRowCellArgument()
        {
        }

        internal TestStepTableRowCellArgument(string value, TestStepArgumentLocation location)
        {
            Value = value;
            Location = location;
        }

        public string Value { get; private set; }
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