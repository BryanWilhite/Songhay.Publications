using Songhay.Publications.Abstractions;

namespace Songhay.Publications.Tests.Extensions;

// ReSharper disable once InconsistentNaming
public class IFragmentExtensionsTests
{
    public IFragmentExtensionsTests(ITestOutputHelper helper)
    {
        _testOutputHelper = helper;
    }

    [Fact]
    public void GetFragmentByPredicate_Test()
    {
        var clientId = "my-data";

        var collection = new[]
        {
            new Fragment(),
            new Fragment(),
            new Fragment { ClientId = clientId },
            new Fragment(),
        };

        var first = collection
            .GetFragmentByPredicate(i => i.ClientId == clientId)
            .ToReferenceTypeValueOrThrow();

        Assert.Equal(clientId, first.ClientId);
    }

    [Fact]
    public void ToDisplayText_Test()
    {
        var testCollection = new (IFragment? data, Func<IFragment?, bool> test)[]
        {
            (
                null,
                data =>
                {
                    var text = data.ToDisplayText();
                    _testOutputHelper.WriteLine(text);

                    return text.Contains("the specified ") && text.Contains("is null.");
                }
            ),
            (
                new Fragment
                {
                    ClientId = "my-fragment",
                    FragmentDisplayName = "my-display-name",
                    FragmentName = "my-name",
                    IsActive =  true,
                    InceptDate = DateTime.Now
                },
                data =>
                {
                    var text = data.ToDisplayText();
                    _testOutputHelper.WriteLine(text);

                    return data switch
                    {
                        null => false,
                        _ =>
                            !string.IsNullOrEmpty(data.ClientId) && text.Contains(data.ClientId) &&
                            !string.IsNullOrEmpty(data.FragmentDisplayName) && text.Contains(data.FragmentDisplayName) &&
                            !string.IsNullOrEmpty(data.FragmentName) && text.Contains(data.FragmentName) &&
                            text.Contains($"{data.IsActive}") &&
                            !text.Contains(DateTime.Now.Day.ToString())
                    };
                }
            ),
            (
                new Fragment
                {
                    ClientId = "my-fragment",
                    FragmentDisplayName = "my-display-name",
                    FragmentName = "my-name",
                    IsActive =  true,
                    InceptDate = DateTime.Now
                },
                data =>
                {
                    var text = $"{data}";
                    _testOutputHelper.WriteLine(text);

                    return data switch
                    {
                        null => false,
                        _ =>
                            !string.IsNullOrEmpty(data.ClientId) && text.Contains(data.ClientId) &&
                            !string.IsNullOrEmpty(data.FragmentDisplayName) && text.Contains(data.FragmentDisplayName) &&
                            !string.IsNullOrEmpty(data.FragmentName) && text.Contains(data.FragmentName) &&
                            text.Contains($"{data.IsActive}") &&
                            !text.Contains(DateTime.Now.Day.ToString())
                    };
                }
            ),
            (
                new Fragment
                {
                    FragmentId = 999,
                    ClientId = "my-fragment",
                    FragmentDisplayName = "my-display-name",
                    FragmentName = "my-name",
                    IsActive =  true,
                    InceptDate = DateTime.Now
                },
                data =>
                {
                    var text = data.ToDisplayText(showIdOnly: true);
                    _testOutputHelper.WriteLine(text);

                    return data switch
                    {
                        null => false,
                        _ =>
                            text.Contains($"{data.FragmentId}") &&
                            !string.IsNullOrEmpty(data.ClientId) && text.Contains(data.ClientId) &&
                            !string.IsNullOrEmpty(data.FragmentDisplayName) && !text.Contains(data.FragmentDisplayName) &&
                            !string.IsNullOrEmpty(data.FragmentName) && !text.Contains(data.FragmentName) &&
                            !text.Contains($"{data.IsActive}") &&
                            !text.Contains(DateTime.Now.Day.ToString())
                    };
                }
            )
        };

        foreach (var item in testCollection)
        {
            Assert.True(item.test(item.data));
        }
    }

    readonly ITestOutputHelper _testOutputHelper;
}