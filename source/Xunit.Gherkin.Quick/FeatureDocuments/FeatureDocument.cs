using System;
using Gherkin;
using Gherkin.Ast;
using Xunit.Gherkin.Quick.FeatureFiles;

namespace Xunit.Gherkin.Quick.FeatureDocuments
{
    internal class FeatureDocument
    {
        private bool _initialized = false;
        private readonly IFeatureFile _featureFile;
        private readonly Parser _gherkinParser;
        private GherkinDocument _content = null;
        private Exception _error = null;

        public FeatureDocument(IFeatureFile featureFile, Parser gherkinParser)
        {
            _featureFile = featureFile;
            _gherkinParser = gherkinParser;
        }

        public string Name
            => _featureFile.Name;

        public string FullName
            => _featureFile.FullName;

        public GherkinDocument Content
        {
            get
            {
                _EnsureInitialized();
                return _content;
            }
        }

        public Exception Error
        {
            get
            {
                _EnsureInitialized();
                return _error;
            }
        }

        private void _EnsureInitialized()
        {
            if (!_initialized)
                try
                {
                    GherkinDocument gherkinDocument;
                    using (var featureFileReader = _featureFile.OpenRead())
                        gherkinDocument = _gherkinParser.Parse(featureFileReader);

                    if (gherkinDocument is null || gherkinDocument.Feature is null)
                        _error = new ArgumentException("Feature file does not contain a feature definition.");
                    else
                        _content = gherkinDocument;
                }
                catch (Exception exception)
                {
                    _error = exception;
                }
                finally
                {
                    _initialized = true;
                }
        }
    }
}