using Gherkin.Ast;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureFile
    {
        public GherkinDocument GherkinDocument { get; }

        public FeatureFile(GherkinDocument gherkinDocument)
        {
            GherkinDocument = gherkinDocument ?? throw new System.ArgumentNullException(nameof(gherkinDocument));
        }
    }
}
