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

        [When(@"I choose the integer value {int}")]
        public void SetInteger(int i)
        {
            IntegerValue = i;
        }

        [Then(@"the triple value is {int}")]
        public void CheckTriple(int t)
        {
            Assert.Equal(t, IntegerValue * 3);
        }

        #endregion


        #region Float

        float FloatValue;

        [When(@"I choose the float value {float}")]
        public void SetFloat(float f)
        {
            FloatValue = f;
        }

        [Then(@"10.75 more is {float}")]
        public void AddToFloat(float expected)
        {
            Assert.Equal(expected, FloatValue + 10.75);
        }

        #endregion


        #region Word

        string FirstWord;
        string SecondWord;

        [Given(@"the first word is {word}")]
        public void SetFirstWord(string word)
        {
            FirstWord = word;
        }

        [When(@"I choose the second word (\w+)")]
        public void SetSecondWord(string word)
        {
            SecondWord = word;
        }

        [Then(@"the concatenation of them is (\w+)")]
        public void IsConcatenated(string expectedOutput)
        {
            Assert.Equal(expectedOutput, FirstWord + SecondWord);
        }

        #endregion


        #region String

        string StringValue;

        [When(@"I choose the string {string}")]
        public void SetString(string s)
        {
            StringValue = s;
        }

        [Then(@"the lower case string is {string}")]
        public void IsLowerCase(string expected)
        {
            Assert.Equal(expected, StringValue.ToLower());
        }

        #endregion


        #region Any/wildcard/anonymous

        [When(@"I say {}")]
        public void SetWildcardString(string s)
        {
            StringValue = s;
        }

        [Then(@"it is the same as {string}")]
        public void IsRepeated(string s)
        {
            Assert.Equal(s, StringValue);
        }


        #endregion
    }
}
