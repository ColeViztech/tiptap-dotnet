using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Extensions;

public class FontFamilyDomSerializerTests
{
    [Fact]
    public void FontFamilyIsRenderedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new TextStyle(),
                new FontFamily(),
            }));

        var document = Doc(
            Paragraph(
                Text(
                    "custom font text",
                    Mark(
                        "textStyle",
                        new Dictionary<string, object?>
                        {
                            ["fontFamily"] = "Helvetica, Arial, sans-serif",
                        }))));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p><span style=\"font-family: Helvetica, Arial, sans-serif\">custom font text</span></p>", result);
    }
}
