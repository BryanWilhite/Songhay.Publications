using Songhay.Models;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.Publications.Shell.Tests
{
    public class ProgramTests
    {
        public ProgramTests(ITestOutputHelper helper)
        {
            this._testOutputHelper = helper;
        }

        [Theory]
        [InlineData(
            "MarkdownEntryActivity",
            ProgramArgs.BasePath, "../../../md-presentation-layout/shell",
            ProgramArgs.SettingsFile, "md-add-entry-extract.json",
            ProgramArgs.BasePathRequired)]
        public void ShouldAddEntryExtract(params string[] args)
        {
            var basePath = GetBasePath(args[2]);
            args[2] = basePath;
            this._testOutputHelper.WriteLine(Program.Run(args));
        }

        [Theory]
        [InlineData(
            "MarkdownEntryActivity",
            ProgramArgs.BasePath, "../../../md-presentation-layout/shell",
            ProgramArgs.SettingsFile, "md-expand-uris.json",
            ProgramArgs.BasePathRequired)]
        public void ShouldExpandUris(params string[] args)
        {
            var basePath = GetBasePath(args[2]);
            args[2] = basePath;
            this._testOutputHelper.WriteLine(Program.Run(args));
        }

        [Theory]
        [InlineData(
            "MarkdownEntryActivity",
            ProgramArgs.BasePath, "../../../md-presentation-layout/shell",
            ProgramArgs.SettingsFile, "md-generate-entry.json",
            ProgramArgs.BasePathRequired)]
        public void ShouldGenerateEntry(params string[] args)
        {
            var basePath = GetBasePath(args[2]);
            args[2] = basePath;
            this._testOutputHelper.WriteLine(Program.Run(args));
        }

        [Theory]
        [InlineData(
            "MarkdownEntryActivity",
            ProgramArgs.BasePath, "../../../md-presentation-layout/shell",
            ProgramArgs.SettingsFile, "md-publish-entry.json",
            ProgramArgs.BasePathRequired)]
        public void ShouldPublishEntry(params string[] args)
        {
            var basePath = GetBasePath(args[2]);
            args[2] = basePath;
            this._testOutputHelper.WriteLine(Program.Run(args));
        }

        string GetBasePath(string relativePath) => ProgramAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, relativePath);

        readonly ITestOutputHelper _testOutputHelper;
    }
}
