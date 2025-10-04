using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Marks;

public class SuperscriptDomSerializerTests
{
    [Fact]
    public void SuperscriptMarkGetsRenderedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Superscript(),
            }));

        var document = Doc(
            Text(
                "Example Text",
                Mark("superscript")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<sup>Example Text</sup>", result);
    }
}
