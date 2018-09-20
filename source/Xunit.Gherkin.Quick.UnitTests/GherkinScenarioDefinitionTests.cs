using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public class GherkinScenarioTests
    {
        [Fact]
        public void ApplyBackground_WithNoBackground_ThrowsArgumentNullException()
        {
            var scenario = CreateTestScenario();
            Assert.Throws<ArgumentNullException>(() => scenario.ApplyBackground(null));
        }

        [Fact]
        public void ApplyBackground_WithBackground_PrependsBackgroundSteps()
        {            
            var scenario = CreateTestScenario();
            var backgroundSteps = new Gherkin.Ast.Step[] { new Gherkin.Ast.Step(null, null, "background", null) };
            var background = new Gherkin.Ast.Background(null, null, null, null, backgroundSteps);

            var modified = scenario.ApplyBackground(background);
            
            Assert.Equal(scenario.Location, modified.Location);
            Assert.Equal(scenario.Keyword, modified.Keyword);
            Assert.Equal(scenario.Name, modified.Name);
            Assert.Equal(scenario.Description, modified.Description);
            
            Assert.Equal(2, modified.Steps.Count());
            Assert.Equal("background", modified.Steps.ElementAt(0).Text);
            Assert.Equal("step", modified.Steps.ElementAt(1).Text);
        }        

        private static Gherkin.Ast.Scenario CreateTestScenario()
        {
            var tags = new Gherkin.Ast.Tag[] { new Gherkin.Ast.Tag(null, "test") };
            var location = new Gherkin.Ast.Location(1, 1);
            var steps = new Gherkin.Ast.Step[] { new Gherkin.Ast.Step(null, null, "step", null) };

            return new Gherkin.Ast.Scenario(tags, location, "keyword", "name", "description", steps);            
        }
    }
}
