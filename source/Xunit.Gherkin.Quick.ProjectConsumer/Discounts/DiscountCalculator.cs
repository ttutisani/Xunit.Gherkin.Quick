using System;

namespace Xunit.Gherkin.Quick.ProjectConsumer.Discounts
{
    public sealed class DiscountCalculator
    {
        public decimal Final { get; internal set; }

        private decimal _price;
        public void SetOriginalPrice(decimal price)
        {
            _price = price; ;
        }

        private decimal _discountPercentage;
        public void SetDiscountPercentage(decimal discountPercentage)
        {
            _discountPercentage = discountPercentage;
        }

        public void ApplyDiscount()
        {
            Final = Math.Round(_price - (_price / 100 * _discountPercentage), 2);
        }
    }
}
