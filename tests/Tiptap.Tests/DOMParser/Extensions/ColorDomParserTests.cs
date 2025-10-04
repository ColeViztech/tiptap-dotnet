using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Extensions;

public class ColorDomParserTests
{
    [Fact]
    public void SpanWithColorStyleGetsRenderedCorrectly()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new TextStyle(),
                    new Color(),
                }))
            .SetContent("<p><span style=\"color: red;\">red text</span></p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text(
                    "red text",
                    Mark(
                        "textStyle",
                        new Dictionary<string, object?>
                        {
                            ["color"] = "red",
                        }))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void ColorExtensionRespectsTypesOption()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Color(new Dictionary<string, object?>
                    {
                        ["types"] = new[] { "heading" },
                    }),
                }))
            .SetContent("<h1 style=\"color: red;\">red heading</h1>")
            .GetDocument();

        var expected = Doc(
            Heading(
                new Dictionary<string, object?>
                {
                    ["level"] = 1,
                    ["color"] = "red",
                },
                Text("red heading")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
