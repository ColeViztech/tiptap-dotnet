using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMSerializer.Nodes;

public class BulletListDomSerializerTests
{
    [Fact]
    public void BulletListNodeGetsRenderedCorrectly()
    {
        var document = Doc(
            BulletList(
                ListItem(
                    Text("first list item"))));

        var result = new EditorClass()
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<ul><li>first list item</li></ul>", result);
    }
}
