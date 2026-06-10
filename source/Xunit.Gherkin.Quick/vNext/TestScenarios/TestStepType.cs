using System;

namespace Xunit.Gherkin.Quick.vNext.TestScenarios
{
    [Flags]
    internal enum TestStepType
    {
        Unknown = 0,
        Given = 1 << 0,
        When = 1 << 1,
        Then = 1 << 2,
        And = 1 << 3,
        But = 1 << 4,
        All = Given | When | Then | And | But
    }
}