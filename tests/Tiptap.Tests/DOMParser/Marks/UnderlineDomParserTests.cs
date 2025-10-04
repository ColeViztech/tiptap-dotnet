using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Marks;

public class UnderlineDomParserTests
{
    [Fact]
    public void UnderlineTagGetsRenderedCorrectly()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Underline(),
                }))
            .SetContent("<p><u>Example Text</u></p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example Text", Mark("underline"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void SpanWithUnderlineStyleIsParsed()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new Underline(),
                }))
            .SetContent("<p><span style=\"text-decoration: underline;\">Example Text</span></p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example Text", Mark("underline"))));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
