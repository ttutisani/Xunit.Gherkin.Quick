﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Xunit.Gherkin.Quick
{
    using Gherkin = global::Gherkin;
    using Ast = global::Gherkin.Ast;

    internal static partial class GherkinDialect
    {
        private static Gherkin.GherkinDialectProvider GherkingDialectProvider { get;  } = new();

        private static Dictionary<string, Gherkin.GherkinDialect> Dialects { get; } = new() 
            {{ GherkingDialectProvider.DefaultDialect.Language, GherkingDialectProvider.DefaultDialect }};

        public static void Register(string? language, Gherkin.Ast.Location? location)
        {
            if (language is null || location is null) return;

            if (Dialects.Keys.Contains(language)) return;
            
            Dialects[language] = GherkingDialectProvider.GetDialect(language, location);

        }

        private static IEnumerable<string> Keywords(this IEnumerable<Gherkin.GherkinDialect> @this, Func<Gherkin.GherkinDialect, string[]> keywords)
            => Enumerable.Distinct(
                @this.SelectMany(
                    dialect => Enumerable.Concat(
                        from k in keywords(dialect) select k,
                        from k in keywords(dialect) select k.Trim()
                    )
                )
            );

        public enum KeywordFor
        {
            Feature, Background, Rule, Scenario, Outline,

            Examples,

            Given, When, Then, And, But
        }
        
        public static bool CouldBe(this KeywordFor @for, string keyword)
            => @for switch
            {
                KeywordFor.Feature => Dialects.Values.Keywords(d => d.FeatureKeywords).Contains(keyword),
                KeywordFor.Background => Dialects.Values.Keywords(d => d.BackgroundKeywords).Contains(keyword),
                KeywordFor.Rule => Dialects.Values.Keywords(d => d.RuleKeywords).Contains(keyword),
                KeywordFor.Scenario => Dialects.Values.Keywords(d => d.ScenarioKeywords).Contains(keyword),
                KeywordFor.Outline => Dialects.Values.Keywords(d => d.ScenarioOutlineKeywords).Contains(keyword),
                KeywordFor.Examples => Dialects.Values.Keywords(d => d.ExamplesKeywords).Contains(keyword),
                KeywordFor.Given => Dialects.Values.Keywords(d => d.GivenStepKeywords).Contains(keyword),
                KeywordFor.When => Dialects.Values.Keywords(d => d.WhenStepKeywords).Contains(keyword),
                KeywordFor.Then => Dialects.Values.Keywords(d => d.ThenStepKeywords).Contains(keyword),
                KeywordFor.And => Dialects.Values.Keywords(d => d.AndStepKeywords).Contains(keyword),
                KeywordFor.But => Dialects.Values.Keywords(d => d.ButStepKeywords).Contains(keyword)
            };

        public static bool IsScenario(this Ast.Scenario @this)
            => KeywordFor.Scenario.CouldBe(@this.Keyword);

        public static bool IsOutline(this Ast.Scenario @this)
            => KeywordFor.Outline.CouldBe(@this.Keyword);

    }
}
