using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Extensions;

public class ColorDomSerializerTests
{
    [Fact]
    public void ColorIsRenderedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new TextStyle(),
                new Color(),
            }));

        var document = Doc(
            Paragraph(
                Text(
                    "red text",
                    Mark(
                        "textStyle",
                        new Dictionary<string, object?>
                        {
                            ["color"] = "red",
                        }))));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p><span style=\"color: red\">red text</span></p>", result);
    }
}
