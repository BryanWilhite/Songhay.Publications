namespace Songhay.Publications.Tests.Extensions;

public class ResponsiveImageExtensionsTests
{
    [Theory]
    [ProjectFileData("../../../test-files/json/ToCssMediaAtRules_Test_00_input.json",
        "../../../test-files/txt/ToCssMediaAtRules_Test_00_output.txt")]
    public void ToCssMediaAtRules_Test(FileInfo inputInfo, FileInfo outputInfo)
    {
        string json = File.ReadAllText(inputInfo.FullName);

        ResponsiveImage? responsiveImage = JsonSerializer.Deserialize<ResponsiveImage>(json);
        Assert.NotNull(responsiveImage);

        string txt = responsiveImage.ToCssMediaAtRules();
        File.WriteAllText(outputInfo.FullName, txt);
    }

    [Theory]
    [ProjectFileData("../../../test-files/json/ToImgMarkup_Test_00_input.json",
        "../../../test-files/txt/ToImgMarkup_Test_00_output.txt")]
    [ProjectFileData("../../../test-files/json/ToImgMarkup_Test_01_input.json",
        "../../../test-files/txt/ToImgMarkup_Test_01_output.txt")]
    [ProjectFileData("../../../test-files/json/ToImgMarkup_Test_02_input.json",
        "../../../test-files/txt/ToImgMarkup_Test_02_output.txt")]
    public void ToImgMarkup_Test(FileInfo inputInfo, FileInfo outputInfo)
    {
        string json = File.ReadAllText(inputInfo.FullName);

        ResponsiveImage? responsiveImage = JsonSerializer.Deserialize<ResponsiveImage>(json);
        Assert.NotNull(responsiveImage);

        string txt = responsiveImage.ToImgMarkup();
        File.WriteAllText(outputInfo.FullName, txt);
    }
}
