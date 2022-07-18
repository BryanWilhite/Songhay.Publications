using System.Diagnostics;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Publications.Extensions;
using Songhay.Publications.Models;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.Publications.Tests.Extensions;

public class IFragmentExtensionsTests
{
    static IFragmentExtensionsTests() => traceSource = TraceSources
        .Instance
        .GetTraceSourceFromConfiguredName()
        .WithSourceLevels();

    static readonly TraceSource traceSource;

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
            .GetFragmentByPredicate(i => i.ClientId == clientId);

        Assert.NotNull(first);
        Assert.Equal(clientId, first.ClientId);
    }

    [Fact]
    public void ToDisplayText_Test()
    {
        var testCollection = new (IFragment data, Func<IFragment, bool> test)[]
        {
            (
                null,
                data =>
                {
                    var text = data.ToDisplayText();
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
                    return
                        text.Contains(data.ClientId) &&
                        text.Contains(data.FragmentDisplayName) &&
                        text.Contains(data.FragmentName) &&
                        text.Contains(data.IsActive.ToString()) &&
                        !text.Contains(DateTime.Now.Day.ToString())
                        ;
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
                    return
                        text.Contains(data.ClientId) &&
                        text.Contains(data.FragmentDisplayName) &&
                        text.Contains(data.FragmentName) &&
                        text.Contains(data.IsActive.ToString()) &&
                        !text.Contains(DateTime.Now.Day.ToString())
                        ;
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
                    return
                        text.Contains(data.FragmentId.ToString()) &&
                        text.Contains(data.ClientId) &&
                        !text.Contains(data.FragmentDisplayName) &&
                        !text.Contains(data.FragmentName) &&
                        !text.Contains(data.IsActive.ToString()) &&
                        !text.Contains(DateTime.Now.Day.ToString())
                        ;
                }
            ),
        };

        foreach (var item in testCollection)
        {
            Assert.True(item.test(item.data));
        }
    }

    readonly ITestOutputHelper _testOutputHelper;
}