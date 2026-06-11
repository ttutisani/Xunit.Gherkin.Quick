using System;
using Xunit.Gherkin.Quick.FeatureFiles;

namespace Xunit.Gherkin.Quick.FeatureDocuments
{
    internal class FeatureDocumentParser
    {
        private readonly global::Gherkin.Parser _gherkinParser = new global::Gherkin.Parser();

        public FeatureDocument Parse(IFeatureFile featureFile)
        {
            if (featureFile is null)
                throw new ArgumentNullException(nameof(featureFile));

            return new FeatureDocument(featureFile, _gherkinParser);
        }
    }
}