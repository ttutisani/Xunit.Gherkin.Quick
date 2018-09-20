using System.Linq;

namespace Xunit.Gherkin.Quick
{
    public static class GherkinScenarioDefinitionExtensions
    {
        public static global::Gherkin.Ast.Scenario ApplyBackground(
            this global::Gherkin.Ast.Scenario @this, 
            global::Gherkin.Ast.Background background)            
        {
            var backgroundSteps = background != null ? background.Steps : Enumerable.Empty<global::Gherkin.Ast.Step>();
            var stepsWithBackground = backgroundSteps.Concat(@this.Steps).ToArray();

            return new global::Gherkin.Ast.Scenario(
                @this.Tags.ToArray(), 
                @this.Location, 
                @this.Keyword, 
                @this.Name, 
                @this.Description, 
                stepsWithBackground);
        }

        public static global::Gherkin.Ast.ScenarioOutline ApplyBackground(
            this global::Gherkin.Ast.ScenarioOutline @this, 
            global::Gherkin.Ast.Background background)
        {
            var backgroundSteps = background != null ? background.Steps : Enumerable.Empty<global::Gherkin.Ast.Step>();
            var stepsWithBackground = backgroundSteps.Concat(@this.Steps).ToArray();

            return new global::Gherkin.Ast.ScenarioOutline(
                @this.Tags.ToArray(), 
                @this.Location, 
                @this.Keyword, 
                @this.Name, 
                @this.Description, 
                stepsWithBackground, 
                @this.Examples.ToArray());
        }        
    }
}
