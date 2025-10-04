using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Marks;

public class SubscriptDomSerializerTests
{
    [Fact]
    public void SubscriptMarkGetsRenderedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Subscript(),
            }));

        var document = Doc(
            Text(
                "Example Text",
                Mark("subscript")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<sub>Example Text</sub>", result);
    }
}
