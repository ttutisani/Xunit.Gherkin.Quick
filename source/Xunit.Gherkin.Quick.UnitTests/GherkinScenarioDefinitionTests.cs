using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Gherkin.Quick;

namespace UnitTests
{
    public class GherkinScenarioDefinitionTests
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public void ApplyBackground_WithNoBackground_CopiesScenarioDetails(Gherkin.Ast.ScenarioDefinition baseDefinition)
        {
            Gherkin.Ast.ScenarioDefinition modified = null;

            if (baseDefinition is Gherkin.Ast.Scenario scenario)
                modified = scenario.ApplyBackground(null);
            else if (baseDefinition is Gherkin.Ast.ScenarioOutline scenarioOutline)
                modified = scenarioOutline.ApplyBackground(null);
                       
            Assert.Equal(baseDefinition.Location, modified.Location);
            Assert.Equal(baseDefinition.Keyword, modified.Keyword);
            Assert.Equal(baseDefinition.Name, baseDefinition.Name);
            Assert.Equal(baseDefinition.Description, baseDefinition.Description);
            Assert.Equal(baseDefinition.Steps.Count(), modified.Steps.Count());
            Assert.Equal(baseDefinition.Steps.First().Text, modified.Steps.First().Text);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void ApplyBackground_WithBackground_PrependsBackgroundSteps(Gherkin.Ast.ScenarioDefinition baseDefinition)
        {            
            var backgroundSteps = new Gherkin.Ast.Step[] { new Gherkin.Ast.Step(null, null, "background", null) };
            var background = new Gherkin.Ast.Background(null, null, null, null, backgroundSteps);

            Gherkin.Ast.ScenarioDefinition modified = null;

            if (baseDefinition is Gherkin.Ast.Scenario scenario)
                modified = scenario.ApplyBackground(background);
            else if (baseDefinition is Gherkin.Ast.ScenarioOutline scenarioOutline)
                modified = scenarioOutline.ApplyBackground(background);

            Assert.Equal(baseDefinition.Location, modified.Location);
            Assert.Equal(baseDefinition.Keyword, modified.Keyword);
            Assert.Equal(baseDefinition.Name, baseDefinition.Name);
            Assert.Equal(baseDefinition.Description, baseDefinition.Description);
            
            Assert.Equal(2, modified.Steps.Count());
            Assert.Equal("background", modified.Steps.ElementAt(0).Text);
            Assert.Equal("step", modified.Steps.ElementAt(1).Text);
        }

        public static IEnumerable<object[]> TestData()
        {
            var tags = new Gherkin.Ast.Tag[] { new Gherkin.Ast.Tag(null, "test") };
            var location = new Gherkin.Ast.Location(1, 1);
            var steps = new Gherkin.Ast.Step[] { new Gherkin.Ast.Step(null, null, "step", null) };

            yield return new object[] { new Gherkin.Ast.Scenario(tags, location, "keyword", "name", "description", steps) };

            var examples = new Gherkin.Ast.Examples[] { new Gherkin.Ast.Examples(null, null, null, "examples", null, null, null) };

            yield return new object[] { new Gherkin.Ast.ScenarioOutline(tags, location, "keyword", "name", "description", steps, examples) };
        }
    }
}
