using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Extensions;

public class TextAlignDomParserTests
{
    [Fact]
    public void ParagraphWithTextAlignStyleGetsRenderedCorrectly()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new TextAlign(new Dictionary<string, object?>
                    {
                        ["types"] = new[] { "paragraph" },
                    }),
                }))
            .SetContent("<p style=\"text-align: center;\">Example Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                new Dictionary<string, object?>
                {
                    ["textAlign"] = "center",
                },
                Text("Example Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void ParagraphWithoutTextAlignUsesDefaultAlignment()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new TextAlign(new Dictionary<string, object?>
                    {
                        ["types"] = new[] { "paragraph" },
                    }),
                }))
            .SetContent("<p>Example Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                new Dictionary<string, object?>
                {
                    ["textAlign"] = "left",
                },
                Text("Example Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void DefaultTextAlignCanBeConfigured()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new TextAlign(new Dictionary<string, object?>
                    {
                        ["types"] = new[] { "paragraph" },
                        ["defaultAlignment"] = "center",
                    }),
                }))
            .SetContent("<p>Example Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                new Dictionary<string, object?>
                {
                    ["textAlign"] = "center",
                },
                Text("Example Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
