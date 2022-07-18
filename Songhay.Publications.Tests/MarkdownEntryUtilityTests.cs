using Songhay.Extensions;
using Songhay.Tests;
using Xunit;

namespace Songhay.Publications.Tests;

public class MarkdownEntryUtilityTests
{
    [DebuggerAttachedTheory]
    [InlineData("../../../markdown/presentation-drafts", "Hello World!")]
    public void GenerateEntryFor11ty_Test(string entryRoot, string title)
    {
        entryRoot = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, entryRoot);

        var entry = MarkdownEntryUtility.GenerateEntryFor11Ty(entryRoot, title);

        Assert.NotNull(entry);
        Assert.True(File.Exists(ProgramFileUtility.GetCombinedPath(entryRoot, $"{entry.FrontMatter.GetValue<string>("clientId")}.md")));
    }

    [DebuggerAttachedTheory]
    [InlineData("../../../markdown/presentation-drafts", "../../../markdown/presentation/entry", "2019-11-19-hello-world.md")]
    public void PublishEntryFor11ty_Test(string entryRoot, string presentationRoot, string fileName)
    {
        entryRoot = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, entryRoot);
        presentationRoot = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, presentationRoot);

        var path = MarkdownEntryUtility.PublishEntryFor11Ty(entryRoot, presentationRoot, fileName);

        Assert.True(File.Exists(path));

    }
}
