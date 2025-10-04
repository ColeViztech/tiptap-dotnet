using System.Collections.Generic;
using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using static Tiptap.Tests.ProseMirrorBuilder;
using EditorClass = Tiptap.Core.Editor;

namespace Tiptap.Tests.DOMSerializer.Extensions;

public class TextAlignDomSerializerTests
{
    [Fact]
    public void TextAlignIsRenderedCorrectly()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new TextAlign(new Dictionary<string, object?>
                {
                    ["types"] = new[] { "paragraph" },
                }),
            }));

        var document = Doc(
            Paragraph(
                new Dictionary<string, object?>
                {
                    ["textAlign"] = "center",
                },
                Text("Example Text")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p style=\"text-align: center\">Example Text</p>", result);
    }

    [Fact]
    public void DefaultTextAlignIsNotRendered()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new TextAlign(new Dictionary<string, object?>
                {
                    ["types"] = new[] { "paragraph" },
                }),
            }));

        var document = Doc(
            Paragraph(
                new Dictionary<string, object?>
                {
                    ["textAlign"] = "left",
                },
                Text("Example Text")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p>Example Text</p>", result);
    }

    [Fact]
    public void DefaultTextAlignIsConfigurable()
    {
        var editor = new EditorClass(new EditorOptions(
            Extensions: new Extension[]
            {
                new StarterKit(),
                new TextAlign(new Dictionary<string, object?>
                {
                    ["types"] = new[] { "paragraph" },
                    ["defaultAlignment"] = "center",
                }),
            }));

        var document = Doc(
            Paragraph(
                new Dictionary<string, object?>
                {
                    ["textAlign"] = "center",
                },
                Text("Example Text")));

        var result = editor
            .SetContent(document)
            .GetHTML();

        Assert.Equal("<p>Example Text</p>", result);
    }
}
