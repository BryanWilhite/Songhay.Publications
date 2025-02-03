namespace Songhay.Publications.Tests;

public class YamlUtilityTests
{
    public YamlUtilityTests(ITestOutputHelper helper)
    {
        _testOutputHelper = helper;
    }

    [Theory]
    [InlineData(@"
scalar: a scalar
sequence:
  - one
  - two
")]
    public void ShouldDeserializeYaml(string yaml)
    {
        _testOutputHelper.WriteLine(yaml);
        _testOutputHelper.WriteLine("deserializing to `IDictionary<string, object>`...");

        IDictionary<string, object>? yO = YamlUtility.DeserializeYaml(yaml);

        Assert.NotNull(yO);
    }

    [Theory]
    [InlineData(@"
myString: a scalar
myBoolean: true
myDate: 2005-12-30T23:16:54.0000000
sequence:
  - one
  - two
")]
    public void ShouldDeserializeYamlAndSerializeToJson(string yaml)
    {
        _testOutputHelper.WriteLine(yaml);
        _testOutputHelper.WriteLine("deserializing to `IDictionary<string, string?>`...");

        IDictionary<string, object>? yO = YamlUtility.DeserializeYaml(yaml);

        Assert.NotNull(yO);

        _testOutputHelper.WriteLine(yO.ToJsonString());
    }

    readonly ITestOutputHelper _testOutputHelper;
}
