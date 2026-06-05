using System;
using Gherkin;
using Gherkin.Ast;
using Xunit.Gherkin.Quick.vNext.FeatureFiles;

namespace Xunit.Gherkin.Quick.vNext.FeatureDocuments
{
    internal class FeatureDocument
    {
        private bool _initialized = false;
        private readonly IFeatureFile _featureFile;
        private readonly Parser _gherkinParser;
        private  global::Gherkin.Ast.Feature _feature = null;
        private  Exception _error = null;

        public FeatureDocument(IFeatureFile featureFile, Parser gherkinParser)
        {
            _featureFile = featureFile;
            _gherkinParser = gherkinParser;
        }

        public string Name
            => _featureFile.Name;

        public string FullName 
            => _featureFile.FullName;

        public global::Gherkin.Ast.Feature Feature
        {
            get
            {
                _EnsureInitialized();
                return _feature;
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

                    _feature = gherkinDocument.Feature;
                    if (_feature is null)
                        _error = new ArgumentException("Feature file does not contain a feature definition.");
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