using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Marks;

public class HighlightDomSerializerTests
{
    [Fact]
    public void HighlightMarkDoesNotAllowSpecificColorsByDefault()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Highlight(),
            }));

        var document = Doc(
            Paragraph(
                Text(
                    "Example",
                    Mark(
                        "highlight",
                        new Dictionary<string, object?>
                        {
                            ["color"] = "red",
                        })),
                Text(" Text")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p><mark>Example</mark> Text</p>", result);
    }

    [Fact]
    public void HighlightMarkAllowsSpecificColorsWhenConfigured()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new Highlight(new Dictionary<string, object?>
                {
                    ["multicolor"] = true,
                }),
            }));

        var document = Doc(
            Paragraph(
                Text(
                    "Example",
                    Mark(
                        "highlight",
                        new Dictionary<string, object?>
                        {
                            ["color"] = "red",
                        })),
                Text(" Text")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p><mark data-color=\"red\" style=\"background-color: red\">Example</mark> Text</p>", result);
    }
}
