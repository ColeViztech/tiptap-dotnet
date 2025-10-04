using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Nodes;

public class ParagraphDomParserTests
{
    [Fact]
    public void SimpleTextGetsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p>Example Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example Text")));

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
