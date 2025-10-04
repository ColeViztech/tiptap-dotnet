using Tiptap.Core;
using Tiptap.Core.Core;
using Tiptap.Core.Extensions;
using Tiptap.Core.Marks;
using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Marks;

public class TextStyleDomParserTests
{
    [Fact]
    public void SpanWithInlineStyleGetsRenderedCorrectly()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new TextStyle(),
                }))
            .SetContent("<p><span style=\"color: red\">Example</span> Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example", Mark("textStyle")),
                Text(" Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void SpanWithoutInlineStyleIsIgnored()
    {
        var result = new EditorClass(new EditorOptions(
                Extensions: new Extension[]
                {
                    new StarterKit(),
                    new TextStyle(),
                }))
            .SetContent("<p><span>Example</span> Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
