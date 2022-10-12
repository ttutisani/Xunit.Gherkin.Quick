using Gherkin;
using Gherkin.Ast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xunit.Gherkin.Quick
{
    internal sealed class FeatureFileRepository : IFeatureFileRepository
    {
        private readonly string _featureFileSearchPattern;

        public FeatureFileRepository(string featureFileSearchPattern)
        {
            _featureFileSearchPattern = !string.IsNullOrWhiteSpace(featureFileSearchPattern)
                ? featureFileSearchPattern 
                : throw new ArgumentNullException(nameof(featureFileSearchPattern));
        }

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
            var allPatterns = _featureFileSearchPattern.Split('|');
            var featureFilePaths = allPatterns.SelectMany(pattern => Directory.GetFiles("./", pattern, SearchOption.AllDirectories))
                .Select(p => p.Replace('\\', '/'))
                .ToList();

            return featureFilePaths;
        }
    }
}
