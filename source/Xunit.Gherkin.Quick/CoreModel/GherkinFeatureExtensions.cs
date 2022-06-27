using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ast = global::Gherkin.Ast;

namespace Xunit.Gherkin.Quick
{
    internal static partial class GherkinFeatureExtensions
    {
        public static IEnumerable<Ast.Scenario> Scenarios(this Ast.Feature @this) 
            => @this.Children.OfType<Ast.Scenario>().Where(s => s.IsScenario());

        public static IEnumerable<Ast.Scenario> Outlines(this Ast.Feature @this) 
            => @this.Children.OfType<Ast.Scenario>().Where(s => s.IsOutline());
    }
}
