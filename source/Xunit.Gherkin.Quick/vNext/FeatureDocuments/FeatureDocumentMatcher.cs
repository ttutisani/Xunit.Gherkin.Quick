using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit.Gherkin.Quick.vNext.FeatureFiles;
using Xunit.Gherkin.Quick.vNext.FilePatternMatchers;

namespace Xunit.Gherkin.Quick.vNext.FeatureDocuments
{
    internal class FeatureDocumentMatcher
    {
        private readonly FeatureDocumentParser _featureDocumentParser = new FeatureDocumentParser();
        private readonly IDictionary<Assembly, IReadOnlyCollection<FeatureDocument>> _featureDocumentsByAssembly = new Dictionary<Assembly, IReadOnlyCollection<FeatureDocument>>();

        public IEnumerable<FeatureDocument> GetMatchingDocuments(Type featureTestClass)
        {
            var featureTestClassTypeInfo = featureTestClass.GetTypeInfo();
            var assembly = featureTestClassTypeInfo.Assembly;
            var filePatternMatchers = _GetFilePatternMatchers(featureTestClassTypeInfo);
            var featureDocuments = _GetFeatureDocuments(assembly);

            return featureDocuments.Where(featureDocument => filePatternMatchers.Any(filePatternMatcher => filePatternMatcher.Matches(featureDocument.FullName)));
        }

        public static IReadOnlyList<IFilePatternMatcher> _GetFilePatternMatchers(TypeInfo featureTestClass)
            => featureTestClass
                .GetCustomAttributes<FeatureFileAttribute>()
                .Select(_GetFilePatternMatcher)
                .DefaultIfEmpty(new EndsWithFilePatternMatcher($"{featureTestClass.Name.Substring(1 + featureTestClass.Name.LastIndexOf('.'))}.feature"))
                .ToList();

        private static IFilePatternMatcher _GetFilePatternMatcher(FeatureFileAttribute featureFileAttributeInfo)
        {
            switch (featureFileAttributeInfo.PathType)
            {
                case FeatureFilePathType.Simple:
                case FeatureFilePathType.Regex:
                    return new StrictFilePatternMatcher(featureFileAttributeInfo.Path);

                default:
                    throw new ArgumentException($"Not handled '{featureFileAttributeInfo.PathType}' path type.", nameof(featureFileAttributeInfo));
            }
        }

        private IReadOnlyCollection<FeatureDocument> _GetFeatureDocuments(Assembly assembly)
        {
            if (!_featureDocumentsByAssembly.TryGetValue(assembly, out var featureDocuments))
                _featureDocumentsByAssembly[assembly] = featureDocuments =
                    new IFeatureFilesProvider[]
                    {
                        new EmbeddedFeatureFilesProvider(assembly),
                        new SystemFeatureFilesProvider(new DirectoryInfo(Directory.GetCurrentDirectory()))
                    }
                    .SelectMany(featureDocumentProvider => featureDocumentProvider.GetFeatureFiles())
                    .Select(_featureDocumentParser.Parse)
                    .ToList();

            return featureDocuments;
        }
    }
}