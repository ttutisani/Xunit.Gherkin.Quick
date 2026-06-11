using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Gherkin.Ast;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.TestScenarios
{
    /// <summary>
    /// Represents a table step argument based on a <see cref="DataTable"/>.
    /// </summary>
    public class TestStepTableArgument : IXunitSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepTableArgument"/> class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestStepTableArgument()
        {
        }

        internal TestStepTableArgument(IReadOnlyList<TestStepTableRowArgument> rows)
        {
            Rows = rows;
        }

        /// <summary>
        /// Gets the <see cref="TestStepTableRowArgument"/> as specified in the feature.
        /// </summary>
        public IReadOnlyList<TestStepTableRowArgument> Rows { get; private set; }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Rows), Rows as TestStepTableRowArgument[] ?? Rows.ToArray(), typeof(TestStepTableRowArgument[]));
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            Rows = info.GetValue<TestStepTableRowArgument[]>(nameof(Rows));
        }
    }
}