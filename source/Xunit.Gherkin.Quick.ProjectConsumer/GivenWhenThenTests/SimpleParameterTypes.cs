using System;
using System.Globalization;
using System.Threading;

namespace Xunit.Gherkin.Quick.ProductConsumer
{
    [FeatureFile("./GivenWhenThenTests/SimpleParameterTypes.feature")]
    public sealed class SimpleParameterTypes : Feature
    {
        public SimpleParameterTypes()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        [Given(@"Value (\d) should be 1")]
        public void Value_1_should_be_1(int value)
        {
            Assert.Equal(1, value);
        }

        [And(@"Value (\w+) should be WORD")]
        public void Value_WORD_should_be_WORD(string value)
        {
            Assert.Equal("WORD", value);
        }

        [But(@"Value (\d\.\d\d) should be 1.23")]
        public void Value_1_23_should_be_1_23(float value)
        {
            Assert.Equal(1.23, Math.Round(value, 2));
        }

        [When(@"Value ([\d\/]+) should be 1/2/2003")]
        public void Value_122003_should_be_122003(DateTime value)
        {
            Assert.Equal(new DateTime(2003, 1, 2), value);
        }

        [And(@"Value (-\d\d) should be value -12")]
        public void Value_m12_should_be_value_m12(long value)
        {
            Assert.Equal(-12, value);
        }

        [But(@"Value (-[\d\.]+) should be value -3.21")]
        public void Value_m321_should_be_value_m321(decimal value)
        {
            Assert.Equal(-3.21m, value);
        }

        [Then(@"Values (\d) and (\d) should be 1 and 2")]
        public void Values_1_and_2_should_be_1_and_2(int value1, long value2)
        {
            Assert.Equal(1, value1);
            Assert.Equal(2, value2);
        }

        [And(@"Values (WORD) and (\d\d\d.\d) should be WORD and 123.4")]
        public void Values_WORD_and_1234_should_be_WORD_and_1234(string value1, decimal value2)
        {
            Assert.Equal("WORD", value1);
            Assert.Equal(123.4m, value2);
        }

        [But(@"Values (\d\/\d\/\d{4}) and (-\d\.\d{2}) should be values 3/2/2001 and -3.14")]
        public void Values_232001_and_m314_should_be_322001_and_m314(DateTime value1, float value2)
        {
            Assert.Equal(new DateTime(2001, 3, 2), value1);
            Assert.Equal(-3.14, Math.Round(value2, 2));
        }
    }
}
