using System;
using System.ComponentModel;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.vNext.TestScenarios
{
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