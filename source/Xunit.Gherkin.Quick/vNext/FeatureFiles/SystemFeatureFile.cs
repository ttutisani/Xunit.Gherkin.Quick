using System;
using System.IO;

namespace Xunit.Gherkin.Quick.vNext.FeatureFiles
{
    internal class SystemFeatureFile : IFeatureFile
    {
        private static readonly char[] _pathSeparatorChars = new[] { '/', '\\' };
        private readonly FileInfo _featureFileInfo;

        public SystemFeatureFile(DirectoryInfo rootContentDirectory, FileInfo featureFileInfo)
        {
            if (rootContentDirectory is null)
                throw new ArgumentNullException(nameof(rootContentDirectory));
            if (!featureFileInfo.FullName.StartsWith(rootContentDirectory.FullName, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Feature file info is not placed inside of the root content directory.", nameof(featureFileInfo));

            _featureFileInfo = featureFileInfo ?? throw new ArgumentNullException(nameof(featureFileInfo));

            Name = Path.GetFileName(featureFileInfo.FullName.Substring(1 + featureFileInfo.FullName.LastIndexOfAny(_pathSeparatorChars)));
            FullName = Path.Combine(".", featureFileInfo.FullName.Substring(rootContentDirectory.FullName.Length + 1)).Replace('\\', '/');
        }

        public string Name { get; }

        public string FullName { get; }

        public TextReader OpenRead()
            => new StreamReader(new FileStream(_featureFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read));
    }
}