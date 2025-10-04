using System.Collections.Generic;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Nodes;

public class OrderedListDomParserTests
{
    [Fact]
    public void OrderedListGetsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<ol><li><p>Example</p></li><li><p>Text</p></li></ol>")
            .GetDocument();

        var expected = Doc(
            OrderedList(
                ListItem(
                    Paragraph(
                        Text("Example"))),
                ListItem(
                    Paragraph(
                        Text("Text")))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void OrderedListHasCorrectOffset()
    {
        var result = new EditorClass()
            .SetContent("<ol start=\"3\"><li><p>Example</p></li><li><p>Text</p></li></ol>")
            .GetDocument();

        var expected = Doc(
            OrderedList(
                new Dictionary<string, object?>
                {
                    ["start"] = 3,
                },
                ListItem(
                    Paragraph(
                        Text("Example"))),
                ListItem(
                    Paragraph(
                        Text("Text")))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
