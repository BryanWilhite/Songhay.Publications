﻿using Songhay.Extensions;
using Songhay.Tests;
using System.IO;
using Xunit;

namespace Songhay.Publications.Tests
{
    public class MarkdownEntryUtilityTests
    {
        [DebuggerAttachedTheory]
        [InlineData("../../../markdown/presentation-drafts", "Hello World!")]
        public void GenerateEntryFor11ty_Test(string entryRoot, string title)
        {
            entryRoot = ProgramAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, entryRoot);

            var entry = MarkdownEntryUtility.GenerateEntryFor11ty(entryRoot, title);

            Assert.NotNull(entry);
            Assert.True(File.Exists(ProgramFileUtility.GetCombinedPath(entryRoot, $"{entry.FrontMatter.GetValue<string>("clientId")}.md")));
        }

        [DebuggerAttachedTheory]
        [InlineData("../../../markdown/presentation-drafts", "../../../markdown/presentation/entry", "2019-11-19-hello-world.md")]
        public void PublishEntryFor11ty_Test(string entryRoot, string presentationRoot, string fileName)
        {
            entryRoot = ProgramAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, entryRoot);
            presentationRoot = ProgramAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, presentationRoot);

            var path = MarkdownEntryUtility.PublishEntryFor11ty(entryRoot, presentationRoot, fileName);

            Assert.True(File.Exists(path));

        }
    }
}
