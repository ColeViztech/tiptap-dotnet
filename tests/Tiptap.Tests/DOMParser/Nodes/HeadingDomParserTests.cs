using System.Collections.Generic;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Nodes;

public class HeadingDomParserTests
{
    [Fact]
    public void H1IsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<h1>Example Text</h1>")
            .GetDocument();

        var expected = Doc(
            Heading(
                new Dictionary<string, object?>
                {
                    ["level"] = 1,
                },
                Text("Example Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void H2IsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<h2>Example Text</h2>")
            .GetDocument();

        var expected = Doc(
            Heading(
                new Dictionary<string, object?>
                {
                    ["level"] = 2,
                },
                Text("Example Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void H7IsIgnored()
    {
        var result = new EditorClass()
            .SetContent("<h7>Example Text</h7>")
            .GetDocument();

        var expected = Doc(
            Text("Example Text"));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
