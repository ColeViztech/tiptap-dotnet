using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Nodes;

public class HardBreakDomParserTests
{
    [Fact]
    public void BreakGetsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p>Hard <br />Break</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Hard "),
                HardBreak(),
                Text("Break")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void MultipleNodesGetRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p>Example</p><p>Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example")),
            Paragraph(
                Text("Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
