using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public sealed class StepMethodKindExtensionsTests
    {
        public static object[][] AllStepDefinitionAttributes
        {
            get
            {
                return new object[][] 
                {
                    new object[]
                    {
                        new GivenAttribute("123"),
                        PatternKind.Given
                    },
                    new object[]
                    {
                        new WhenAttribute("123"),
                        PatternKind.When
                    },
                    new object[]
                    {
                        new ThenAttribute("123"),
                        PatternKind.Then
                    },
                    new object[]
                    {
                        new AndAttribute("123"),
                        PatternKind.And
                    },
                    new object[]
                    {
                        new ButAttribute("123"),
                        PatternKind.But
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(AllStepDefinitionAttributes))]
        internal void ToStepMethodKind_Converts_based_on_Attribute_type(
            BaseStepDefinitionAttribute attribute,
            PatternKind kind
            )
        {
            //act.
            var actualKind = PatternKindExtensions.ToPatternKind(attribute);

            //assert.
            Assert.Equal(kind, actualKind);
        }

        [Theory]
        [InlineData(PatternKind.Given, "Given", true)]
        [InlineData(PatternKind.Given, "And", false)]
        [InlineData(PatternKind.When, "When", true)]
        [InlineData(PatternKind.When, "Given", false)]
        [InlineData(PatternKind.Then, "Then", true)]
        [InlineData(PatternKind.Then, "But", false)]
        [InlineData(PatternKind.And, "And", true)]
        [InlineData(PatternKind.And, "Given", false)]
        internal void Match_Is_Comparing_With_String_Keyword(
            PatternKind patternKind,
            string keyword,
            bool expectedMatch)
        {
            //act.
            var actualMatch = patternKind.Matches(keyword);

            //assert.
            Assert.Equal(expectedMatch, actualMatch);
        }
    }
}
