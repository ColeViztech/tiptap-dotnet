using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class HardBreakDomSerializerTests
{
    [Fact]
    public void SelfClosingNodeGetsRenderedCorrectly()
    {
        var document = Doc(
            Paragraph(
                Text("some text"),
                HardBreak(),
                Text("some more text")));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p>some text<br>some more text</p>", result);
    }
}
