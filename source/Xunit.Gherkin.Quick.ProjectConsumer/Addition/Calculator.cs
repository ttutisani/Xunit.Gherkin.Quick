using System;
using System.Collections.Generic;
using System.Linq;

namespace Xunit.Gherkin.Quick.ProjectConsumer.Addition
{
    public sealed class Calculator
    {
        public List<int> Numbers { get; set; } = new List<int>();
        public int Result { get; private set; }       
        public void AddNumbers() => Result = Numbers.Sum();
    }
}
