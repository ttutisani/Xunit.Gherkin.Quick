using System;

namespace Xunit.Gherkin.Quick
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AfterStepAttribute : Attribute
    {
    }
}
