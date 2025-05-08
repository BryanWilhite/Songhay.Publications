namespace Songhay.Publications.Tests;

public class YamlUtilityTests(ITestOutputHelper helper)
{
    [Theory]
    [InlineData("""

                scalar: a scalar
                sequence:
                  - one
                  - two

                """)]
    public void ShouldDeserializeYaml(string yaml)
    {
        helper.WriteLine(yaml);
        helper.WriteLine("deserializing to `IDictionary<string, object>`...");

        IDictionary<string, object>? yO = YamlUtility.DeserializeYaml(yaml);

        Assert.NotNull(yO);
    }

    [Theory]
    [InlineData("""

                myString: a scalar
                myBoolean: true
                myDate: 2005-12-30T23:16:54.0000000
                sequence:
                  - one
                  - two

                """)]
    public void ShouldDeserializeYamlAndSerializeToJson(string yaml)
    {
        helper.WriteLine(yaml);
        helper.WriteLine("deserializing to `IDictionary<string, string?>`...");

        IDictionary<string, object>? yO = YamlUtility.DeserializeYaml(yaml);

        Assert.NotNull(yO);

        helper.WriteLine(yO.ToJsonString());
    }
}
