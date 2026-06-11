using System;

namespace Xunit.Gherkin.Quick.TestScenarios
{
    /// <summary>
    /// Represents the step type based on its keyword across translations.
    /// </summary>
    [Flags]
    public enum TestStepType
    {
        /// <summary>
        /// Represents an unknown keyword.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Represents a step defined with a "given" keyword across translations.
        /// </summary>
        Given = 1 << 0,
        /// <summary>
        /// Represents a step defined with a "when" keyword across translations.
        /// </summary>
        When = 1 << 1,
        /// <summary>
        /// Represents a step defined with a "then" keyword across translations.
        /// </summary>
        Then = 1 << 2,
        /// <summary>
        /// Represents a step defined with a "and" keyword across translations.
        /// </summary>
        And = 1 << 3,
        /// <summary>
        /// Represents a step defined with a "but" keyword across translations.
        /// </summary>
        But = 1 << 4,
        /// <summary>
        /// Represents a step defined with an asterisk mapping to all keywords.
        /// </summary>
        All = Given | When | Then | And | But
    }
}