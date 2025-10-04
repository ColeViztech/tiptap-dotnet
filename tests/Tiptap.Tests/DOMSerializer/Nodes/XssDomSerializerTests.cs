using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class XssDomSerializerTests
{
    [Fact]
    public void TextShouldNotBeRenderedAsHtml()
    {
        var result = new EditorClass()
            .SetContent("<script>alert(1)</script>")
            .GetHTML();

        Assert.Equal(string.Empty, result);
    }
}
