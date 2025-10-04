using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Nodes;

public class BlockquoteDomParserTests
{
    [Fact]
    public void BlockquoteGetsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<blockquote><p>Paragraph</p></blockquote>")
            .GetDocument();

        var expected = Doc(
            Blockquote(
                Paragraph(
                    Text("Paragraph"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
