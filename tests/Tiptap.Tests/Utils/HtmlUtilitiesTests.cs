using System.Collections.Generic;
using Tiptap.Core.Utils;

namespace Tiptap.Tests.Utils;

public class HtmlUtilitiesTests
{
    [Fact]
    public void MergeAttributesMergesClassNames()
    {
        var result = HtmlUtilities.MergeAttributes(
            new Dictionary<string, object?>
            {
                ["class"] = "a",
            },
            new Dictionary<string, object?>
            {
                ["class"] = "b",
            });

        Assert.True(result.TryGetValue("class", out var classes));
        Assert.Equal("a b", classes);
    }
}
