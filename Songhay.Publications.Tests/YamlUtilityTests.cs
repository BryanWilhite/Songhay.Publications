using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

using System.Text.Json.Nodes;

namespace Songhay.Publications.Tests;

public class YamlUtilityTests(ITestOutputHelper helper)
{
    [Theory]
    [InlineData("""

                scalar: 42
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

                myString: hello world
                myNumber: 42
                myBoolean: true
                myDate: 2005-12-30T23:16:54.0000000
                sequence:
                  - one
                  - two

                """)]
    public void ShouldDeserializeYamlAndSerializeToJson(string yaml)
    {
        helper.WriteLine(yaml);
        helper.WriteLine("deserializing to `IDictionary<string, object>`...");

        IDictionary<string, object>? data = YamlUtility.DeserializeYaml(yaml);

        Assert.NotNull(data);

        JsonObject jsonObject = new();

        foreach (KeyValuePair<string, object> kvp in data)
        {
            helper.WriteLine(kvp.Value.GetType().Name);
            jsonObject[kvp.Key] = JsonValue.Create(kvp.Value);
        }

        helper.WriteLine(jsonObject.ToJsonString());
    }

    [Theory]
    [InlineData(
        """
            documentId: 9609
            title: 'Michael A. Gonzales: 7 Questions from Invisible Woman'
            documentShortName: 
            fileName: kp_blackadelic.html
            path: ./
            templateId: 9059
            segmentId: 4585
            isRoot: true
            isActive: true
            sortOrdinal: 
            clientId: 
            endDate: 
            inceptDate: 2008-03-28T13:38:00.0000000
            modificationDate: 2011-07-13T12:18:38.3400000
            """
            )]
    public void ShouldDeserializeYamlToDocument(string yaml)
    {
        IDeserializer deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithAttributeOverride(typeof(Document), nameof(Document.ClientId), new YamlIgnoreAttribute())
            .Build();

        Document actual = deserializer.Deserialize<Document>(yaml);

        helper.WriteLine(actual.ToDisplayText());
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private record OrderItem
    {
        // ReSharper disable once UnusedMember.Local
        public string? PartNumber { get; set; }
        // ReSharper disable once UnusedMember.Local
        public string? Description { get; set; }
        // ReSharper disable once UnusedMember.Local
        public decimal? Price { get; set; }
        // ReSharper disable once UnusedMember.Local
        public int? Quantity { get; set; }
    }

    ///<remarks>
    /// This test verifies that YamlDotNet:
    /// - supports records
    /// - supports non-public types
    /// - supports properties with nullable types
    /// - can be configured to ignore properties not on a specified type
    ///</remarks>
    [Theory]
    [InlineData(
        """
        partNumber: WXY5527-INC
        description: This is the widget of the other widget.
        price: 59.99
        quantity: 42
        tags:
        - one
        - two
        """)]
    public void ShouldDeserializeYamlToOrderItem(string yaml)
    {
        IDeserializer deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        OrderItem actual = deserializer.Deserialize<OrderItem>(yaml);

        helper.WriteLine($"{actual}");
    }
}
