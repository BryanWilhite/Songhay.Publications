using System.IO;
using System.Text.Json;
using Songhay.Publications.Extensions;
using Songhay.Publications.Models;
using Songhay.Tests;
using Xunit;

namespace Songhay.Publications.Tests.Extensions
{
    public class ResponsiveImageExtensionsTests
    {
        [Theory]
        [ProjectFileData(typeof(ResponsiveImageExtensionsTests),
            "../../../json/ToImgMarkup_Test_test0_input.json",
            "../../../txt/ToImgMarkup_Test_test0_output.txt")]
        [ProjectFileData(typeof(ResponsiveImageExtensionsTests),
            "../../../json/ToImgMarkup_Test_test1_input.json",
            "../../../txt/ToImgMarkup_Test_test1_output.txt")]
        public void ToImgMarkup_Test(FileInfo inputInfo, FileInfo outputInfo)
        {
            var json = File.ReadAllText(inputInfo.FullName);

            var responsiveImage = JsonSerializer.Deserialize<ResponsiveImage>(json);
            Assert.NotNull(responsiveImage);

            var txt = responsiveImage.ToImgMarkup();
            File.WriteAllText(outputInfo.FullName, txt);
        }
    }
}
