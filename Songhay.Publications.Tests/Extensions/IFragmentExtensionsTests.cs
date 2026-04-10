using Songhay.Publications.Abstractions;

namespace Songhay.Publications.Tests.Extensions;

// ReSharper disable once InconsistentNaming
public class IFragmentExtensionsTests
{
    [Fact]
    public void GetFragmentByPredicate_Test()
    {
        const string clientId = "my-data";

        Fragment[] collection =
        [
            new(),
            new(),
            new() { ClientId = clientId },
            new()
        ];

        IFragment first = collection
            .GetFragmentByPredicate(i => i.ClientId == clientId)
            .ToReferenceTypeValueOrThrow();

        Assert.Equal(clientId, first.ClientId);
    }

    public static TheoryData<IFragment?, Func<IFragment?, bool>> ToDisplayTextTheoryData = new()
    {
        {
            null,
            data =>
            {
                string text = data.ToDisplayText();

                return text.Contains("the specified ") && text.Contains("is null.");
            }
        },
        {
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
                string text = data.ToDisplayText();

                return data switch
                {
                    null => false,
                    _ =>
                        !string.IsNullOrEmpty(data.ClientId) && text.Contains(data.ClientId) &&
                        !string.IsNullOrEmpty(data.FragmentDisplayName) && text.Contains(data.FragmentDisplayName) &&
                        !string.IsNullOrEmpty(data.FragmentName) && text.Contains(data.FragmentName) &&
                        text.Contains($"{data.IsActive}")
                };
            }
        },
        {
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
                string text = $"{data}";

                return data switch
                {
                    null => false,
                    _ =>
                        !string.IsNullOrEmpty(data.ClientId) && text.Contains(data.ClientId) &&
                        !string.IsNullOrEmpty(data.FragmentDisplayName) && text.Contains(data.FragmentDisplayName) &&
                        !string.IsNullOrEmpty(data.FragmentName) && text.Contains(data.FragmentName) &&
                        text.Contains($"{data.IsActive}")
                };
            }
        },
        {
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
                string text = data.ToDisplayText(showIdOnly: true);

                return data switch
                {
                    null => false,
                    _ =>
                        text.Contains($"{data.FragmentId}") &&
                        !string.IsNullOrEmpty(data.ClientId) && text.Contains(data.ClientId) &&
                        !string.IsNullOrEmpty(data.FragmentDisplayName) && !text.Contains(data.FragmentDisplayName) &&
                        !string.IsNullOrEmpty(data.FragmentName) && !text.Contains(data.FragmentName) &&
                        !text.Contains($"{data.IsActive}")
                };
            }
        }
    };

    [Theory, MemberData(nameof(ToDisplayTextTheoryData))]
    public void ToDisplayText_Test(IFragment? data, Func<IFragment?, bool> test)
    {
        Assert.True(test(data));
    }
}