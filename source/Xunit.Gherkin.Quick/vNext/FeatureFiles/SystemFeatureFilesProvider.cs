using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xunit.Gherkin.Quick.vNext.FeatureFiles
{
    internal class SystemFeatureFilesProvider : IFeatureFilesProvider
    {
        private readonly DirectoryInfo _contentDirectory;

        public SystemFeatureFilesProvider(DirectoryInfo contentDirectory)
            => _contentDirectory = contentDirectory ?? throw new ArgumentNullException(nameof(contentDirectory));

        public IEnumerable<IFeatureFile> GetFeatureFiles()
            => _contentDirectory
                .EnumerateFiles("*", SearchOption.AllDirectories)
                .Select(file => new SystemFeatureFile(_contentDirectory, file));
    }
}