using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.vNext.TestScenarios
{
    internal class TestStepTableRowArgument : IXunitSerializable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestStepTableRowArgument()
        {
        }

        internal TestStepTableRowArgument(IReadOnlyList<TestStepTableRowCellArgument> cells, TestStepArgumentLocation location)
        {
            Cells = cells;
            Location = location;
        }

        public IReadOnlyList<TestStepTableRowCellArgument> Cells { get; private set; }
        internal TestStepArgumentLocation Location { get; private set; }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Cells), Cells as TestStepTableRowCellArgument[] ?? Cells.ToArray(), typeof(TestStepTableRowCellArgument[]));
            info.AddValue(nameof(Location), Location, typeof(TestStepArgumentLocation));
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            Cells = info.GetValue<TestStepTableRowCellArgument[]>(nameof(Cells));
            Location = info.GetValue<TestStepArgumentLocation>(nameof(Location));
        }
    }
}