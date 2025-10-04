using System.Collections.Generic;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class OrderedListDomSerializerTests
{
    [Fact]
    public void OrderedListNodeGetsRenderedCorrectly()
    {
        var document = Doc(
            OrderedList(
                ListItem(
                    Text("first list item"))));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<ol><li>first list item</li></ol>", result);
    }

    [Fact]
    public void OrderedListSupportsStartOffset()
    {
        var document = Doc(
            OrderedList(
                new Dictionary<string, object?>
                {
                    ["start"] = 3,
                },
                ListItem(
                    Text("first list item"))));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<ol start=\"3\"><li>first list item</li></ol>", result);
    }
}
