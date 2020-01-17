namespace Xunit.Gherkin.Quick
{
    internal interface IFileSystem
    {
        bool FileExists(string fileName);
    }
}
