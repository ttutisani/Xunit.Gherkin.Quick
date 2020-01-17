using Gherkin;
using Gherkin.Ast;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureFileRepository : IFeatureFileRepository
    {
        public FeatureFile GetByFilePath(string filePath)
        {
            var gherkinDocument = ParseGherkinDocument(filePath);
            return new FeatureFile(gherkinDocument);
        }

        private static GherkinDocument ParseGherkinDocument(string filePath)
        {
            var parser = new Parser();
            using (var gherkinFile = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var gherkinReader = new StreamReader(gherkinFile))
                {
                    var gherkinDocument = parser.Parse(gherkinReader);
                    return gherkinDocument;
                }
            }
        }

        public List<string> GetFeatureFilePaths()
        {
            var featureFilePaths = Directory.GetFiles("./", "*.feature"); //TODO: remove hardcoded extension.

            return featureFilePaths.ToList();
        }
    }
}
