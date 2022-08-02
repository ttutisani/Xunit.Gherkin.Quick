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
                        GherkinDialect.KeywordFor.Given
                    },
                    new object[]
                    {
                        new WhenAttribute("123"),
                        GherkinDialect.KeywordFor.When
                    },
                    new object[]
                    {
                        new ThenAttribute("123"),
                        GherkinDialect.KeywordFor.Then
                    },
                    new object[]
                    {
                        new AndAttribute("123"),
                        GherkinDialect.KeywordFor.And
                    },
                    new object[]
                    {
                        new ButAttribute("123"),
                        GherkinDialect.KeywordFor.But
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(AllStepDefinitionAttributes))]
        internal void ToStepMethodKind_Converts_based_on_Attribute_type(
            BaseStepDefinitionAttribute attribute,
            GherkinDialect.KeywordFor kind
            )
        {
            //act.
            var actualKind = PatternKindExtensions.ToPatternKind(attribute);

            //assert.
            Assert.Equal(kind, actualKind);
        }

        [Theory]
        [InlineData(GherkinDialect.KeywordFor.Given, "Given", true)]
        [InlineData(GherkinDialect.KeywordFor.Given, "And", false)]
        [InlineData(GherkinDialect.KeywordFor.Given, "*", true)]
        [InlineData(GherkinDialect.KeywordFor.When, "When", true)]
        [InlineData(GherkinDialect.KeywordFor.When, "Given", false)]
        [InlineData(GherkinDialect.KeywordFor.When, "*", true)]
        [InlineData(GherkinDialect.KeywordFor.Then, "Then", true)]
        [InlineData(GherkinDialect.KeywordFor.Then, "But", false)]
        [InlineData(GherkinDialect.KeywordFor.Then, "*", true)]
        [InlineData(GherkinDialect.KeywordFor.And, "And", true)]
        [InlineData(GherkinDialect.KeywordFor.And, "Given", false)]
        [InlineData(GherkinDialect.KeywordFor.And, "*", true)]
        internal void Match_Is_Comparing_With_String_Keyword(
            GherkinDialect.KeywordFor patternKind,
            string keyword,
            bool expectedMatch)
        {
            //act.
            var actualMatch = patternKind.CouldBe(keyword);

            //assert.
            Assert.Equal(expectedMatch, actualMatch);
        }
    }
}
