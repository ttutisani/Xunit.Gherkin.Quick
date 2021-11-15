using System.Threading;

namespace Xunit.Gherkin.Quick.ProjectConsumer.CucumberExpressions
{
    [FeatureFile("./CucumberExpressions/PassParameters.feature")]
    public class PassParameters : Feature
    {
        public PassParameters()
        {
            // Needed to ensure floats always use "." for decimal points.
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }


        #region Integer

        int IntegerValue;

        [When(@"I pass the integer value {int} using cucumber expression")]
        public void SetInteger(int i)
        {
            IntegerValue = i;
        }

        [Then(@"the received integer value is ([+-]?\d+) using regex")]
        public void CheckInteger(int t)
        {
            Assert.Equal(t, IntegerValue);
        }

        #endregion


        #region Float

        float FloatValue;

        [When(@"I pass the float value {float} using cucumber expression")]
        public void SetFloat(float f)
        {
            FloatValue = f;
        }

        [Then(@"the received float value is ([+-]?([0-9]*[.])?[0-9]+) using regex")]
        public void CheckFloat(float expected)
        {
            Assert.Equal(expected, FloatValue);
        }

        #endregion


        #region Word

        string WordValue;

        [When(@"I pass the word {word} using cucumber expression")]
        public void SetWord(string word)
        {
            WordValue = word;
        }

        [Then(@"the received word is (\w+) using regex")]
        public void CheckWord(string expectedOutput)
        {
            Assert.Equal(expectedOutput, WordValue);
        }

        #endregion


        #region String

        string StringValue;

        [When(@"I pass the string {string} using cucumber expression")]
        public void SetString(string s)
        {
            StringValue = s;
        }

        [Then(@"the received string is ""([^""]*)"" using regex")]
        public void CheckString(string expected)
        {
            Assert.Equal(expected, StringValue);
        }

        [Then(@"the received string is '([^']*)' using single quotes regex")]
        public void CheckStringSingleQuote(string expected)
        {
            Assert.Equal(expected, StringValue);
        }

        #endregion


        #region Any/wildcard/anonymous

        [When(@"I say {}")]
        public void SetWildcardString(string s)
        {
            StringValue = s;
        }

        [Then(@"it is the same as ""([^""]*)""")]
        public void IsRepeated(string s)
        {
            Assert.Equal(s, StringValue);
        }


        #endregion
    }
}
