using Songhay.Publications.Activities;
using Songhay.Tests;
using System.IO;
using Xunit;

namespace Songhay.Publications.Tests.Activities
{
    public class IndexActivityTests
    {
        [DebuggerAttachedTheory]
        [InlineData("../../../markdown/presentation/presentation-index.json")]
        public void CompressIndex_Test(string indexFile)
        {
            indexFile = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, indexFile);

            var indexFileInfo = new FileInfo(indexFile);
            Assert.True(indexFileInfo.Exists);

            var compressedIndexFileInfo = IndexActivity.CompressIndex(indexFileInfo);
            Assert.True(compressedIndexFileInfo?.Exists);
        }

        [DebuggerAttachedTheory]
        [InlineData("../../../markdown/presentation/entry", "../../../markdown/presentation", "presentation-index.json")]
        public void GenerateIndexFrom11tyEntries_Test(string entryRoot, string indexRoot, string indexFileName)
        {
            entryRoot = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, entryRoot);
            indexRoot = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, indexRoot);

            var entryRootInfo = new DirectoryInfo(entryRoot);
            Assert.True(entryRootInfo.Exists);

            var indexRootInfo = new DirectoryInfo(indexRoot);
            Assert.True(indexRootInfo.Exists);

            var indexInfo = IndexActivity.GenerateIndexFrom11tyEntries(entryRootInfo, indexRootInfo, indexFileName);
            Assert.True(indexInfo?.Exists);
        }
    }
}
