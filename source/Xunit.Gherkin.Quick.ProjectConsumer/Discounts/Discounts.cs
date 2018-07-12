namespace Xunit.Gherkin.Quick.ProjectConsumer.Discounts
{
    [FeatureFile("./Discounts/Discounts.feature")]
    public sealed class Discounts : Feature
    {
        private readonly DiscountCalculator _discountCalculator = new DiscountCalculator();

        [Given(@"items like (.+) costing \$([\d\.]+) in John's cart")]
        public void Given_Items_With_Price(string items, decimal price)
        {
            _discountCalculator.SetOriginalPrice(price);
        }

        [And(@"a (\d+)% discount")]
        public void And_Discount_Percentage(decimal discountPercentage)
        {
            _discountCalculator.SetDiscountPercentage(discountPercentage);
        }

        [When(@"John proceeds to checkout")]
        public void When_John_Proceeds_To_Checkout()
        {
            _discountCalculator.ApplyDiscount();
        }

        [Then(@"he should only pay \$([\d\.]+) for the items in his cart")]
        public void He_Should_Only_Pay_Final(decimal final)
        {
            var actualFinal = _discountCalculator.Final;

            Assert.Equal(final, actualFinal);
        }
    }
}
