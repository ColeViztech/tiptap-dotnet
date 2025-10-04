using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class BlockquoteDomSerializerTests
{
    [Fact]
    public void BlockquoteNodeGetsRenderedCorrectly()
    {
        var document = Doc(
            Blockquote(
                Text("Example Quote")));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<blockquote>Example Quote</blockquote>", result);
    }
}
