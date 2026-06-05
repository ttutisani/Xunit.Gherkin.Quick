using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xunit.Gherkin.Quick.vNext.FeatureFiles
{
    internal class EmbeddedFeatureFilesProvider : IFeatureFilesProvider
    {
        private readonly Assembly _assembly;

        public EmbeddedFeatureFilesProvider(Assembly assembly)
            => _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

        public IEnumerable<IFeatureFile> GetFeatureFiles()
            => _assembly
                .GetManifestResourceNames()
                .Select(resourceFileName => new EmbeddedFeatureFile(_assembly, resourceFileName));
    }
}