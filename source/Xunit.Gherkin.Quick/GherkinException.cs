using System;

namespace Xunit.Gherkin.Quick
{
    public class GherkinException : Exception
    {
        public GherkinException() : base()
        {
        }

        public GherkinException(string message) : base(message)
        {
        }

        public GherkinException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
