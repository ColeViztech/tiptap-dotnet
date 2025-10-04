using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Marks;

public class HighlightDomParserTests
{
    [Fact]
    public void MarkTagGetsRenderedCorrectly()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Highlight(),
                }))
            .SetContent("<p><mark>Example</mark> Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example", Mark("highlight")),
                Text(" Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void ColorAttributeIsIgnoredByDefault()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Highlight(),
                }))
            .SetContent("<p><mark>Example</mark> Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example", Mark("highlight")),
                Text(" Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void ColorAttributeIsParsedFromDataAttribute()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Highlight(new Dictionary<string, object?>
                    {
                        ["multicolor"] = true,
                    }),
                }))
            .SetContent("<p><mark data-color=\"red\">Example</mark> Text</p>")
            .GetDocument();

        var expected = Doc(
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

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void ColorAttributeIsParsedFromBackgroundColorInlineStyle()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Highlight(new Dictionary<string, object?>
                    {
                        ["multicolor"] = true,
                    }),
                }))
            .SetContent("<p><mark style=\"background-color: #ffcc00\">Example</mark> Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text(
                    "Example",
                    Mark(
                        "highlight",
                        new Dictionary<string, object?>
                        {
                            ["color"] = "#ffcc00",
                        })),
                Text(" Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
