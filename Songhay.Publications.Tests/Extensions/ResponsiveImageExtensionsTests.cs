namespace Songhay.Publications.Tests.Extensions;

public class ResponsiveImageExtensionsTests
{
    [Theory]
    [ProjectFileData(typeof(ResponsiveImageExtensionsTests),
        "../../../json/ToCssMediaAtRules_Test_00_input.json",
        "../../../txt/ToCssMediaAtRules_Test_00_output.txt")]
    public void ToCssMediaAtRules_Test(FileInfo inputInfo, FileInfo outputInfo)
    {
        var json = File.ReadAllText(inputInfo.FullName);

        var responsiveImage = JsonSerializer.Deserialize<ResponsiveImage>(json);
        Assert.NotNull(responsiveImage);

        var txt = responsiveImage.ToCssMediaAtRules();
        File.WriteAllText(outputInfo.FullName, txt);
    }

    [Theory]
    [ProjectFileData(typeof(ResponsiveImageExtensionsTests),
        "../../../json/ToImgMarkup_Test_00_input.json",
        "../../../txt/ToImgMarkup_Test_00_output.txt")]
    [ProjectFileData(typeof(ResponsiveImageExtensionsTests),
        "../../../json/ToImgMarkup_Test_01_input.json",
        "../../../txt/ToImgMarkup_Test_01_output.txt")]
    public void ToImgMarkup_Test(FileInfo inputInfo, FileInfo outputInfo)
    {
        var json = File.ReadAllText(inputInfo.FullName);

        var responsiveImage = JsonSerializer.Deserialize<ResponsiveImage>(json);
        Assert.NotNull(responsiveImage);

        var txt = responsiveImage.ToImgMarkup();
        File.WriteAllText(outputInfo.FullName, txt);
    }
}