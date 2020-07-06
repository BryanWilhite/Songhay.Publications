using Newtonsoft.Json.Linq;
using System.IO;
using Xunit;
using Songhay.Publications.Extensions;
using Songhay.Models;
using Songhay.Extensions;
using Songhay.Publications.Activities;

namespace Songhay.Publications.Tests.Extensions
{
    public class JObjectExtensionsTests
    {
        [Theory]
        [InlineData("md-add-entry-extract-settings.json", "../../../markdown/shell")]
        public void GetAddEntryExtractArg_Test(string settingsFile, string presentationRoot)
        {
            presentationRoot = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, presentationRoot);

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
            const string indexRootPropertyName = "indexRoot";

            presentationRoot = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, presentationRoot);

            var args = new ProgramArgs(new[]
                {
                    ProgramArgs.SettingsFile, settingsFile,
                    ProgramArgs.BasePath, presentationRoot
                });

            var (presentationInfo, settingsInfo) = args.ToPresentationAndSettingsInfo();

            Assert.True(presentationInfo.Exists);
            Assert.True(settingsInfo.Exists);

            var jO = JObject.Parse(File.ReadAllText(settingsInfo.FullName));
            jO[indexRootPropertyName] = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, jO.GetValue<string>(indexRootPropertyName));
            var (entryRootInfo, indexRootInfo, indexFileName) = jO.GetCompressed11tyIndexArgs(presentationInfo);

            Assert.True(entryRootInfo.Exists);
            Assert.True(indexRootInfo.Exists);
            Assert.False(string.IsNullOrWhiteSpace(indexFileName));

            var commandName = jO.GetPublicationCommand();
            Assert.Equal(nameof(IndexActivity.GenerateCompressed11tyIndex), commandName);
        }

        [Theory]
        [InlineData("md-expand-uris-settings.json", "../../../markdown/shell")]
        public void GetExpandUrisArgs_Test(string settingsFile, string presentationRoot)
        {
            presentationRoot = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, presentationRoot);

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
        [InlineData("md-generate-entry-settings.json", "../../../markdown/shell")]
        public void GetGenerateEntryArgs_Test(string settingsFile, string presentationRoot)
        {
            presentationRoot = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, presentationRoot);

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
            presentationRoot = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, presentationRoot);

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
    }
}
