using System;
using System.ComponentModel;
using Gherkin.Ast;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.TestScenarios
{
    /// <summary>
    /// Represents feature file location information, this is used for backwards compatibility to accurately
    /// recreate <see cref="DocString"/>s and <see cref="DataTable"/>s for scenario step methods.
    /// </summary>
    internal class TestStepArgumentLocation : IXunitSerializable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TestStepArgumentLocation()
        {
        }

        internal TestStepArgumentLocation(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; private set; }
        public int Column { get; private set; }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Line), Line, typeof(int));
            info.AddValue(nameof(Column), Column, typeof(int));
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            Line = info.GetValue<int>(nameof(Line));
            Column = info.GetValue<int>(nameof(Column));
        }
    }
}