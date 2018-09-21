using System;
using System.Linq;

namespace Xunit.Gherkin.Quick
{
    public static class GherkinScenarioExtensions
    {
        public static global::Gherkin.Ast.Scenario ApplyBackground(
            this global::Gherkin.Ast.Scenario @this, 
            global::Gherkin.Ast.Background background)            
        {
            if(background == null)
                throw new ArgumentNullException(nameof(background));

            var stepsWithBackground = background.Steps.Concat(@this.Steps).ToArray();

            return new global::Gherkin.Ast.Scenario(
                @this.Tags.ToArray(), 
                @this.Location, 
                @this.Keyword, 
                @this.Name, 
                @this.Description, 
                stepsWithBackground);
        }       
    }
}
