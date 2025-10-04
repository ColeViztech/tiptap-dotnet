using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class HorizontalRuleDomSerializerTests
{
    [Fact]
    public void SelfClosingNodeGetsRenderedCorrectly()
    {
        var document = Doc(
            Paragraph(
                Text("some text")),
            HorizontalRule(),
            Paragraph(
                Text("some more text")));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p>some text</p><hr><p>some more text</p>", result);
    }
}
