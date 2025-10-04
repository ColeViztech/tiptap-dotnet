using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Extensions;

public class FontFamilyDomParserTests
{
    [Fact]
    public void SpanWithFontFamilyStyleGetsRenderedCorrectly()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new TextStyle(),
                    new FontFamily(),
                }))
            .SetContent("<p><span style=\"font-family: Arial;\">Arial text</span></p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text(
                    "Arial text",
                    Mark(
                        "textStyle",
                        new Dictionary<string, object?>
                        {
                            ["fontFamily"] = "Arial",
                        }))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void SpanWithMultipleFontFamilyValuesGetsRenderedCorrectly()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new TextStyle(),
                    new FontFamily(),
                }))
            .SetContent(@"<p><span style=""font-family: Helvetica Neue, Arial, 'Times New Roman', &quot;Open Sans&quot;, sans-serif;"">Multiple fonts</span></p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text(
                    "Multiple fonts",
                    Mark(
                        "textStyle",
                        new Dictionary<string, object?>
                        {
                            ["fontFamily"] = "Helvetica Neue, Arial, 'Times New Roman', \"Open Sans\", sans-serif",
                        }))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void FontFamilyExtensionRespectsTypesOption()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new FontFamily(new Dictionary<string, object?>
                    {
                        ["types"] = new[] { "heading" },
                    }),
                }))
            .SetContent("<h1 style=\"font-family: Times New Roman;\">Times New Roman heading</h1>")
            .GetDocument();

        var expected = Doc(
            Heading(
                new Dictionary<string, object?>
                {
                    ["level"] = 1,
                    ["fontFamily"] = "Times New Roman",
                },
                Text("Times New Roman heading")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
