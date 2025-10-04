using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser;

public class EmojiDomParserTests
{
    [Fact]
    public void EmojisAreTransformedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p>🔥</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("🔥")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
