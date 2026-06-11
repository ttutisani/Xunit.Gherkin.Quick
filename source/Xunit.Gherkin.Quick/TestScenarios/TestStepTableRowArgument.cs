using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Gherkin.Ast;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.TestScenarios
{
    /// <summary>
    /// Represents a row for a table step argument based on a <see cref="TableRow"/>.
    /// </summary>
    public class TestStepTableRowArgument : IXunitSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepTableRowArgument"/> class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestStepTableRowArgument()
        {
        }

        internal TestStepTableRowArgument(IReadOnlyList<TestStepTableCellArgument> cells, TestStepArgumentLocation location)
        {
            Cells = cells;
            Location = location;
        }

        /// <summary>
        /// Gets the <see cref="TestStepTableCellArgument"/> as specified in the feature.
        /// </summary>
        public IReadOnlyList<TestStepTableCellArgument> Cells { get; private set; }

        /// <summary>
        /// Used internally to accurately recreate <see cref="TableRow"/>s for scenario step methods.
        /// </summary>
        internal TestStepArgumentLocation Location { get; private set; }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Cells), Cells as TestStepTableCellArgument[] ?? Cells.ToArray(), typeof(TestStepTableCellArgument[]));
            info.AddValue(nameof(Location), Location, typeof(TestStepArgumentLocation));
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            Cells = info.GetValue<TestStepTableCellArgument[]>(nameof(Cells));
            Location = info.GetValue<TestStepArgumentLocation>(nameof(Location));
        }
    }
}