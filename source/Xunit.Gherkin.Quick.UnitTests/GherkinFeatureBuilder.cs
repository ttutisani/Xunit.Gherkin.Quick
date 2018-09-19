using System;
using System.Collections.Generic;
using System.Linq;
using Gherkin.Ast;

namespace UnitTests
{
    public class GherkinFeatureBuilder
    {
		private List<ScenarioDefinition> _definitions;

		public GherkinFeatureBuilder()
		{
			_definitions = new List<ScenarioDefinition>();
		}

		public GherkinFeatureBuilder WithBackground(Action<GherkinStepBuilder> buildSteps)
		{
			var stepBuilder = new GherkinStepBuilder();
			buildSteps(stepBuilder);
			_definitions.Add(new Background(null, null, null, null, stepBuilder.Steps));
			return this;
		}

		public GherkinFeatureBuilder WithScenario(string name, Action<GherkinStepBuilder> buildSteps)
		{
			return WithScenario(name, new Tag[0], buildSteps);
		}

		public GherkinFeatureBuilder WithScenario(string name, Tag[] tags, Action<GherkinStepBuilder> buildSteps)
		{
			var stepBuilder = new GherkinStepBuilder();
			buildSteps(stepBuilder);
			_definitions.Add(new Scenario(tags, null, null, name, null, stepBuilder.Steps));
			return this;
		}

		public GherkinFeatureBuilder WithScenarioOutline(string name, Action<GherkinStepBuilder> buildSteps, Action<ExamplesBuilder> buildExamples)
		{
			var stepBuilder = new GherkinStepBuilder();
			buildSteps(stepBuilder);

			var examplesBuilder = new ExamplesBuilder();
			buildExamples(examplesBuilder);

			_definitions.Add(new ScenarioOutline(new Tag[0], null, null, name, null, stepBuilder.Steps, examplesBuilder.Examples));
			return this;
		}

		public Feature Build()
		{
			return new Feature(null, null, null, null, null, null, _definitions.ToArray());
		}

		public class ExamplesBuilder
		{
			private TableRow _header;
			private List<Examples> _examples = new List<Examples>();

			public Examples[] Examples => _examples.ToArray();

			public ExamplesBuilder WithExampleHeadings(params string[] headings)
			{
				_header = new TableRow(null, headings.Select(h => new TableCell(null, h)).ToArray());
				return this;
			}

			public ExamplesBuilder WithExamples(string name, Action<TableBuilder> buildRows)
			{
				var tableBuilder = new TableBuilder();
				buildRows(tableBuilder);

				var examples = new Examples(new Tag[0], null, null, name, null, _header, tableBuilder.Rows);
				_examples.Add(examples);
				return this;
			}
		}

		public class TableBuilder
		{
			public List<TableRow> _rows = new List<TableRow>();

			public TableRow[] Rows => _rows.ToArray();

			public TableBuilder WithData(params object[] data)
			{
				_rows.Add(new TableRow(null, data.Select(d => new TableCell(null, d.ToString())).ToArray()));
				return this;
			}
		}

		public class GherkinStepBuilder
		{
			private List<Step> _steps = new List<Step>();

			public Step[] Steps => _steps.ToArray();

			public GherkinStepBuilder Given(string step, StepArgument stepArgument)
			{
				_steps.Add(new Step(null, "Given", step, stepArgument));
				return this;
			}

			public GherkinStepBuilder When(string step, StepArgument stepArgument)
			{
				_steps.Add(new Step(null, "When", step, stepArgument));
				return this;
			}

			public GherkinStepBuilder Then(string step, StepArgument stepArgument)
			{
				_steps.Add(new Step(null, "Then", step, stepArgument));
				return this;
			}

			public GherkinStepBuilder And(string step, StepArgument stepArgument)
			{
				_steps.Add(new Step(null, "And", step, stepArgument));
				return this;
			}
		}
	}	
}
