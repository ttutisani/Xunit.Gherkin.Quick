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
        private readonly char[] _common_path_separators = new char[]{'/', '\\'};

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

        public List<string> FindFilesByPattern(string pattern) {
            var lastPathSeparatorIndex = pattern.LastIndexOfAny(_common_path_separators);          
            string baseDirectory = Path.GetFullPath(lastPathSeparatorIndex > -1 ? pattern.Substring(0,lastPathSeparatorIndex) : "");
            string filePattern = pattern.Substring(lastPathSeparatorIndex+1);
            var found = Directory
                .EnumerateFiles(baseDirectory, filePattern, SearchOption.AllDirectories)
                .ToList();
            return found;
        }

        public List<string> GetFeatureFilePaths()
        {
            var featureFilePaths = Directory.GetFiles("./", _featureFileSearchPattern, SearchOption.AllDirectories)
                .Select(p => p.Replace('\\', '/'))
                .ToList();

            return featureFilePaths;
        }
    }
}
