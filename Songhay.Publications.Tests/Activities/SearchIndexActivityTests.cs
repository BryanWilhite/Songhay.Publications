using System.Diagnostics;
using Songhay.Publications.Activities;

namespace Songhay.Publications.Tests.Activities;

public class SearchIndexActivityTests
{
    [SkippableTheory]
    [InlineData("../../../test-files/markdown/presentation/presentation-index.json")]
    public void CompressSearchIndex_Test(string indexFile)
    {
        Skip.IfNot(Debugger.IsAttached);

        indexFile = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, indexFile);

        FileInfo indexFileInfo = new(indexFile);
        Assert.True(indexFileInfo.Exists);

        FileInfo compressedIndexFileInfo = SearchIndexActivity.CompressSearchIndex(indexFileInfo);
        Assert.True(compressedIndexFileInfo.Exists);
    }

    [SkippableTheory]
    [InlineData("../../../test-files/markdown/presentation/entry", "../../../test-files/markdown/presentation", "presentation-index.json")]
    public void GenerateSearchIndexFrom11tyEntries_Test(string entryRoot, string indexRoot, string indexFileName)
    {
        Skip.IfNot(Debugger.IsAttached);

        entryRoot = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, entryRoot);
        indexRoot = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, indexRoot);

        DirectoryInfo entryRootInfo = new(entryRoot);
        Assert.True(entryRootInfo.Exists);

        DirectoryInfo indexRootInfo = new(indexRoot);
        Assert.True(indexRootInfo.Exists);

        FileInfo[] indices = SearchIndexActivity.GenerateSearchIndexFrom11TyEntries(entryRootInfo, indexRootInfo, indexFileName);
        Assert.NotEmpty(indices);
    }
}
