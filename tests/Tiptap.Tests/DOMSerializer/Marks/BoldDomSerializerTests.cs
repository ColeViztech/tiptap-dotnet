using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Marks;

public class BoldDomSerializerTests
{
    [Fact]
    public void BoldMarkGetsRenderedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
            }));

        var document = Doc(
            Text(
                "Example Text",
                Mark("bold")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<strong>Example Text</strong>", result);
    }
}
