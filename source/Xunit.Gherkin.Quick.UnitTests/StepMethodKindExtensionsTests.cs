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
                        StepMethodKind.Given
                    },
                    new object[]
                    {
                        new WhenAttribute("123"),
                        StepMethodKind.When
                    },
                    new object[]
                    {
                        new ThenAttribute("123"),
                        StepMethodKind.Then
                    },
                    new object[]
                    {
                        new AndAttribute("123"),
                        StepMethodKind.And
                    },
                    new object[]
                    {
                        new ButAttribute("123"),
                        StepMethodKind.But
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(AllStepDefinitionAttributes))]
        internal void ToStepMethodKind_Converts_based_on_Attribute_type(
            BaseStepDefinitionAttribute attribute,
            StepMethodKind kind
            )
        {
            //act.
            var actualKind = StepMethodKindExtensions.ToStepMethodKind(attribute);

            //assert.
            Assert.Equal(kind, actualKind);
        }
    }
}
