using Songhay.Publications.Activities;

namespace Songhay.Publications.Tests.Activities;

public class MarkdownEntryActivityTests
{
    [Theory]
    [InlineData("one, three, four", "one,", "one, two,", false, "one, two, three, four")]
    [InlineData("one, three, four", @"\w+", "_", true, "_, _, _")]
    [InlineData(@"one
three
four", @"^three", null, true, @"one

four")]
    [InlineData("one, three, four", @"\w+", "$&_", true, "one_, three_, four_")]
    public void FindChange_Test(string input, string pattern, string replacement, bool useRegex, string expectedResult)
    {
        var actual = MarkdownEntryActivity.FindChange(input, pattern, replacement, useRegex);
        Assert.Equal(expectedResult, actual);
    }
}