using Songhay.Publications.Activities;
using Songhay.Tests;
using System.IO;
using System.Linq;
using Xunit;

namespace Songhay.Publications.Tests.Activities
{
    public class IndexActivityTests
    {
        [DebuggerAttachedTheory]
        [InlineData("../../../markdown/presentation/presentation-index.json")]
        public void CompressSearchIndex_Test(string indexFile)
        {
            indexFile = ProgramAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, indexFile);

            var indexFileInfo = new FileInfo(indexFile);
            Assert.True(indexFileInfo.Exists);

            var compressedIndexFileInfo = IndexActivity.CompressSearchIndex(indexFileInfo);
            Assert.True(compressedIndexFileInfo?.Exists);
        }

        [DebuggerAttachedTheory]
        [InlineData("../../../markdown/presentation/entry", "../../../markdown/presentation", "presentation-index.json")]
        public void GenerateSearchIndexFrom11tyEntries_Test(string entryRoot, string indexRoot, string indexFileName)
        {
            entryRoot = ProgramAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, entryRoot);
            indexRoot = ProgramAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, indexRoot);

            var entryRootInfo = new DirectoryInfo(entryRoot);
            Assert.True(entryRootInfo.Exists);

            var indexRootInfo = new DirectoryInfo(indexRoot);
            Assert.True(indexRootInfo.Exists);

            var indices = IndexActivity.GenerateSearchIndexFrom11tyEntries(entryRootInfo, indexRootInfo, indexFileName);
            Assert.True(indices?.Any());
        }
    }
}
