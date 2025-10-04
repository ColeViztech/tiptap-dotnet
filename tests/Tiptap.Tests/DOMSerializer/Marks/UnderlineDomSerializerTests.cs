using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Marks;

public class UnderlineDomSerializerTests
{
    [Fact]
    public void UnderlineMarkGetsRenderedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Underline(),
            }));

        var document = Doc(
            Text(
                "Example Text",
                Mark("underline")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<u>Example Text</u>", result);
    }
}
