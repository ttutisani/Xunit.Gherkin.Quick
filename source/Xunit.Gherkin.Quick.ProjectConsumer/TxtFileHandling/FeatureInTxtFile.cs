using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Gherkin.Quick.ProjectConsumer.TxtFileHandling
{
    [FeatureFile("./TxtFileHandling/FeatureInTxtFile.txt")]
    public sealed class FeatureInTxtFile : Feature
    {
        [Given("I have a feature in TXT file")]
        public void Given_I_have_feature_in_txt_file() { }
    }
}
