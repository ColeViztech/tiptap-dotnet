using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class ParagraphDomSerializerTests
{
    [Fact]
    public void ParagraphNodeGetsRenderedCorrectly()
    {
        var document = Doc(
            Paragraph(
                Text("Example Paragraph")));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p>Example Paragraph</p>", result);
    }
}
