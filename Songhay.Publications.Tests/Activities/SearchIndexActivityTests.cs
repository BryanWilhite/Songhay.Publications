using System.Diagnostics;
using Songhay.Publications.Activities;

namespace Songhay.Publications.Tests.Activities;

public class SearchIndexActivityTests
{
    [SkippableTheory]
    [InlineData("../../../markdown/presentation/presentation-index.json")]
    public void CompressSearchIndex_Test(string indexFile)
    {
        Skip.IfNot(Debugger.IsAttached);

        indexFile = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, indexFile);

        var indexFileInfo = new FileInfo(indexFile);
        Assert.True(indexFileInfo.Exists);

        var compressedIndexFileInfo = SearchIndexActivity.CompressSearchIndex(indexFileInfo);
        Assert.True(compressedIndexFileInfo.Exists);
    }

    [SkippableTheory]
    [InlineData("../../../markdown/presentation/entry", "../../../markdown/presentation", "presentation-index.json")]
    public void GenerateSearchIndexFrom11tyEntries_Test(string entryRoot, string indexRoot, string indexFileName)
    {
        Skip.IfNot(Debugger.IsAttached);

        entryRoot = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, entryRoot);
        indexRoot = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, indexRoot);

        var entryRootInfo = new DirectoryInfo(entryRoot);
        Assert.True(entryRootInfo.Exists);

        var indexRootInfo = new DirectoryInfo(indexRoot);
        Assert.True(indexRootInfo.Exists);

        var indices = SearchIndexActivity.GenerateSearchIndexFrom11TyEntries(entryRootInfo, indexRootInfo, indexFileName);
        Assert.True(indices.Any());
    }
}
