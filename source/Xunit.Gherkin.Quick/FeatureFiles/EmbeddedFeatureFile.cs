using System;
using System.IO;
using System.Reflection;

namespace Xunit.Gherkin.Quick.FeatureFiles
{
    internal class EmbeddedFeatureFile : IFeatureFile
    {
        private readonly Assembly _assembly;

        public EmbeddedFeatureFile(Assembly assembly, string embeddedResourceName)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            FullName = embeddedResourceName ?? throw new ArgumentNullException(nameof(embeddedResourceName));
            Name = _GetFileName(embeddedResourceName);
        }

        public string Name { get; }

        public string FullName { get; }

        public TextReader OpenRead()
            => new StreamReader(_assembly.GetManifestResourceStream(FullName));

        private static string _GetFileName(string embeddedResourceName)
        {
            var extensionStartIndex = embeddedResourceName.LastIndexOf('.');
            if (extensionStartIndex <= 0)
                return embeddedResourceName;

            var baseFileNameStartIndex = 1 + embeddedResourceName.LastIndexOf('.', startIndex: extensionStartIndex - 1);
            return embeddedResourceName.Substring(baseFileNameStartIndex);
        }
    }
}