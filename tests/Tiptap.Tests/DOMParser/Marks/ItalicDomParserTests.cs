using EditorClass = Tiptap.Core.Editor;
using static Tiptap.Tests.ProseMirrorBuilder;

namespace Tiptap.Tests.DOMParser.Marks;

public class ItalicDomParserTests
{
    [Fact]
    public void ITagGetsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p><i>Example</i> Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example", Mark("italic")),
                Text(" Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void EmTagGetsRenderedCorrectly()
    {
        var result = new EditorClass()
            .SetContent("<p><em>Example</em> Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example", Mark("italic")),
                Text(" Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void ITagWithFontStyleNormalIsIgnored()
    {
        var result = new EditorClass()
            .SetContent("<p><i style=\"font-style: normal;\">Example</i> Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }

    [Fact]
    public void SpanWithFontStyleItalicIsParsed()
    {
        var result = new EditorClass()
            .SetContent("<p><span style=\"font-style: italic;\">Example</span> Text</p>")
            .GetDocument();

        var expected = Doc(
            Paragraph(
                Text("Example", Mark("italic")),
                Text(" Text")));

        Assert.Equal(JsonTestHelper.Serialize(expected), JsonTestHelper.Serialize(result));
    }
}
