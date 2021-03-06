﻿using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using Songhay.Publications.Extensions;
using Songhay.Publications.Models;
using Songhay.Tests;
using System;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.Publications.Tests.Extensions
{
    public class MarkdownEntryExtensionsTests
    {
        public MarkdownEntryExtensionsTests(ITestOutputHelper helper)
        {
            this._testOutputHelper = helper;
        }

        [Fact]
        public void DoNullCheck_Test()
        {
            // arrange
            MarkdownEntry data = null;

            // assert
            Assert.Throws<NullReferenceException>(() => data.DoNullCheck());
        }

        [Theory]
        [ProjectFileData(typeof(MarkdownEntryExtensionsTests),
            new object[] { 255 },
            "../../../markdown/presentation-drafts/to-extract-test.md")]
        [ProjectFileData(typeof(MarkdownEntryExtensionsTests),
            new object[] { 255 },
            "../../../markdown/presentation-drafts/to-extract-test2.md")]
        public void ToExtract_Test(int expectedLength, FileInfo entryInfo)
        {
            var entry = entryInfo.ToMarkdownEntry();

            var extract = entry.ToExtract(expectedLength);

            Assert.False(string.IsNullOrWhiteSpace(extract));
            Assert.Equal(expectedLength + 1, extract.Length);
        }

        [Theory]
        [ProjectFileData(typeof(MarkdownEntryExtensionsTests),
            "../../../markdown/to-markdown-entry-test.md")]
        public void ToFinalEdit_Test(FileInfo entryInfo)
        {
            //arrange
            var expected = File.ReadAllText(entryInfo.FullName);

            // act
            var actual = entryInfo.ToMarkdownEntry().ToFinalEdit();

            //assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [ProjectFileData(typeof(MarkdownEntryExtensionsTests),
            new object[] { "2005-01-18-msn-video-replaces-feedroomcom" },
            "../../../markdown/to-markdown-entry-test.md")]
        public void ToMarkdownEntry_Test(string clientId, FileInfo entryInfo)
        {
            // arrange
            var entry = entryInfo.ToMarkdownEntry();

            // act
            var jFrontMatter = entry.FrontMatter;

            // assert
            Assert.NotNull(jFrontMatter);
            Assert.Equal(clientId, jFrontMatter.GetValue<string>("clientId"));
        }

        [Theory]
        [ProjectFileData(typeof(MarkdownEntryExtensionsTests),
            new object[] { 5 },
            "../../../markdown/to-markdown-entry-test.md")]
        public void ToParagraphs_Test(int expectedNumberOfParagraphs, FileInfo entryInfo)
        {
            //arrange
            var entry = entryInfo.ToMarkdownEntry();

            // act
            var paragraphs = entry.ToParagraphs();

            //assert
            Assert.Equal(expectedNumberOfParagraphs, paragraphs.Length);
        }

        [Theory]
        [ProjectFileData(typeof(MarkdownEntryExtensionsTests),
            "../../../markdown/to-markdown-entry-test.md")]
        public void ToParagraphsAndToFinalEdit_Test(FileInfo entryInfo)
        {
            //arrange
            var expected = File.ReadAllText(entryInfo.FullName);
            var entry = entryInfo.ToMarkdownEntry();
            var paragraphs = entry.ToParagraphs();

            // act
            entry.Content = paragraphs
                                .Aggregate(
                                    string.Empty,
                                    (a, p) => string.Concat(a, $"{MarkdownEntry.NewLine}{MarkdownEntry.NewLine}", p)
                                );
            var actual = entryInfo.ToMarkdownEntry().ToFinalEdit();

            //assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Hello World!", null, "./path", null, 8)]
        [InlineData("Hello World!", "It was the best of times.", "./path", null, 8)]
        [InlineData("Hello World!", "It was the best of times.", "./path", "thing", 8)]
        [InlineData("Hello World!", "It was the best of times.", "./path", "{\"thing\": true}", 8)]
        public void With11tyExtract_Test(string title, string content, string path, string tag, int length)
        {
            var entry = (new MarkdownEntry())
                .WithNew11tyFrontMatter(title, DateTime.Now, path, tag)
                .WithContentHeader()
                .WithEdit(i => i.Content = string.Concat(i.Content, content))
                .With11tyExtract(length);
            var jO = JObject.Parse(entry.FrontMatter.GetValue<string>("tag"));
            Assert.NotNull(jO);

            var extract = jO.GetValue<string>("extract");

            this._testOutputHelper.WriteLine($"front matter (input-tag: `{tag ?? "[null]"}`):");
            this._testOutputHelper.WriteLine(entry.FrontMatter.ToString());

            if (content == null)
                Assert.True(string.IsNullOrWhiteSpace(extract));
            else
                Assert.False(string.IsNullOrWhiteSpace(extract));
        }

        [Theory]
        [ProjectFileData(typeof(MarkdownEntryExtensionsTests),
            new object[] { 255 },
            "../../../markdown/presentation-drafts/to-extract-test.md")]
        [ProjectFileData(typeof(MarkdownEntryExtensionsTests),
            new object[] { 255 },
            "../../../markdown/presentation-drafts/to-extract-test2.md")]
        public void With11tyExtract_FromFile_Test(int expectedLength, FileInfo entryInfo)
        {
            var entry = entryInfo.ToMarkdownEntry()
                .With11tyExtract(expectedLength);

            var jO = JObject.Parse(entry.FrontMatter.GetValue<string>("tag"));
            Assert.NotNull(jO);

            var extract = jO.GetValue<string>("extract");

            Assert.False(string.IsNullOrWhiteSpace(extract));
            Assert.Equal(expectedLength + 1, extract.Length);
        }

        [Fact]
        public void WithNew11tyFrontMatterAndContentHeaderAndTouch_Test()
        {
            //arrange
            var title = "Hello World!";
            var inceptDate = DateTime.Now.AddSeconds(-3);
            var entry = new MarkdownEntry()
                .WithNew11tyFrontMatter(title, inceptDate, "/path/to/entry/", "entry_tag")
                .WithContentHeader();

            //act
            this._testOutputHelper.WriteLine(entry.ToFinalEdit());
            entry.Touch(DateTime.Now);

            //assert
            Assert.True(entry.FrontMatter.GetValue<DateTime>("modificationDate") > inceptDate);
        }

        readonly ITestOutputHelper _testOutputHelper;
    }
}
