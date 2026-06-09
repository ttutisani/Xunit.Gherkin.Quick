using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.vNext.TestScenarios
{
    internal class TestStepTableArgument : IXunitSerializable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestStepTableArgument()
        {
        }

        internal TestStepTableArgument(IReadOnlyList<TestStepTableRowArgument> rows, TestStepArgumentLocation location)
        {
            Rows = rows;
            Location = location;
        }

        public IReadOnlyList<TestStepTableRowArgument> Rows { get; private set; }
        internal TestStepArgumentLocation Location { get; private set; }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Rows), Rows as TestStepTableRowArgument[] ?? Rows.ToArray(), typeof(TestStepTableRowArgument[]));
            info.AddValue(nameof(Location), Location, typeof(TestStepArgumentLocation));
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            Rows = info.GetValue<TestStepTableRowArgument[]>(nameof(Rows));
            Location = info.GetValue<TestStepArgumentLocation>(nameof(Location));
        }
    }
}