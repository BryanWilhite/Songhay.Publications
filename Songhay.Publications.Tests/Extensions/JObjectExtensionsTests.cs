using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using Songhay.Models;
using Songhay.Publications.Activities;
using Songhay.Publications.Extensions;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.Publications.Tests.Extensions
{
    public class JObjectExtensionsTests
    {
        public JObjectExtensionsTests(ITestOutputHelper helper)
        {
            this._testOutputHelper = helper;
        }

        [Theory]
        [InlineData("md-add-entry-extract-settings.json", "../../../markdown/shell")]
        public void GetAddEntryExtractArg_Test(string settingsFile, string presentationRoot)
        {
            presentationRoot = ProgramAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, presentationRoot);

            var args = new ProgramArgs(new[]
                {
                    ProgramArgs.SettingsFile, settingsFile,
                    ProgramArgs.BasePath, presentationRoot
                });

            var (presentationInfo, settingsInfo) = args.ToPresentationAndSettingsInfo();

            Assert.True(presentationInfo.Exists);
            Assert.True(settingsInfo.Exists);

            var jO = JObject.Parse(File.ReadAllText(settingsInfo.FullName));

            var entryPath = jO.GetAddEntryExtractArg(presentationInfo);
            Assert.True(File.Exists(entryPath));

            var commandName = jO.GetPublicationCommand();
            Assert.Equal(nameof(MarkdownEntryActivity.AddEntryExtract), commandName);
        }

        [Theory]
        [InlineData("index-activity-settings.json", "../../../markdown/shell")]
        public void GetCompressed11tyIndexArgs_Test(string settingsFile, string presentationRoot)
        {
            presentationRoot = ProgramAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, presentationRoot);

            var args = new ProgramArgs(new[]
                {
                    ProgramArgs.SettingsFile, settingsFile,
                    ProgramArgs.BasePath, presentationRoot
                });

            var (presentationInfo, settingsInfo) = args.ToPresentationAndSettingsInfo();

            Assert.True(presentationInfo.Exists);
            Assert.True(settingsInfo.Exists);

            var jO = JObject.Parse(File.ReadAllText(settingsInfo.FullName));
            var (entryRootInfo, indexRootInfo, indexFileName) = jO.GetCompressed11tyIndexArgs(presentationInfo);

            Assert.True(entryRootInfo.Exists);
            Assert.True(indexRootInfo.Exists);
            Assert.False(string.IsNullOrWhiteSpace(indexFileName));

            indexRootInfo.FindFile(indexFileName);

            var commandName = jO.GetPublicationCommand();
            Assert.Equal(nameof(IndexActivity.GenerateCompressed11tySearchIndex), commandName);
        }

        [Theory]
        [InlineData("md-expand-uris-settings.json", "../../../markdown/shell")]
        public void GetExpandUrisArgs_Test(string settingsFile, string presentationRoot)
        {
            presentationRoot = ProgramAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, presentationRoot);

            var args = new ProgramArgs(new[]
                {
                    ProgramArgs.SettingsFile, settingsFile,
                    ProgramArgs.BasePath, presentationRoot
                });

            var (presentationInfo, settingsInfo) = args.ToPresentationAndSettingsInfo();

            Assert.True(presentationInfo.Exists);
            Assert.True(settingsInfo.Exists);

            var jO = JObject.Parse(File.ReadAllText(settingsInfo.FullName));

            var (entryPath, collapsedHost) = jO.GetExpandUrisArgs(presentationInfo);
            Assert.True(File.Exists(entryPath));
            Assert.False(string.IsNullOrEmpty(collapsedHost));

            var commandName = jO.GetPublicationCommand();
            Assert.Equal(nameof(MarkdownEntryActivity.ExpandUris), commandName);
        }

        [Theory]
        [InlineData("md-find-change-settings.json", "../../../markdown/shell")]
        public void GetFindChangeArgs_Test(string settingsFile, string presentationRoot)
        {
            presentationRoot = ProgramAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, presentationRoot);

            var args = new ProgramArgs(new[]
                {
                    ProgramArgs.SettingsFile, settingsFile,
                    ProgramArgs.BasePath, presentationRoot
                });

            var (presentationInfo, settingsInfo) = args.ToPresentationAndSettingsInfo();

            Assert.True(presentationInfo.Exists);
            Assert.True(settingsInfo.Exists);

            var jO = JObject.Parse(File.ReadAllText(settingsInfo.FullName));
            var (input, pattern, replacement, useRegex, outputPath) = jO.GetFindChangeArgs(presentationInfo);

            Assert.False(string.IsNullOrEmpty(input));
            Assert.False(string.IsNullOrEmpty(pattern));
            Assert.False(string.IsNullOrEmpty(replacement));
            Assert.True(useRegex);
            Assert.False(string.IsNullOrEmpty(outputPath));

            var inputPath = jO.GetValue<string>("inputPath");

            this._testOutputHelper.WriteLine($"{nameof(inputPath)}: {inputPath}");
            this._testOutputHelper.WriteLine($"{nameof(input)}: {input.Substring(0, 16)}...");
            this._testOutputHelper.WriteLine($"{nameof(pattern)}: {pattern}");
            this._testOutputHelper.WriteLine($"{nameof(replacement)}: {replacement}");
            this._testOutputHelper.WriteLine($"{nameof(useRegex)}: {useRegex}");
            this._testOutputHelper.WriteLine($"{nameof(outputPath)}: {outputPath}");
        }

        [Theory]
        [InlineData("md-generate-entry-settings.json", "../../../markdown/shell")]
        public void GetGenerateEntryArgs_Test(string settingsFile, string presentationRoot)
        {
            presentationRoot = ProgramAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, presentationRoot);

            var args = new ProgramArgs(new[]
                {
                    ProgramArgs.SettingsFile, settingsFile,
                    ProgramArgs.BasePath, presentationRoot
                });

            var (presentationInfo, settingsInfo) = args.ToPresentationAndSettingsInfo();

            Assert.True(presentationInfo.Exists);
            Assert.True(settingsInfo.Exists);

            var jO = JObject.Parse(File.ReadAllText(settingsInfo.FullName));

            var (entryDraftsRootInfo, title) = jO.GetGenerateEntryArgs(presentationInfo);
            Assert.True(entryDraftsRootInfo.Exists);
            Assert.False(string.IsNullOrEmpty(title));

            var commandName = jO.GetPublicationCommand();
            Assert.Equal(nameof(MarkdownEntryActivity.GenerateEntry), commandName);
        }

        [Theory]
        [InlineData("md-publish-entry-settings.json", "../../../markdown/shell")]
        public void GetPublishEntryArgs_Test(string settingsFile, string presentationRoot)
        {
            presentationRoot = ProgramAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, presentationRoot);

            var args = new ProgramArgs(new[]
                {
                    ProgramArgs.SettingsFile, settingsFile,
                    ProgramArgs.BasePath, presentationRoot
                });

            var (presentationInfo, settingsInfo) = args.ToPresentationAndSettingsInfo();

            Assert.True(presentationInfo.Exists);
            Assert.True(settingsInfo.Exists);

            var jO = JObject.Parse(File.ReadAllText(settingsInfo.FullName));

            var (entryDraftsRootInfo, entryRootInfo, entryFileName) =
                jO.GetPublishEntryArgs(presentationInfo);
            Assert.True(entryDraftsRootInfo.Exists);
            Assert.True(entryRootInfo.Exists);
            Assert.False(string.IsNullOrEmpty(entryFileName));

            var commandName = jO.GetPublicationCommand();
            Assert.Equal(nameof(MarkdownEntryActivity.PublishEntry), commandName);
        }

        readonly ITestOutputHelper _testOutputHelper;
    }
}
